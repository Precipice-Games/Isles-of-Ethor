using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public class SoapDictionaryCreatorPopup : PopupWindowContent
    {
        private string _dictionaryName = "ScriptableDictionaryKeyValue";
        private string _keyText = "Key";
        private string _valueText = "Value";
        private string _namespaceText = "";
        private bool _keyClass;
        private bool _valueClass;
        private bool _isKeyNameInvalid;
        private bool _isValueNameInvalid;
        private bool _isDictionaryNameInvalid;
        private bool _isNamespaceInvalid;
        private string _path;
        private Texture[] _icons;
        private int _destinationFolderIndex;
        private readonly Color _validTypeColor = new Color(0.32f, 0.96f, 0.8f);
        private Rect _rect;
        private string ValidColorHtml => "#51ffcc";
        private string InvalidColorHtml => "#ff0000";

        public override Vector2 GetWindowSize()
        {
            var dimension = new Vector2(_rect.width, _rect.height);
            dimension.x = Mathf.Max(_rect.width, 400f);
            return dimension;
        }

        private float Width => GetWindowSize().x;

        public SoapDictionaryCreatorPopup(Rect rect)
        {
            _rect = rect;
            _icons = new Texture[2];
            _icons[0] = EditorGUIUtility.IconContent("cs Script Icon").image;
            _icons[1] = EditorGUIUtility.IconContent("Error").image;
            _destinationFolderIndex = SoapEditorUtils.TypeCreatorDestinationFolderIndex;
            _path = _destinationFolderIndex == 0
                ? SoapFileUtils.GetSelectedFolderPathInProjectWindow()
                : SoapEditorUtils.TypeCreatorDestinationFolderPath;
        }

        public override void OnGUI(Rect rect)
        {
            var center = SoapInspectorUtils.CenterInWindow(editorWindow.position, _rect);
            center.y += 20f;
            editorWindow.position = center;
            SoapInspectorUtils.DrawPopUpHeader(editorWindow, "Create Scriptable Dictionary Type");
            GUILayout.BeginVertical(SoapInspectorUtils.Styles.PopupContent);
            DrawNamespace();
            GUILayout.Space(2);
            DrawName();
            GUILayout.Space(2);
            DrawKeyAndValueTextFields();
            GUILayout.Space(2);
            DrawKeyAndValueToggles();
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            SoapInspectorUtils.DrawLine();
            GUILayout.Space(2);
            DrawAssetPreview();
            GUILayout.FlexibleSpace();
            DrawPath();
            GUILayout.Space(5);
            DrawCreateButton();
            GUILayout.EndVertical();
        }

        private void DrawNamespace()
        {
            EditorGUILayout.BeginHorizontal();
            Texture2D texture = new Texture2D(0, 0);
            var icon = _isNamespaceInvalid ? _icons[1] : texture;
            var style = new GUIStyle(GUIStyle.none);
            style.margin = new RectOffset(10, 0, 5, 0);
            GUILayout.Box(icon, style, GUILayout.Width(18), GUILayout.Height(18));
            var labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f, 1f);
            EditorGUILayout.LabelField("Namespace:", labelStyle, GUILayout.Width(75));
            EditorGUI.BeginChangeCheck();
            var textStyle = new GUIStyle(GUI.skin.textField);
            textStyle.focused.textColor = _isNamespaceInvalid ? Color.red : Color.white;
            _namespaceText = EditorGUILayout.TextField(_namespaceText, textStyle);
            if (EditorGUI.EndChangeCheck())
            {
                _isNamespaceInvalid = !SoapEditorUtils.IsNamespaceValid(_namespaceText);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawName()
        {
            EditorGUILayout.BeginHorizontal();
            Texture2D texture = new Texture2D(0, 0);
            var icon = _isDictionaryNameInvalid ? _icons[1] : texture;
            var style = new GUIStyle(GUIStyle.none);
            style.margin = new RectOffset(10, 0, 5, 0);
            GUILayout.Box(icon, style, GUILayout.Width(18), GUILayout.Height(18));
            EditorGUILayout.LabelField("Name:", GUILayout.Width(75));
            EditorGUI.BeginChangeCheck();
            var textStyle = new GUIStyle(GUI.skin.textField);
            textStyle.focused.textColor = _isDictionaryNameInvalid ? Color.red : Color.white;
            _dictionaryName = EditorGUILayout.TextField(_dictionaryName, textStyle);
            if (EditorGUI.EndChangeCheck())
            {
                _isDictionaryNameInvalid = !SoapEditorUtils.IsTypeNameValid(_dictionaryName);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawKeyAndValueTextFields()
        {
            EditorGUILayout.BeginHorizontal();
            //Draw Key Text Field
            {
                Texture2D texture = new Texture2D(0, 0);
                var icon = _isKeyNameInvalid ? _icons[1] : texture;
                var style = new GUIStyle(GUIStyle.none);
                style.margin = new RectOffset(10, 0, 5, 0);
                GUILayout.Box(icon, style, GUILayout.Width(18), GUILayout.Height(18));
                EditorGUILayout.LabelField("Key:", GUILayout.Width(75));
                EditorGUI.BeginChangeCheck();
                var textStyle = new GUIStyle(GUI.skin.textField);
                textStyle.focused.textColor = _isKeyNameInvalid ? Color.red : Color.white;
                _keyText = EditorGUILayout.TextField(_keyText, textStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    _isKeyNameInvalid = !SoapEditorUtils.IsTypeNameValid(_keyText);
                }
            }
            GUILayout.Space(2);

            //Draw Value Text Field
            {
                Texture2D texture = new Texture2D(0, 0);
                var icon = _isValueNameInvalid ? _icons[1] : texture;
                var style = new GUIStyle(GUIStyle.none);
                style.margin = new RectOffset(10, 0, 5, 0);
                GUILayout.Box(icon, style, GUILayout.Width(18), GUILayout.Height(18));
                EditorGUILayout.LabelField("Value:", GUILayout.Width(50));
                EditorGUI.BeginChangeCheck();
                var textStyle = new GUIStyle(GUI.skin.textField);
                textStyle.focused.textColor = _isValueNameInvalid ? Color.red : Color.white;
                _valueText = EditorGUILayout.TextField(_valueText, textStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    _isValueNameInvalid = !SoapEditorUtils.IsTypeNameValid(_valueText);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawKeyAndValueToggles()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Create Classes?", GUILayout.Width(96));

            if (SoapUtils.CanBeCreated(_keyText))
            {
                var capitalizedType = $"{_keyText.CapitalizeFirstLetter()}";
                DrawToggle(ref _keyClass, $"{capitalizedType}", _isKeyNameInvalid);
            }
            else
            {
                GUILayout.Space(Width / 3 + 18f + 6);
            }

            GUILayout.Space(Width / 13);
            GUILayout.FlexibleSpace();
            if (SoapUtils.CanBeCreated(_valueText))
            {
                var vcapitalizedType = $"{_valueText.CapitalizeFirstLetter()}";
                DrawToggle(ref _valueClass, $"{vcapitalizedType}", _isValueNameInvalid);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawToggle(ref bool toggleValue, string typeName, bool isValid)
        {
            EditorGUILayout.BeginHorizontal();
            var icon = _icons[0];
            var style = new GUIStyle(GUIStyle.none);
            GUILayout.Box(icon, style, GUILayout.Width(18), GUILayout.Height(18));
            GUIStyle firstStyle = new GUIStyle(GUI.skin.toggle);
            var width = Width / 3f;
            toggleValue = GUILayout.Toggle(toggleValue, typeName, firstStyle, GUILayout.Width(width));
            firstStyle.normal.textColor = isValid ? SoapEditorUtils.SoapColor : _validTypeColor;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAssetPreview()
        {
            EditorGUILayout.BeginHorizontal();
            var style = new GUIStyle(GUIStyle.none);
            GUILayout.Box(SoapInspectorUtils.Icons.ScriptableDictionary, style, GUILayout.Width(18), GUILayout.Height(18));
            GUIStyle firstStyle = new GUIStyle(GUI.skin.label);
            firstStyle.normal.textColor = _isDictionaryNameInvalid ? Color.red : Color.white;
            GUILayout.Label(_dictionaryName, firstStyle);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(22f);
            var keyColor = _isKeyNameInvalid ? InvalidColorHtml : ValidColorHtml;
            var valueColor = _isValueNameInvalid ? InvalidColorHtml : ValidColorHtml;
            var types = $"<color={keyColor}>{_keyText}</color> , <color={valueColor}>{_valueText}</color>";
            var classText = $"ScriptableDictionary<{types}>";
            GUIStyle secondStyle = new GUIStyle(GUI.skin.label);
            secondStyle.richText = true;
            secondStyle.fontSize = 11;
            GUILayout.Label(classText, secondStyle);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPath()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Destination Folder:");
            var style = new GUIStyle(EditorStyles.popup);
            var options = new[]
            {
                "Selected in Project",
                "Custom"
            };

            if (GUILayout.Button(options[_destinationFolderIndex], style))
            {
                SoapInspectorUtils.ShowPathMenu(options, _destinationFolderIndex,
                    newTag =>
                    {
                        _destinationFolderIndex = newTag;
                        SoapEditorUtils.TypeCreatorDestinationFolderIndex = newTag;
                    });
            }

            EditorGUILayout.EndHorizontal();

            if (_destinationFolderIndex == 0)
            {
                GUI.enabled = false;
                _path = SoapFileUtils.GetSelectedFolderPathInProjectWindow();
                EditorGUILayout.TextField($"{_path}");
                GUI.enabled = true;
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                _path = EditorGUILayout.TextField(SoapEditorUtils.TypeCreatorDestinationFolderPath);
                if (EditorGUI.EndChangeCheck())
                {
                    SoapEditorUtils.TypeCreatorDestinationFolderPath = _path;
                }
            }
        }


        private void DrawCreateButton()
        {
            GUI.enabled = !_isKeyNameInvalid && !_isNamespaceInvalid && !_isDictionaryNameInvalid &&
                          !_isValueNameInvalid;
            if (SoapInspectorUtils.DrawCallToActionButton("Create", SoapInspectorUtils.ButtonSize.Medium))
            {
                var newFilePaths = new List<string>();
                TextAsset newFile = null;
                var progress = 0f;
                EditorUtility.DisplayProgressBar("Progress", "Start", progress);

                if (_keyClass)
                {
                    var templateName = "NewTypeTemplate.cs";
                    if (!SoapEditorUtils.CreateClassFromTemplate(templateName, _namespaceText, _keyText, _path,
                            out newFile))
                    {
                        CloseWindow();
                        return;
                    }
                }

                progress += 0.33f;
                EditorUtility.DisplayProgressBar("Progress", "Generating...", progress);

                if (_valueClass)
                {
                    if (!SoapEditorUtils.CreateClassFromTemplate("NewTypeTemplate.cs", _namespaceText,
                            _valueText, _path,
                            out newFile))
                    {
                        CloseWindow();
                        return;
                    }

                    newFilePaths.Add(AssetDatabase.GetAssetPath(newFile));
                }

                progress += 0.33f;
                EditorUtility.DisplayProgressBar("Progress", "Generating...", progress);

                if (!SoapEditorUtils.CreateDictionaryFromTemplate("ScriptableDictionaryTemplate.cs", _namespaceText,
                        _dictionaryName, _keyText, _valueText, _path, out newFile))
                {
                    CloseWindow();
                    return;
                }

                newFilePaths.Add(AssetDatabase.GetAssetPath(newFile));

                progress += 0.34f;
                EditorPrefs.SetString("Soap_NewFilePaths", string.Join(";", newFilePaths));
                EditorUtility.DisplayProgressBar("Progress", "Completed!", progress);
                EditorUtility.DisplayDialog("Success", $"{_dictionaryName} was created!", "OK");
                CloseWindow(false);
                AssetDatabase.SaveAssets();
                EditorGUIUtility.PingObject(newFile);
            }

            GUI.enabled = true;
        }
        
        private void CloseWindow(bool hasError = true)
        {
            EditorUtility.ClearProgressBar();
            editorWindow.Close();
            if (hasError)
                EditorUtility.DisplayDialog("Error", $"Failed to create {_dictionaryName}", "OK");
        }
    }
}