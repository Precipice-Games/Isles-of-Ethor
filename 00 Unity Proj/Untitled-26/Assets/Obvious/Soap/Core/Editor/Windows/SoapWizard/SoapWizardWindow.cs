using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using PopupWindow = UnityEditor.PopupWindow;

namespace Obvious.Soap.Editor
{
    public partial class SoapWizardWindow : EditorWindow
    {
        private Vector2 _scrollPosition = Vector2.zero;
        private List<ScriptableBase> _scriptableObjects;
        private ScriptableType _currentType = ScriptableType.All;
        private float TabWidth => position.width / 6;
        private string _searchText = "";

        [SerializeField] private string _currentFolderPath = "Assets";
        [SerializeField] private int _selectedScriptableIndex;
        [SerializeField] private int _typeTabIndex = -1;
        [SerializeField] private int _tagMask;
        [SerializeField] private bool _isInitialized;
        [SerializeField] private ScriptableBase _scriptableBase;
        [SerializeField] private ScriptableBase _previousScriptableBase;
        [SerializeField] private FavoriteData _favoriteData;

        private SoapSettings _soapSettings;
        private readonly float _widthRatio = 0.6f;

        //Cache
        private Texture[] _icons;
        private UnityEditor.Editor _editor;
        private Dictionary<ScriptableBase, Object> _subAssetsLookup;

        private Dictionary<ScriptableBase, SerializedObject> _serializedObjects =
            new Dictionary<ScriptableBase, SerializedObject>();

        private readonly List<int> _indicesToDraw = new List<int>();
        private GUIStyle _entryStyle;
        private GUIStyle _iconStyle;
        private GUIStyle _buttonStyle;
        private GUIContent _guiContent;
        private Rect _bgRect;
        private Rect _entryNameRect;
        private Rect _iconRect;
        private readonly Color _selectedColor = new Color(0.172f, 0.365f, 0.529f);
        private readonly Color _hoverColor = new Color(0.3f, 0.3f, 0.3f);
        private Event _currentEvent;
        private string[] _tags;
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private readonly string[] _tabNames = Enum.GetNames(typeof(ScriptableType));
        public static bool IsPopupOpen = false;

        private enum ScriptableType
        {
            All,
            Variables,
            Events,
            Collections,
            Enums,
            Favorites
        }

        [MenuItem("Window/Obvious/\ud83e\uddfc Soap/Soap Wizard")]
        public new static void Show()
        {
            var window = GetWindow<SoapWizardWindow>(typeof(SceneView));
            window.titleContent = new GUIContent("Soap Wizard", Resources.Load<Texture>("Icons/icon_soapLogo"));
        }

        [MenuItem("Tools/Obvious/\ud83e\uddfc Soap/Soap Wizard %#w")]
        private static void OpenSoapWizard() => Show();

        private void OnEnable()
        {
            _soapSettings = SoapEditorUtils.GetOrCreateSoapSettings();
            this.wantsMouseMove = true;
            LoadIcons();
            LoadSavedData();
            _tags = _soapSettings.Tags.ToArray();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (_isInitialized)
            {
                SelectTab(_typeTabIndex);
                return;
            }

            SelectTab((int)_currentType, true); //default is 0
            _isInitialized = true;
        }

        private void OnDisable()
        {
            SoapEditorUtils.WizardTags = _tagMask;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            DestroyImmediate(_editor);
            foreach (var serializedObject in _serializedObjects.Values)
            {
                serializedObject.Dispose();
            }

            _serializedObjects.Clear();
        }

        private void OnFocus()
        {
            if (_soapSettings == null)
                return;
            //Tags can be modified from different windows, so always update them when focusing back
            _tags = _soapSettings.Tags.ToArray();
        }

