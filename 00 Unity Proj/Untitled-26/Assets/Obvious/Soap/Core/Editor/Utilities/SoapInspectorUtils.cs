using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Obvious.Soap.Editor
{
    public static class SoapInspectorUtils
    {
        /// <summary>
        /// Draws all properties like base.OnInspectorGUI() but excludes the specified fields by name.
        /// </summary>
        /// <param name="fieldsToSkip">An array of names that should be excluded.</param>
        /// <example>Example: new string[] { "m_Script" , "myInt" } will skip the default Script field and the Integer field myInt.
        /// </example>
        internal static void DrawInspectorExcept(this SerializedObject serializedObject, string[] fieldsToSkip)
        {
            serializedObject.Update();
            var prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                    if (fieldsToSkip.Any(prop.name.Contains))
                        continue;

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
                } while (prop.NextVisible(false));
            }
        }

        internal static void DrawCustomInspector(this SerializedObject serializedObject, HashSet<string> fieldsToSkip,
            System.Type genericType, bool isReadOnly)
        {
            serializedObject.Update();
            var prop = serializedObject.GetIterator();
            var runtimeValueProperty = serializedObject.FindProperty("_runtimeValue");
            var savedProperty = serializedObject.FindProperty("_saved");
            var guidProperty = serializedObject.FindProperty("_guid");
            var saveGuidProperty = serializedObject.FindProperty("_saveGuid");

            if (prop.NextVisible(true))
            {
                do
                {
                    if (fieldsToSkip.Contains(prop.name))
                        continue;

                    if (prop.name == "_value")
                    {
                        DrawValueField(prop.name, serializedObject.targetObject, isReadOnly);
                    }
                    else if (prop.name != "_runtimeValue")
                    {
                        EditorGUILayout.PropertyField(prop, true);
                    }

                    //Draw save properties
                    if (prop.name == "_saved" && savedProperty.boolValue)
                    {
                        DrawSaveProperties();
                    }
                } while (prop.NextVisible(false));
            }

            serializedObject.ApplyModifiedProperties();

            void DrawValueField(string propertyName, Object target, bool isValueReadOnly)
            {
                var isUnityObject = genericType != null && typeof(UnityEngine.Object).IsAssignableFrom(genericType);
                if (Application.isPlaying)
                {
                    //Draw Object field
                    if (isUnityObject)
                    {
                        GUI.enabled = !isValueReadOnly;
                        var objectValue = EditorGUILayout.ObjectField("Runtime Value",
                            runtimeValueProperty.objectReferenceValue, genericType,
                            true);
                        target.GetType().GetProperty("Value").SetValue(target, objectValue);
                        GUI.enabled = true;
                    }
                    else
                    {
                        GUI.enabled = !isValueReadOnly;
                        EditorGUILayout.PropertyField(runtimeValueProperty);
                        GUI.enabled = true;
                    }
                }
                else
                {
                    //Draw Object field
                    if (isUnityObject)
                    {
                        var tooltip = "The value should only be set at runtime.";
                        GUI.enabled = false;
                        EditorGUILayout.ObjectField(new GUIContent("Value", tooltip), null, genericType, false);
                        GUI.enabled = true;
                    }
                    else
                    {
                        GUI.enabled = !isValueReadOnly;
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName), true);
                        GUI.enabled = true;
                    }
                }
            }

            void DrawSaveProperties()
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(saveGuidProperty, true);
                var saveGuidType = (SaveGuidType)saveGuidProperty.enumValueIndex;
                if (saveGuidType == SaveGuidType.Auto)
                {
                    GUI.enabled = false;
                    EditorGUILayout.TextField(guidProperty.stringValue);
                    GUI.enabled = true;
                }
                else
                {
                    guidProperty.stringValue = EditorGUILayout.TextField(guidProperty.stringValue);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
        }

        internal static void DrawOnlyField(this SerializedObject serializedObject, string fieldName,
            bool isReadOnly)
        {
            serializedObject.Update();
            var prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                    if (prop.name != fieldName)
                        continue;

                    GUI.enabled = !isReadOnly;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
                    GUI.enabled = true;
                } while (prop.NextVisible(false));
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw all properties except the ones specified.
        /// Also disables the m_Script property.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyToExclude"></param>
        internal static void DrawPropertiesExcluding(SerializedObject obj, params string[] propertyToExclude)
        {
            obj.Update();
            SerializedProperty iterator = obj.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (!propertyToExclude.Contains(iterator.name))
                {
                    GUI.enabled = iterator.name != "m_Script";
                    EditorGUILayout.PropertyField(iterator, true);
                    GUI.enabled = true;
                }
            }

            obj.ApplyModifiedProperties();
        }

        internal static void DrawLine(int height = 1) => DrawColoredLine(height, new Color(0.5f, 0.5f, 0.5f, 1));

        internal static void DrawColoredLine(int height, Color color)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, color);
        }

        internal static void DrawVerticalColoredLine(int width, Color color)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, GUILayout.Width(width), GUILayout.ExpandHeight(true));
            rect.width = width;
            EditorGUI.DrawRect(rect, color);
        }

        internal static void DrawSelectableObject(Object obj, string[] labels)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(labels[0], GUILayout.MaxWidth(300)))
                EditorGUIUtility.PingObject(obj);

            if (GUILayout.Button(labels[1], GUILayout.MaxWidth(75)))
            {
                EditorGUIUtility.PingObject(obj);
                Selection.activeObject = obj;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
        }

        internal static Texture2D CreateTexture(Color color)
        {
            var result = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            result.SetPixel(0, 0, color);
            result.Apply();
            return result;
        }

        /// <summary> Centers a rect inside another window. </summary>
        /// <param name="window"></param>
        /// <param name="origin"></param>
        internal static Rect CenterInWindow(Rect window, Rect origin)
        {
            var pos = window;
            float w = (origin.width - pos.width) * 0.5f;
            float h = (origin.height - pos.height) * 0.5f;
            pos.x = origin.x + w;
            pos.y = origin.y + h;
            return pos;
        }

        internal static void DrawPopUpHeader(EditorWindow editorWindow, string titleName)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(Icons.Cancel, Styles.CancelButton))
                editorWindow.Close();

            EditorGUILayout.LabelField(titleName, Styles.Header);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        internal static bool DrawCallToActionButton(string text, ButtonSize size)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var style = new GUIStyle(Styles.CallToActionButton);
            switch (size)
            {
                case ButtonSize.Small:
                    style.fixedHeight = 20;
                    style.fixedWidth = 60;
                    break;
                case ButtonSize.Medium:
                    style.fixedHeight = 25;
                    style.fixedWidth = 75;
                    break;
                case ButtonSize.Large:
                    style.fixedHeight = 25;
                    style.fixedWidth = 150;
                    break;
            }

            Color originalColor = GUI.backgroundColor;
            var color = new Color(0.2f, 1.1f, 1.7f, 1);
            GUI.backgroundColor = color.Lighten(0.3f);
            var hasClicked = GUILayout.Button(text, style);
            GUI.backgroundColor = originalColor;
            GUILayout.EndHorizontal();
            return hasClicked;
        }

        internal enum ButtonSize
        {
            Small,
            Medium,
            Large
        }

        internal static void ShowTagMenu(int currentTagIndex, Action<int> onTagSelected)
        {
            var menu = new GenericMenu();
            var soapSettings = SoapEditorUtils.GetOrCreateSoapSettings();
            var tags = soapSettings.Tags.ToArray();
            for (int i = 0; i < tags.Length; i++)
            {
                var i1 = i;
                var isCurrentTag = currentTagIndex > 0 && i == currentTagIndex;
                menu.AddItem(new GUIContent(tags[i]), isCurrentTag, () => { onTagSelected(i1); });
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Add Tag..."), false, () =>
            {
                EditorWindow currentWindow = EditorWindow.focusedWindow;
                var position = currentWindow.position;
                //position.y -= currentWindow.position.height * 0.3f;
                var rect = new Rect(position);
                PopupWindow.Show(new Rect(), new TagPopUpWindow(rect));
            });
            menu.ShowAsContext();
        }

        internal static void ShowPathMenu(string[] options, int currentIndex, Action<int> onOptionSelected)
        {
            var menu = new GenericMenu();
            for (int i = 0; i < options.Length; i++)
            {
                var i1 = i;
                var isCurrentOption = currentIndex > 0 && i == currentIndex;
                menu.AddItem(new GUIContent(options[i]), isCurrentOption, () => { onOptionSelected(i1); });
            }

            menu.ShowAsContext();
        }

        internal static void DrawSerializationError(Type type, Rect position = default)
        {
            if (position == default)
            {
                EditorGUILayout.HelpBox($"{type} Value field cannot be shown as it it not marked as serializable." +
                                        "\n Add [System.Serializable] attribute.", MessageType.Warning);
            }
            else
            {
                var icon = EditorGUIUtility.IconContent("Error").image;
                GUI.DrawTexture(position, icon, ScaleMode.ScaleToFit);
            }
        }


        internal static class Icons
        {
            private static Texture _cancel;

            internal static Texture Cancel =>
                _cancel ? _cancel : _cancel = Resources.Load<Texture>("Icons/icon_cancel");

            private static Texture _subAsset;

            internal static Texture SubAsset =>
                _subAsset ? _subAsset : _subAsset = Resources.Load<Texture>("Icons/icon_subAsset");

            private static Texture _editTags;

            internal static Texture EditTags => _editTags
                ? _editTags
                : _editTags =
                    Resources.Load<Texture>("Icons/icon_edit");

            private static Texture _runtimeInjectable;

            internal static Texture RuntimeInjectable => _runtimeInjectable
                ? _runtimeInjectable
                : _runtimeInjectable =
                    Resources.Load<Texture>("Icons/icon_runtimeInjectable");

            private static Texture _scriptableVariable;

            internal static Texture ScriptableVariable => _scriptableVariable
                ? _scriptableVariable
                : _scriptableVariable =
                    Resources.Load<Texture>("Icons/icon_scriptableVariable");

            private static Texture _scriptableEvent;

            internal static Texture ScriptableEvent => _scriptableEvent
                ? _scriptableEvent
                : _scriptableEvent =
                    Resources.Load<Texture>("Icons/icon_scriptableEvent");

            private static Texture _scriptableList;

            internal static Texture ScriptableList => _scriptableList
                ? _scriptableList
                : _scriptableList =
                    Resources.Load<Texture>("Icons/icon_scriptableList");

            private static Texture _scriptableDictionary;

            internal static Texture ScriptableDictionary => _scriptableDictionary
                ? _scriptableDictionary
                : _scriptableDictionary =
                    Resources.Load<Texture>("Icons/icon_scriptableDictionary");

            private static Texture _scriptableEnum;

            internal static Texture ScriptableEnum => _scriptableEnum
                ? _scriptableEnum
                : _scriptableEnum =
                    Resources.Load<Texture>("Icons/icon_scriptableEnum");

            private static Texture _scriptableSave;

            internal static Texture ScriptableSave => _scriptableSave
                ? _scriptableSave
                : _scriptableSave =
                    Resources.Load<Texture>("Icons/icon_scriptableSave");

            private static Texture _eventListener;

            internal static Texture EventListener => _eventListener
                ? _eventListener
                : _eventListener =
                    Resources.Load<Texture>("Icons/icon_eventListener");

            private static Texture _scriptableSingleton;

            internal static Texture ScriptableSingleton => _scriptableSingleton
                ? _scriptableSingleton
                : _scriptableSingleton =
                    Resources.Load<Texture>("Icons/icon_scriptableSingleton");
            
            private static Texture _readOnly;

            internal static Texture ReadOnly => _readOnly
                ? _readOnly
                : _readOnly =
                    Resources.Load<Texture>("Icons/icon_lock");

            #region Icon Utils

            private static readonly MethodInfo SetIconForObject =
                typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);

            private static readonly MethodInfo CopyMonoScriptIconToImporters =
                typeof(MonoImporter).GetMethod("CopyMonoScriptIconToImporters",
                    BindingFlags.Static | BindingFlags.NonPublic);

            private static readonly Type AnnotationType = Type.GetType("UnityEditor.Annotation, UnityEditor");
            private static readonly FieldInfo AnnotationClassId = AnnotationType?.GetField("classID");
            private static readonly FieldInfo AnnotationScriptClass = AnnotationType?.GetField("scriptClass");

            private static readonly Type AnnotationUtilityType =
                Type.GetType("UnityEditor.AnnotationUtility, UnityEditor");

            private static readonly MethodInfo GetAnnotations =
                AnnotationUtilityType?.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic);

            private static readonly MethodInfo SetIconEnabled =
                AnnotationUtilityType?.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);

            internal static void SetIcons(MonoScript[] monoScripts)
            {
                foreach (var script in monoScripts)
                    TrySetSoapIcon(script);
                DisableIconsInGizmos(monoScripts);

                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                AssetDatabase.SaveAssets();
            }

            private static void TrySetSoapIcon(MonoScript monoScript)
            {
                var path = AssetDatabase.GetAssetPath(monoScript);
                var monoImporter = AssetImporter.GetAtPath(path) as MonoImporter;
                if (monoImporter == null)
                    return;
                TrySetSoapIcon(monoScript, monoImporter);
            }

            internal static void TrySetSoapIcon(MonoScript monoScript, MonoImporter monoImporter)
            {
                var icon = GetIconFor(monoScript.GetClass()) as Texture2D;
                if (icon == null)
                    return;
#if UNITY_2021_3_OR_NEWER
                monoImporter.SetIcon(icon);
                monoImporter.SaveAndReimport();
#else
                SetIconForObject?.Invoke(null, new object[] { monoScript, icon });
                CopyMonoScriptIconToImporters?.Invoke(null, new object[] { monoScript });
#endif
            }

            private static void DisableIconsInGizmos(MonoScript[] monoScripts)
            {
                var annotations = (Array)GetAnnotations.Invoke(null, null);
                foreach (var monoScript in monoScripts)
                {
                    foreach (var annotation in annotations)
                    {
                        string scriptClass = (string)AnnotationScriptClass.GetValue(annotation);
                        if (scriptClass == monoScript.name)
                        {
                            int classId = (int)AnnotationClassId.GetValue(annotation);
                            SetIconEnabled.Invoke(null, new object[] { classId, scriptClass, 0 });
                        }
                    }
                }
            }

            public static Texture GetIconFor(Type type)
            {
                if (type.IsSubclassOf(typeof(ScriptableVariableBase)))
                    return ScriptableVariable;
                if (type.IsSubclassOf(typeof(ScriptableEventBase)))
                    return ScriptableEvent;
                if (type.IsSubclassOf(typeof(ScriptableListBase)))
                    return ScriptableList;
                if (type.IsSubclassOf(typeof(ScriptableDictionaryBase)))
                    return ScriptableDictionary;
                if (type.IsSubclassOf(typeof(ScriptableEnumBase)))
                    return ScriptableEnum;
                if (type.IsSubclassOf(typeof(ScriptableSaveBase)))
                    return ScriptableSave;
                if (type.IsSubclassOf(typeof(EventListenerBase)))
                    return EventListener;
                if (SoapUtils.InheritsFromOpenGeneric(type, typeof(ScriptableSingleton<>)))
                    return ScriptableSingleton;
                return null;
            }

            #endregion
        }

        internal static class Styles
        {
            private static GUIStyle _header;

            internal static GUIStyle Header
            {
                get
                {
                    if (_header == null)
                    {
                        _header = new GUIStyle(EditorStyles.boldLabel)
                        {
                            fontSize = 14,
                            fontStyle = FontStyle.Bold,
                            alignment = TextAnchor.MiddleCenter,
                            fixedHeight = 25,
                            contentOffset = new Vector2(-10, 0)
                        };
                    }

                    return _header;
                }
            }

            private static GUIStyle _cancelButton;

            internal static GUIStyle CancelButton
            {
                get
                {
                    if (_cancelButton == null)
                    {
                        _cancelButton = new GUIStyle(GUIStyle.none)
                        {
                            padding = new RectOffset(4, 4, 4, 4),
                            margin = new RectOffset(4, 0, 4, 0),
                            fixedWidth = 20,
                            fixedHeight = 20
                        };
                    }

                    return _cancelButton;
                }
            }

            private static GUIStyle _popupContentStyle;

            internal static GUIStyle PopupContent
            {
                get
                {
                    if (_popupContentStyle == null)
                    {
                        _popupContentStyle = new GUIStyle(GUIStyle.none)
                        {
                            padding = new RectOffset(10, 10, 10, 10),
                        };
                    }

                    return _popupContentStyle;
                }
            }

            private static GUIStyle _callToActionButton;

            internal static GUIStyle CallToActionButton
            {
                get
                {
                    if (_callToActionButton == null)
                    {
                        _callToActionButton = new GUIStyle(GUI.skin.button);
                    }

                    return _callToActionButton;
                }
            }

            private static GUIStyle _toolbarButton;

            internal static GUIStyle ToolbarButton
            {
                get
                {
                    if (_toolbarButton == null)
                    {
                        _toolbarButton = new GUIStyle(EditorStyles.toolbar)
                        {
                            fontSize = 10,
                            contentOffset = new Vector2(0, 2),
                        };
                        _toolbarButton.normal.textColor = Color.gray.Lighten(0.1f);
                    }

                    return _toolbarButton;
                }
            }

            private static GUIStyle _editTagHeaderButton;

            internal static GUIStyle EditTagHeaderButton
            {
                get
                {
                    if (_editTagHeaderButton == null)
                    {
                        _editTagHeaderButton = new GUIStyle(GUI.skin.button)
                        {
                            padding = new RectOffset(4, 4, 4, 4),
                        };
                    }

                    return _editTagHeaderButton;
                }
            }
        }
    }
}