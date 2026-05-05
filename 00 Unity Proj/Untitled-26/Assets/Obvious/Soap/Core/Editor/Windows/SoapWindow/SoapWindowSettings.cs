using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public class SoapWindowSettings
    {
        private readonly Texture[] _icons;
        private Vector2 _scrollPosition = Vector2.zero;
        private SoapSettings _settings;
        private readonly float _defaultLabelWidth = EditorGUIUtility.labelWidth;
        private EditorWindow _editorWindow;
        private bool _isTagsFoldoutOpen;
        private bool _isPrefixFoldoutOpen;
        
        private static Dictionary<int,string> _typeNames = new Dictionary<int, string>()
        {
            {0, "ScriptableVariable"},
            {1, "ScriptableEvent"},
            {2, "ScriptableList"},
            {3, "ScriptableDictionary"},
            {4, "ScriptableEnum"},
            {5, "ScriptableSave"}
        };

        public SoapWindowSettings(EditorWindow editorWindow)
        {
            _icons = new Texture[1];
            _icons[0] = EditorGUIUtility.IconContent("cs Script Icon").image;
            _editorWindow = editorWindow;
        }

        public void Draw()
        {
            EditorGUI.BeginChangeCheck();
            //Fixes weird bug with the label width
            EditorGUIUtility.labelWidth = _defaultLabelWidth;
            if (_settings == null)
                _settings = SoapEditorUtils.GetOrCreateSoapSettings();
            EditorGUILayout.BeginVertical();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawVariableDisplayMode();
            GUILayout.Space(10);
            DrawNamingModeOnCreation();
            GUILayout.Space(10);
            DrawAllowRaisingEventsInEditor();
            DrawTags();
            DrawPrefixes();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_settings);
                AssetDatabase.SaveAssets();
            }
        }

        private void DrawVariableDisplayMode()
        {
#if ODIN_INSPECTOR
            GUI.enabled = false;
#endif
            EditorGUILayout.BeginHorizontal();

            _settings.VariableDisplayMode =
                (EVariableDisplayMode)EditorGUILayout.EnumPopup("Variable display mode",
                    _settings.VariableDisplayMode, GUILayout.Width(225));

            var infoText = _settings.VariableDisplayMode == EVariableDisplayMode.Default
                ? "Displays all the parameters of variables."
                : "Only displays the value.";
            EditorGUILayout.LabelField(infoText, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndHorizontal();

#if ODIN_INSPECTOR
            GUI.enabled = true;
            EditorGUILayout.HelpBox("The variable display mode cannot be changed when using Odin Inspector.",
                MessageType.Warning);
#endif
        }

        private void DrawNamingModeOnCreation()
        {
            EditorGUILayout.BeginVertical();

            //Draw Naming Mode On Creation
            EditorGUILayout.BeginHorizontal();
            _settings.NamingOnCreationMode =
                (ENamingCreationMode)EditorGUILayout.EnumPopup("Creation Mode",
                    _settings.NamingOnCreationMode, GUILayout.Width(225));

            var namingInfoText = _settings.NamingOnCreationMode == ENamingCreationMode.Auto
                ? "Automatically create the asset and assign it a name."
                : "Opens the Soap Asset Creator Popup";
            EditorGUILayout.LabelField(namingInfoText, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndHorizontal();

            //Draw Create Path Mode
            EditorGUILayout.BeginHorizontal();
            _settings.CreatePathMode =
                (ECreatePathMode)EditorGUILayout.EnumPopup("Create Path Mode",
                    _settings.CreatePathMode, GUILayout.Width(225));

            var pathInfoText = _settings.CreatePathMode == ECreatePathMode.Auto
                ? "Creates the asset in the selected path of the project window."
                : "Creates the asset at a custom path.";
            EditorGUILayout.LabelField(pathInfoText, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndHorizontal();

            //Draw Path
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 40;
            if (_settings.CreatePathMode == ECreatePathMode.Auto)
            {
                var guiStyle = new GUIStyle(EditorStyles.label);
                guiStyle.fontStyle = FontStyle.Italic;
                var path = SoapFileUtils.GetSelectedFolderPathInProjectWindow();
                EditorGUILayout.LabelField("Path:", $"{path}", guiStyle);
            }
            else
            {
                var path = EditorGUILayout.TextField("Path:", SoapEditorUtils.CustomCreationPath);
                SoapEditorUtils.CustomCreationPath = path;
            }

            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUILayout.EndVertical();

            // //Example
            // {
            //     EditorGUILayout.BeginVertical(_skin.box);
            //     _exampleClassSerializedObject?.Update();
            //     EditorGUILayout.BeginHorizontal();
            //     var guiStyle = new GUIStyle(GUIStyle.none);
            //     guiStyle.contentOffset = new Vector2(0, 2);
            //     GUILayout.Box(_icons[0], guiStyle, GUILayout.Width(18), GUILayout.Height(18));
            //     GUILayout.Space(16);
            //     EditorGUILayout.LabelField("Example Class (Script)", EditorStyles.boldLabel);
            //     EditorGUILayout.EndHorizontal();
            //     GUILayout.Space(2);
            //     SoapInspectorUtils.DrawColoredLine(1, new Color(0f, 0f, 0f, 0.2f));
            //     EditorGUILayout.PropertyField(_currentHealthProperty);
            //     _exampleClassSerializedObject?.ApplyModifiedProperties();
            //     EditorGUILayout.EndVertical();
            // }
        }

        private void DrawAllowRaisingEventsInEditor()
        {
            EditorGUILayout.BeginHorizontal();
            _settings.RaiseEventsInEditor = (ERaiseEventInEditorMode)EditorGUILayout.EnumPopup("Raise events in editor",
                _settings.RaiseEventsInEditor, GUILayout.Width(225));
            var pathInfoText = _settings.RaiseEventsInEditor == ERaiseEventInEditorMode.Disabled
                ? "Prevent raising events in editor mode."
                : "Allow raising events in editor mode.";
            EditorGUILayout.LabelField(pathInfoText, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTags()
        {
            _isTagsFoldoutOpen = EditorGUILayout.Foldout(_isTagsFoldoutOpen, "Tags", true);
            if (_isTagsFoldoutOpen)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < _settings.Tags.Count; i++)
                {
                    //indent

                    GUI.enabled = i != 0;
                    _settings.Tags[i] = EditorGUILayout.TextField(_settings.Tags[i]);
                    GUI.enabled = true;
                }

                if (GUILayout.Button("Edit Tags"))
                {
                    if (_editorWindow == null)
                    {
                        Type preferences = Type.GetType("UnityEditor.PreferenceSettingsWindow,UnityEditor");
                        _editorWindow = EditorWindow.GetWindow(preferences);
                    }
                    PopupWindow.Show(new Rect(), new TagPopUpWindow(_editorWindow.position));
                }

                EditorGUI.indentLevel--;
            }
        }

        private void DrawPrefixes()
        {
            _isPrefixFoldoutOpen = EditorGUILayout.Foldout(_isPrefixFoldoutOpen, "Prefixes", true);
            if (_isPrefixFoldoutOpen)
            {
                EditorGUI.indentLevel++;

                for (int i = 0; i < _settings.Prefixes.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(_typeNames[i]);
                    var currentPrefix = _settings.Prefixes[i];
                    string newValue = EditorGUILayout.TextField(currentPrefix);
                    if (newValue != currentPrefix)
                    {
                        _settings.SetPrefix(i, newValue);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}