        private void LoadIcons()
        {
            _icons = new Texture[8];
            _icons[0] = EditorGUIUtility.IconContent("Favorite On Icon").image;
            _icons[1] = Resources.Load<Texture>("Icons/icon_edit");
            _icons[2] = Resources.Load<Texture>("Icons/icon_duplicate");
            _icons[3] = Resources.Load<Texture>("Icons/icon_delete");
            _icons[4] = EditorGUIUtility.IconContent("Favorite Icon").image;
            _icons[5] = EditorGUIUtility.IconContent("Folder Icon").image;
            _icons[6] = Resources.Load<Texture>("Icons/icon_ping");
            _icons[7] = EditorGUIUtility.IconContent("Settings").image;
        }

        private void LoadSavedData()
        {
            _currentFolderPath = SoapEditorUtils.WizardRootFolderPath;
            _favoriteData = FavoriteData.Load();
            _tagMask = SoapEditorUtils.WizardTags;
        }

        private void CacheStyles()
        {
            if (_entryStyle != null)
                return;

            _entryStyle = new GUIStyle(EditorStyles.label)
            {
                normal = { textColor = Color.white }
            };
            _iconStyle = new GUIStyle(GUIStyle.none);
            _guiContent = new GUIContent();
            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.margin = new RectOffset(0, 2, 0, 0);
            _buttonStyle.padding = new RectOffset(4, 4, 4, 4);
        }

        private void OnGUI()
        {
            if (_soapSettings == null)
                return;

            CacheStyles();

            _currentEvent = Event.current;

            var padding = 2f;
            var paddedArea = new Rect(padding, padding, position.width - (padding * 2),
                position.height - (padding * 2));

            GUILayout.BeginArea(paddedArea);
            DrawFolder();
            GUILayout.Space(1);
            SoapInspectorUtils.DrawLine();
            GUILayout.Space(1);
            DrawTags();
            GUILayout.Space(2);
            DrawSearchBar();
            SoapInspectorUtils.DrawColoredLine(1, Color.black.Lighten(0.137f));
            DrawTabs();
            DrawScriptableBases(_scriptableObjects);
            DrawBottomButtons();
            GUILayout.EndArea();

            CheckShortcutInputs();
            if (_currentEvent.type == EventType.MouseMove && !IsPopupOpen)
            {
                Repaint();
            }
        }

        private void CheckShortcutInputs()
        {
            // Handle arrow key navigation to navigate between tabs
            if (_currentEvent.type == EventType.KeyDown)
            {
                int increment = 0;
                if (_currentEvent.keyCode == KeyCode.LeftArrow)
                {
                    increment = -1;
                    _currentEvent.Use();
                }
                else if (_currentEvent.keyCode == KeyCode.RightArrow)
                {
                    increment = 1;
                    _currentEvent.Use();
                }

                if (increment != 0)
                {
                    _typeTabIndex = (_typeTabIndex + increment + Enum.GetNames(typeof(ScriptableType)).Length) %
                                    Enum.GetNames(typeof(ScriptableType)).Length;
                    SelectTab(_typeTabIndex, true);
                    Repaint();
                }
            }

            //Handle arrow key up down to navigate between scriptables
            if (_currentEvent.type == EventType.KeyDown && _scriptableObjects.Count > 0)
            {
                int increment = 0;
                if (_currentEvent.keyCode == KeyCode.UpArrow)
                {
                    increment = -1;
                    _currentEvent.Use();
                }
                else if (_currentEvent.keyCode == KeyCode.DownArrow)
                {
                    increment = 1;
                    _currentEvent.Use();
                }

                if (increment != 0)
                {
                    var selected = _selectedScriptableIndex;
                    Deselect();
                    _selectedScriptableIndex = (selected + increment + _scriptableObjects.Count) %
                                               _scriptableObjects.Count;
                    _scriptableBase = _scriptableObjects[_selectedScriptableIndex];
                    Repaint();
                }
            }

            if (_currentEvent.type == EventType.KeyDown)
            {
                // Handle CTRL+F keyboard event to focus search bar
                if (_currentEvent.keyCode == KeyCode.F &&
                    _currentEvent.control)
                {
                    _currentEvent.Use();
                    EditorGUI.FocusTextInControl("SearchBar");
                    return;
                }

                if (_scriptableBase == null)
                    return;

                if (_currentEvent.keyCode == KeyCode.F)
                {
                    if (_favoriteData.IsFavorite(_scriptableBase))
                        _favoriteData.RemoveFavorite(_scriptableBase);
                    else
                        _favoriteData.AddFavorite(_scriptableBase);
                }
                else if (_currentEvent.keyCode == KeyCode.P)
                {
                    Selection.activeObject = _scriptableBase;
                    EditorGUIUtility.PingObject(_scriptableBase);
                }
                else if (_currentEvent.keyCode == KeyCode.R &&
                         _currentEvent.control)
                {
                    ReferencesPopupWindow.ShowWindow(position, _scriptableBase);
                }
                else if (_currentEvent.keyCode == KeyCode.R)
                {
                    PopupWindow.Show(new Rect(), new RenamePopUpWindow(position, _scriptableBase));
                }
                else if (_currentEvent.keyCode == KeyCode.T)
                {
                    var scriptableBases = new[] { _scriptableBase };
                    PopupWindow.Show(new Rect(), new SetTagPopupWindow(scriptableBases));
                }
                else if (_currentEvent.keyCode == KeyCode.D)
                {
                    if (AssetDatabase.IsMainAsset(_scriptableBase))
                    {
                        SoapEditorUtils.CreateCopy(_scriptableBase);
                        Refresh(_currentType);
                    }
                }
                else if (_currentEvent.keyCode == KeyCode.Delete)
                {
                    var isDeleted = SoapEditorUtils.DeleteObjectWithConfirmation(_scriptableBase);
                    if (isDeleted)
                    {
                        _scriptableBase = null;
                        OnTabSelected(_currentType, true);
                    }
                }

                _currentEvent.Use();
            }
        }


