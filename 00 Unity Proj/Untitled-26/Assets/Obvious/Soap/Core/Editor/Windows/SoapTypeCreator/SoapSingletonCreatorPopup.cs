using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public class SoapSingletonCreatorPopup : PopupWindowContent
    {
        private string _className = "NewClass";
        private string _namespaceText = "";
        private bool _isClassNameInvalid;
        private bool _isNamespaceInvalid;
        private bool _isResourcePathValid = true;
        private bool _autoCreateInstance = true;
        private string _path;
        private string _resourcePath = "Assets/Resources";
        private Texture[] _icons;
        private int _destinationFolderIndex;
        private Rect _rect;
        private string ValidColorHtml => "#51ffcc";
        private string InvalidColorHtml => "#ff0000";
        private Action<MonoScript> _onClassCreated;
        
        public override Vector2 GetWindowSize()
        {
            var dimension = new Vector2(_rect.width, _rect.height);
            dimension.x = Mathf.Max(_rect.width, 400f);
            return dimension;
        }

        public SoapSingletonCreatorPopup(Rect rect)
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
            SoapInspectorUtils.DrawPopUpHeader(editorWindow, "Create Scriptable Singleton Type");
            GUILayout.BeginVertical(SoapInspectorUtils.Styles.PopupContent);
            DrawNamespace();
            GUILayout.Space(2);
            DrawName();
            GUILayout.Space(10);
            DrawAutoInstanceCreationToggle();
            GUILayout.Space(5f);
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
            var icon = _isClassNameInvalid ? _icons[1] : texture;
            var style = new GUIStyle(GUIStyle.none);
            style.margin = new RectOffset(10, 0, 5, 0);
            GUILayout.Box(icon, style, GUILayout.Width(18), GUILayout.Height(18));
            EditorGUILayout.LabelField("Type Name:", GUILayout.Width(75));
            EditorGUI.BeginChangeCheck();
            var textStyle = new GUIStyle(GUI.skin.textField);
            textStyle.focused.textColor = _isClassNameInvalid ? Color.red : Color.white;
            _className = EditorGUILayout.TextField(_className, textStyle);
            if (EditorGUI.EndChangeCheck())
            {
                _isClassNameInvalid = !SoapEditorUtils.IsTypeNameValid(_className)
                                      || !SoapUtils.CanBeCreated(_className);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawAutoInstanceCreationToggle()
        {
            GUIStyle firstStyle = new GUIStyle(GUI.skin.toggle);
            var toggleLabel = "Create Instance at Resource Path";
            _autoCreateInstance =
                GUILayout.Toggle(_autoCreateInstance, toggleLabel, firstStyle);
            if (_autoCreateInstance)
            {
                GUILayout.Space(2f);
                DrawResourcePath();
            }
        }

        private void DrawResourcePath()
        {
            EditorGUILayout.BeginHorizontal();
            Texture2D texture = new Texture2D(0, 0);
            var icon = _isResourcePathValid ? texture : _icons[1];
            var style = new GUIStyle(GUIStyle.none);
            style.margin = new RectOffset(10, 0, 5, 0);
            GUILayout.Box(icon, style, GUILayout.Width(18), GUILayout.Height(18));
            var labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f, 1f);
            EditorGUILayout.LabelField("Resources Path:", labelStyle, GUILayout.Width(95));
            EditorGUI.BeginChangeCheck();
            var textStyle = new GUIStyle(GUI.skin.textField);
            textStyle.focused.textColor = _isResourcePathValid ? Color.white : Color.red;
            _resourcePath = EditorGUILayout.TextField(_resourcePath, textStyle);
            if (EditorGUI.EndChangeCheck())
            {
                _isResourcePathValid = SoapEditorUtils.IsResourcePathValid(_resourcePath);
            }

            EditorGUILayout.EndHorizontal();

            if (!_isResourcePathValid)
            {
                var helpStyle = new GUIStyle(EditorStyles.wordWrappedMiniLabel);
                helpStyle.wordWrap = true;
                EditorGUILayout.LabelField(
                    "Invalid resource path. It should contain 'Resources' and be within the Assets folder.",
                    helpStyle);
            }
        }

        private void DrawAssetPreview()
        {
            EditorGUILayout.BeginHorizontal();
            var style = new GUIStyle(GUIStyle.none);
            GUILayout.Box(SoapInspectorUtils.Icons.ScriptableSingleton, style, GUILayout.Width(18),
                GUILayout.Height(18));
            GUIStyle firstStyle = new GUIStyle(GUI.skin.label);
            firstStyle.normal.textColor = _isClassNameInvalid ? Color.red : Color.white;
            GUILayout.Label(_className, firstStyle);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(22f);
            var color = _isClassNameInvalid ? InvalidColorHtml : ValidColorHtml;
            var typeLabel = $"<color={color}>{_className.CapitalizeFirstLetter()}</color>";
            var classText = $"ScriptableSingleton<{typeLabel}>";
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
            GUI.enabled = !_isNamespaceInvalid && !_isClassNameInvalid && SoapUtils.CanBeCreated(_className);
            if (SoapInspectorUtils.DrawCallToActionButton("Create", SoapInspectorUtils.ButtonSize.Medium))
            {
                var newFilePaths = new List<string>();
                TextAsset newFile = null;
                var progress = 0f;
                EditorUtility.DisplayProgressBar("Progress", "Start", progress);

                progress += 0.33f;
                EditorUtility.DisplayProgressBar("Progress", "Generating...", progress);
                if (!SoapEditorUtils.CreateClassFromTemplate("ScriptableSingletonTemplate.cs", _namespaceText,
                        _className, _path, out newFile, false, true))
                {
                    CloseWindow();
                    return;
                }

                newFilePaths.Add(AssetDatabase.GetAssetPath(newFile));
                progress += 0.64f;
                EditorUtility.DisplayProgressBar("Progress", "Generating...", progress);

                StoreSingletonInfoToSessionState();
                EditorPrefs.SetString("Soap_NewFilePaths", string.Join(";", newFilePaths));
                EditorUtility.DisplayProgressBar("Progress", "Completed!", progress);
                EditorUtility.DisplayDialog("Success", $"{_className} was created!", "OK");
                CloseWindow(false);
                EditorGUIUtility.PingObject(newFile);
            }

            GUI.enabled = true;
        }

        private void StoreSingletonInfoToSessionState()
        {
            if (!_autoCreateInstance)
            {
                SoapEditorUtils.SingletonInstanceInfo = string.Empty;
                return;
            }

            var singletonInfo = new SingletonInstanceInfo
            {
                ClassName = _className,
                ResourcesPath = _resourcePath
            };

            var json = JsonUtility.ToJson(singletonInfo);
            SoapEditorUtils.SingletonInstanceInfo = json;
        }

        private void CloseWindow(bool hasError = true)
        {
            EditorUtility.ClearProgressBar();
            editorWindow.Close();
            if (hasError)
                EditorUtility.DisplayDialog("Error", $"Failed to create {_className}", "OK");
        }
    }

    [Serializable]
    public struct SingletonInstanceInfo
    {
        public string ClassName;
        public string ResourcesPath;
    }
}