        private void DrawFolder()
        {
            EditorGUILayout.BeginHorizontal();
            _guiContent.image = _icons[5];
            _guiContent.tooltip = "Change Selected Folder";
            if (GUILayout.Button(_guiContent, _buttonStyle, GUILayout.MaxWidth(25f), GUILayout.Height(20f)))
            {
                var path = EditorUtility.OpenFolderPanel("Select folder to set path.", _currentFolderPath, "");

                //remove Application.dataPath from path & replace \ with / for cross-platform compatibility
                path = path.Replace(Application.dataPath, "Assets").Replace("\\", "/");

                if (!AssetDatabase.IsValidFolder(path))
                    EditorUtility.DisplayDialog("Error: File Path Invalid",
                        "Make sure the path is a valid folder in the project.", "Ok");
                else
                {
                    _currentFolderPath = path;
                    SoapEditorUtils.WizardRootFolderPath = _currentFolderPath;
                    OnTabSelected(_currentType, true);
                }
            }

            _stringBuilder.Clear();
            _stringBuilder.Append(_currentFolderPath);
            _stringBuilder.Append("/");
            var displayedPath = _stringBuilder.ToString();
            EditorGUILayout.LabelField(displayedPath);
            
            //Settings button
            _guiContent.image = _icons[7];
            _guiContent.tooltip = "Open Settings";
            if (GUILayout.Button(_guiContent, _buttonStyle, GUILayout.MaxWidth(25), GUILayout.MaxHeight(20)))
            {
                SoapEditorUtils.WindowLastCategory = "Settings";
                SoapWindow.Open();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTags()
        {
            var height = EditorGUIUtility.singleLineHeight;
            EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(height));
            _guiContent.image = _icons[1];
            _guiContent.tooltip = "Edit Tags";
            if (GUILayout.Button(_guiContent, _buttonStyle, GUILayout.MaxWidth(25), GUILayout.MaxHeight(20)))
            {
                PopupWindow.Show(new Rect(), new TagPopUpWindow(position));
            }

            EditorGUILayout.LabelField("Tags", GUILayout.MaxWidth(70));
            _tagMask = EditorGUILayout.MaskField(_tagMask, _tags, GUILayout.MaxWidth(150));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTabs()
        {
            EditorGUILayout.BeginHorizontal();

            var defaultStyle = SoapInspectorUtils.Styles.ToolbarButton;
            var selectedStyle = new GUIStyle(defaultStyle);
            selectedStyle.normal.textColor = Color.white;

            for (int i = 0; i < _tabNames.Length; i++)
            {
                var isSelected = i == _typeTabIndex;

                var style = isSelected ? selectedStyle : defaultStyle;

                if (GUILayout.Button(_tabNames[i], style, GUILayout.Width(TabWidth)))
                {
                    _typeTabIndex = i;
                    OnTabSelected((ScriptableType)_typeTabIndex, true);
                }
            }

            EditorGUILayout.EndHorizontal();

            // Draw the bottom line
            var lastRect = GUILayoutUtility.GetLastRect();
            var width = lastRect.width / _tabNames.Length;
            var x = lastRect.x + _typeTabIndex * width;
            EditorGUI.DrawRect(new Rect(x, lastRect.yMax - 2, width, 2), Color.white);
        }

        private void DrawSearchBar()
        {
            GUILayout.BeginHorizontal();
            GUI.SetNextControlName("SearchBar");
            _searchText = GUILayout.TextField(_searchText, EditorStyles.toolbarSearchField);
            if (GUILayout.Button("", GUI.skin.FindStyle("SearchCancelButton")))
            {
                _searchText = "";
                GUI.FocusControl(null);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawScriptableBases(List<ScriptableBase> scriptables)
        {
            if (scriptables is null)
                return;

            EditorGUILayout.BeginVertical();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUIUtility.hierarchyMode = true;


            _indicesToDraw.Clear();
            // Reverse iteration to safely handle deletions
            var count = scriptables.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                if (scriptables[i] != null)
                    _indicesToDraw.Add(i);
            }

            count = _indicesToDraw.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                GUILayout.Space(2f);
                DrawScriptable(_indicesToDraw[i]);
            }

            EditorGUIUtility.hierarchyMode = false;

            // Handle deselection with a single check
            if (_currentEvent.type == EventType.MouseDown && _currentEvent.button == 0)
            {
                var totalRect = GUILayoutUtility.GetLastRect();
                if (!totalRect.Contains(_currentEvent.mousePosition))
                {
                    Deselect();
                    Repaint();
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            //The end

            void DrawScriptable(int i)
            {
                var scriptable = scriptables[i];
                if (scriptable == null)
                    return;

                //filter tags
                if ((_tagMask & (1 << scriptable.TagIndex)) == 0)
                    return;

                var entryName = GetNameFor(scriptable);
                //filter search
                if (entryName.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) < 0)
                    return;

                var rect = EditorGUILayout.GetControlRect();
                var selected = _selectedScriptableIndex == i;

                _bgRect.position = rect.position;
                _bgRect.height = rect.height + 2f;
                _bgRect.width = rect.width * 1.2f;
                _bgRect.x = rect.x - 10f;

                if (selected)
                    EditorGUI.DrawRect(_bgRect, _selectedColor);
                else if (rect.Contains(_currentEvent.mousePosition))
                    EditorGUI.DrawRect(_bgRect, _hoverColor);

                //Draw icon
                var icon = GetIconFor(scriptable);
                _guiContent.image = icon;
                _iconRect.position = rect.position;
                _iconRect.width = 18f;
                _iconRect.height = 18f;
                GUI.Box(_iconRect, _guiContent, _iconStyle);

                _entryNameRect.position = rect.position;
                _entryNameRect.width = rect.width * _widthRatio;
                _entryNameRect.height = rect.height;
                _entryNameRect.x = rect.x + 18f;

                // Handle right-click here
                if (_currentEvent.type == EventType.MouseDown && _currentEvent.button == 1 &&
                    rect.Contains(_currentEvent.mousePosition))
                {
                    ShowContextMenu(scriptable);
                    _currentEvent.Use();
                }

                // Add favorite icon
                _stringBuilder.Clear();
                if (_favoriteData.IsFavorite(scriptable))
                    _stringBuilder.Append("\u2605 ");
                _stringBuilder.Append(entryName);
                entryName = _stringBuilder.ToString();

                // Draw Label or button
                if (selected)
                {
                    GUI.Label(_entryNameRect, entryName, _entryStyle);
                    EditorGUILayout.BeginVertical(GUI.skin.box);
                    DrawEditor(scriptable);
                    EditorGUILayout.EndVertical();
                }
                else if (GUI.Button(_entryNameRect, entryName, EditorStyles.label)) // Select
                {
                    Deselect();
                    _selectedScriptableIndex = i;
                    _scriptableBase = scriptable;
                }

                // Draw Shortcut
                var shortcutRect = new Rect(rect)
                {
                    x = rect.x + rect.width * _widthRatio,
                    height = EditorGUIUtility.singleLineHeight,
                    width = rect.width * (1 - _widthRatio)
                };
                DrawShortcut(shortcutRect, scriptable);
            }

            string GetNameFor(ScriptableBase scriptableBase)
            {
                _stringBuilder.Clear();
                if (_subAssetsLookup != null && _subAssetsLookup.TryGetValue(scriptableBase, out var mainAsset))
                {
                    _stringBuilder.Append("[");
                    _stringBuilder.Append(mainAsset.name);
                    _stringBuilder.Append("] ");
                    _stringBuilder.Append(scriptableBase.name);
                    return _stringBuilder.ToString();
                }

                _stringBuilder.Append(scriptableBase.name);
                return _stringBuilder.ToString();
            }

            void ShowContextMenu(ScriptableBase scriptableBase)
            {
                var menu = new GenericMenu();
                var isFavorite = _favoriteData.IsFavorite(scriptableBase);
                var favoriteText = isFavorite ? "Remove from favorite" : "Add to favorite";
                var favoriteIcon = isFavorite ? "\u2730 " : "\u2605 ";
                menu.AddItem(new GUIContent(favoriteIcon + favoriteText), false, () =>
                {
                    if (isFavorite)
                        _favoriteData.RemoveFavorite(scriptableBase);
                    else
                        _favoriteData.AddFavorite(scriptableBase);
                });
                menu.AddItem(new GUIContent("\ud83c\udfaf Ping"), false, () =>
                {
                    Selection.activeObject = scriptableBase;
                    EditorGUIUtility.PingObject(scriptableBase);
                });
                menu.AddItem(new GUIContent("\u270f\ufe0f Rename"), false,
                    () => { PopupWindow.Show(new Rect(), new RenamePopUpWindow(position, scriptableBase)); });

                if (AssetDatabase.IsMainAsset(scriptableBase))
                {
                    menu.AddItem(new GUIContent("\ud83d\udcc4 Duplicate"), false, () =>
                    {
                        SoapEditorUtils.CreateCopy(scriptableBase);
                        Refresh(_currentType);
                    });
                }

                menu.AddItem(new GUIContent("\ud83d\udd0d Find References/In Scene and Project"), false,
                    () => { ReferencesPopupWindow.ShowWindow(position, scriptableBase, FindReferenceType.All); });
                menu.AddItem(new GUIContent("\ud83d\udd0d Find References/In Scene"), false,
                    () => { ReferencesPopupWindow.ShowWindow(position, scriptableBase, FindReferenceType.Scene); });
                menu.AddItem(new GUIContent("\ud83d\udd0d Find References/In Project"), false,
                    () => { ReferencesPopupWindow.ShowWindow(position, scriptableBase, FindReferenceType.Project); });

                menu.AddItem(new GUIContent("\ud83d\udd16 Set Tag"), false,
                    () =>
                    {
                        var scriptableBases = new[] { scriptableBase };
                        PopupWindow.Show(new Rect(), new SetTagPopupWindow(scriptableBases));
                    });

                menu.AddItem(new GUIContent("\u274c Delete"), false, () =>
                {
                    var isDeleted = SoapEditorUtils.DeleteObjectWithConfirmation(scriptableBase);
                    if (isDeleted)
                    {
                        _scriptableBase = null;
                        OnTabSelected(_currentType, true);
                    }
                });
                menu.ShowAsContext();
            }
        }

        private void DrawShortcut(Rect rect, ScriptableBase scriptable)
        {
            if (!_serializedObjects.TryGetValue(scriptable, out var serializedObject))
            {
                serializedObject = new SerializedObject(scriptable);
                _serializedObjects[scriptable] = serializedObject;
            }

            if (serializedObject == null) //could be destroyed
                return;

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            if (scriptable is ScriptableVariableBase scriptableVariableBase)
                DrawVariableValue(scriptableVariableBase);
            else if (scriptable is ScriptableEventNoParam scriptableEventNoParam)
                DrawEventNoParam(scriptableEventNoParam);
            else if (scriptable is ScriptableCollection scriptableCollection)
                DrawCollection(scriptableCollection);
            else if (scriptable is ScriptableEventBase scriptableEventBase)
                DrawEventWithParam(scriptableEventBase);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            void DrawVariableValue(ScriptableVariableBase variableBase)
            {
                var valuePropertyDrawer = new ScriptableVariablePropertyDrawer(serializedObject, variableBase);
                valuePropertyDrawer.DrawShortcut(rect);
            }

            void DrawEventNoParam(ScriptableEventNoParam scriptableEventNoParam)
            {
                var propertyDrawer = new ScriptableEventNoParamPropertyDrawer();
                propertyDrawer.DrawShortcut(rect, scriptableEventNoParam);
            }

            void DrawEventWithParam(ScriptableEventBase scriptableEventBase)
            {
                var propertyDrawer = new ScriptableEventPropertyDrawer(serializedObject, scriptableEventBase);
                propertyDrawer.DrawShortcut(rect);
            }

            void DrawCollection(ScriptableCollection scriptableCollection)
            {
                var propertyDrawer = new ScriptableCollectionPropertyDrawer(serializedObject, scriptableCollection);
                propertyDrawer.DrawShortcut(rect);
            }
        }

        private void DrawEditor(ScriptableBase scriptableBase)
        {
            EditorGUILayout.BeginVertical();
            if (_editor == null)
                UnityEditor.Editor.CreateCachedEditor(scriptableBase, null, ref _editor);
            if (scriptableBase == null)
            {
                DestroyImmediate(_editor);
                return;
            }

            _editor.OnInspectorGUI();
            EditorGUILayout.EndVertical();
        }

        private void DrawUtilityButtons()
        {
            EditorGUILayout.BeginHorizontal();
            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.padding = new RectOffset(0, 0, 3, 3);
            var lessPaddingStyle = new GUIStyle(buttonStyle);
            lessPaddingStyle.padding = new RectOffset(0, 0, 1, 1);

            var buttonHeight = 20;

            var isFavorite = _favoriteData.IsFavorite(_scriptableBase);
            var icon = isFavorite ? _icons[4] : _icons[0];
            var tooltip = isFavorite ? "Remove from favorite" : "Add to favorite";
            var buttonContent = new GUIContent("Favorite", icon, tooltip);
            if (GUILayout.Button(buttonContent, lessPaddingStyle, GUILayout.MaxHeight(buttonHeight)))
            {
                if (isFavorite)
                    _favoriteData.RemoveFavorite(_scriptableBase);
                else
                    _favoriteData.AddFavorite(_scriptableBase);
            }

            buttonContent = new GUIContent("Ping", _icons[6], "Pings the asset in the project");
            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.MaxHeight(buttonHeight)))
            {
                Selection.activeObject = _scriptableBase;
                EditorGUIUtility.PingObject(_scriptableBase);
            }

            buttonContent = new GUIContent("Rename", _icons[1]);
            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.MaxHeight(buttonHeight)))
                PopupWindow.Show(new Rect(), new RenamePopUpWindow(position, _scriptableBase));

            EditorGUI.BeginDisabledGroup(!AssetDatabase.IsMainAsset(_scriptableBase));
            buttonContent = new GUIContent("Duplicate", _icons[2], "Create Copy");
            if (GUILayout.Button(buttonContent, lessPaddingStyle, GUILayout.MaxHeight(buttonHeight)))
            {
                SoapEditorUtils.CreateCopy(_scriptableBase);
                Refresh(_currentType);
            }

            EditorGUI.EndDisabledGroup();

            buttonContent = new GUIContent("Delete", _icons[3]);
            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.MaxHeight(buttonHeight)))
            {
                var isDeleted = SoapEditorUtils.DeleteObjectWithConfirmation(_scriptableBase);
                if (isDeleted)
                {
                    _scriptableBase = null;
                    OnTabSelected(_currentType, true);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawBottomButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create New Type", GUILayout.Height(25f)))
            {
                SoapTypeCreatorWindow.Show();
            }

            if (GUILayout.Button("Create New Asset", GUILayout.Height(25f)))
            {
                PopupWindow.Show(new Rect(),
                    new SoapAssetCreatorPopup(position, 
                        SoapAssetCreatorPopup.EOrigin.SoapWizard));
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OnTabSelected(ScriptableType type, bool deselectCurrent = false)
        {
            Refresh(type);
            _currentType = type;
            if (deselectCurrent)
            {
                Deselect();
            }
        }

        private void Deselect()
        {
            _scriptableBase = null;
            _selectedScriptableIndex = -1;
            GUIUtility.keyboardControl = 0; //remove focus
            DestroyImmediate(_editor);
        }

        private void Refresh(ScriptableType type)
        {
            switch (type)
            {
                case ScriptableType.All:
                    _scriptableObjects =
                        SoapEditorUtils.FindAll<ScriptableBase>(_currentFolderPath, out _subAssetsLookup);
                    break;
                case ScriptableType.Variables:
                    var variables =
                        SoapEditorUtils.FindAll<ScriptableVariableBase>(_currentFolderPath, out _subAssetsLookup);
                    _scriptableObjects = variables.Cast<ScriptableBase>().ToList();
                    break;
                case ScriptableType.Events:
                    var events = SoapEditorUtils.FindAll<ScriptableEventBase>(_currentFolderPath, out _subAssetsLookup);
                    _scriptableObjects = events.Cast<ScriptableBase>().ToList();
                    break;
                case ScriptableType.Collections:
                    var lists = SoapEditorUtils.FindAll<ScriptableCollection>(_currentFolderPath, out _subAssetsLookup);
                    _scriptableObjects = lists.Cast<ScriptableBase>().ToList();
                    break;
                case ScriptableType.Enums:
                    var enums = SoapEditorUtils.FindAll<ScriptableEnumBase>(_currentFolderPath, out _subAssetsLookup);
                    _scriptableObjects = enums.Cast<ScriptableBase>().ToList();
                    break;
                case ScriptableType.Favorites:
                    SoapEditorUtils.FindAll<ScriptableBase>(_currentFolderPath, out _subAssetsLookup);
                    var favorites = _favoriteData.GetFavorites();
                    foreach (var kvp in _subAssetsLookup)
                    {
                        if (_favoriteData.IsFavorite(kvp.Key))
                        {
                            favorites.Add(kvp.Key);
                        }
                    }

                    _scriptableObjects = favorites;
                    break;
            }
        }

        private void SelectTab(int index, bool deselect = false)
        {
            _typeTabIndex = index;
            OnTabSelected((ScriptableType)_typeTabIndex, deselect);
        }

        private Texture GetIconFor(ScriptableBase scriptableBase)
        {
            var texture = SoapInspectorUtils.Icons.GetIconFor(scriptableBase.GetType());
            return texture ?? _icons[0];
        }
    
        #region Repaint

        private void OnPlayModeStateChanged(PlayModeStateChange pm)
        {
            if (pm == PlayModeStateChange.EnteredPlayMode)
            {
                foreach (var scriptableBase in _scriptableObjects)
                {
                    if (scriptableBase != null)
                        scriptableBase.RepaintRequest += OnRepaintRequested;
                }
            }
            else if (pm == PlayModeStateChange.EnteredEditMode)
            {
                foreach (var scriptableBase in _scriptableObjects)
                {
                    if (scriptableBase != null)
                        scriptableBase.RepaintRequest -= OnRepaintRequested;
                }
            }
        }

        private void OnRepaintRequested()
        {
            //Debug.Log("Repaint Wizard " + _scriptableBase.name);
            Repaint();
        }

        #endregion
    }
}