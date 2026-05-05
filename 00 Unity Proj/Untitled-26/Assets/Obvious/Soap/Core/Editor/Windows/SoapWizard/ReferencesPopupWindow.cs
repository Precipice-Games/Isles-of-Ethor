using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Button = UnityEngine.UI.Button;

namespace Obvious.Soap.Editor
{
    public class ReferencesPopupWindow : EditorWindow
    {
        private ScriptableBase _scriptableBase;
        private Vector2 _scrollPosition = Vector2.zero;
        private Texture[] _icons;
        private Dictionary<string, int> _assetReferences;
        private List<(GameObject, Type, string, string)> _sceneReferences;
        private bool _sceneFoldout = true;
        private bool _assetFoldout = true;
        private GUIStyle _windowStyle;
        private FindReferenceType _findReferenceType = FindReferenceType.All;
        private GUIStyle _foldoutStyle;


        public static void ShowWindow(Rect rect, ScriptableBase scriptableBase,
            FindReferenceType findReferenceType = FindReferenceType.All)
        {
            var window = CreateInstance<ReferencesPopupWindow>();
            window.Init(scriptableBase, findReferenceType);
            window.titleContent = new GUIContent("Find References");
            window.position = new Rect(rect.x, rect.y, 350, 350);
            window.ShowPopup();
            window.position = SoapInspectorUtils.CenterInWindow(window.position, rect);
        }

        private void Init(ScriptableBase scriptableBase, FindReferenceType findReferenceType)
        {
            _scriptableBase = scriptableBase;
            _findReferenceType = findReferenceType;
            _foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold
            };
            LoadIcons();
            if (_findReferenceType.HasFlag(FindReferenceType.Scene))
            {
                _sceneReferences = SoapEditorUtils.FindReferencesInScene(_scriptableBase);
                EditorApplication.hierarchyChanged += OnHierarchyChanged;
            }

            if (_findReferenceType.HasFlag(FindReferenceType.Project))
            {
                _assetReferences = SoapEditorUtils.FindReferencesInProject(_scriptableBase);
                EditorApplication.projectChanged += OnProjectChanged;
            }

            _windowStyle = new GUIStyle();
            var darkness = 0.235f;
            _windowStyle.normal.background =
                SoapInspectorUtils.CreateTexture(new Color(darkness, darkness, darkness, 1f));
            SoapWizardWindow.IsPopupOpen = true;
        }

        private void OnDisable()
        {
            SoapWizardWindow.IsPopupOpen = false;
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.projectChanged -= OnProjectChanged;
        }

        private void LoadIcons()
        {
            _icons = new Texture[5];
            _icons[0] = EditorGUIUtility.IconContent("SceneAsset Icon").image;
            _icons[1] = EditorGUIUtility.IconContent("Folder Icon").image;
            _icons[2] = Resources.Load<Texture>("Icons/icon_eventListener");
            _icons[3] = EditorGUIUtility.IconContent("Button Icon").image;
            _icons[4] = EditorGUIUtility.IconContent("cs Script Icon").image;
        }

        public void OnGUI()
        {
            DrawBorder(new Color(0.388f, 0.388f, 0.388f, 1f)); // Dark grey
            GUILayout.BeginArea(new Rect(1, 1, position.width - 2, position.height - 2));
            GUILayout.BeginVertical(_windowStyle);
            SoapInspectorUtils.DrawPopUpHeader(this, "Find References (Used by)");
            GUILayout.BeginVertical(SoapInspectorUtils.Styles.PopupContent);
            DrawReferences();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawBorder(Color color)
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, 1), color); // Top
            EditorGUI.DrawRect(new Rect(0, position.height - 1, position.width, 1), color); // Bottom
            EditorGUI.DrawRect(new Rect(0, 0, 1, position.height), color); // Left
            EditorGUI.DrawRect(new Rect(position.width - 1, 0, 1, position.height), color); // Right
        }

        private void DrawReferences()
        {
            EditorGUILayout.BeginVertical();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            if (_findReferenceType.HasFlag(FindReferenceType.Scene))
                DrawSceneReferences();

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            
            if (_findReferenceType.HasFlag(FindReferenceType.Project))
                DrawProjectReferences();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawSceneReferences()
        {
            var suffix = _sceneReferences == null ? "" : $"({_sceneReferences.Count})";
            var foldoutText = $"Scene {suffix}";
            var newFoldoutState = EditorGUILayout.Foldout(_sceneFoldout, new GUIContent(foldoutText, _icons[0]),
                true,
                _foldoutStyle);

            if (newFoldoutState != _sceneFoldout)
            {
                _sceneFoldout = newFoldoutState;
                if (_sceneFoldout)
                {
                    _sceneReferences = SoapEditorUtils.FindReferencesInScene(_scriptableBase);
                }
            }

            if (_sceneFoldout)
            {
                if (_sceneReferences != null)
                {
                    foreach (var sceneReference in _sceneReferences)
                    {
                        if (sceneReference.Item1 == null) //object could have been deleted (for manual refresh mode)
                            continue;
                        Texture icon = GetComponentIcon(sceneReference.Item2);
                        var data = new ReferenceEntryData
                        {
                            Obj = sceneReference.Item1,
                            Icon = icon,
                            Content = $"{sceneReference.Item1.name} ({sceneReference.Item2.Name})",
                            Argument = sceneReference.Item3
                        };
                        DrawReferenceEntry(data, 0.7f);
                    }
                }
            }
        }

        private void DrawProjectReferences()
        {
            var suffix = _assetReferences == null ? "" : $"({_assetReferences.Count})";
            var foldoutText = $"Assets {suffix}";
            var newFoldoutState = EditorGUILayout.Foldout(_assetFoldout, new GUIContent(foldoutText, _icons[1]),
                true,
                _foldoutStyle);

            if (newFoldoutState != _assetFoldout)
            {
                _assetFoldout = newFoldoutState;
                if (_assetFoldout)
                {
                    _assetReferences = SoapEditorUtils.FindReferencesInProject(_scriptableBase);
                }
            }

            if (_assetFoldout)
            {
                if (_assetReferences != null)
                {
                    foreach (var assetReference in _assetReferences)
                    {
                        var assetPath = assetReference.Key;
                        var mainAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                        if (mainAsset == null) //object could have been deleted (for manual refresh mode)
                            continue;
                        var objectContent = EditorGUIUtility.ObjectContent(mainAsset, mainAsset.GetType());
                        Texture2D icon = objectContent.image as Texture2D;
                        var path = assetPath.Remove(0, 7);
                        var data = new ReferenceEntryData
                        {
                            Obj = mainAsset,
                            Icon = icon,
                            Content = mainAsset.name,
                            Argument = assetReference.Value.ToString()
                        };
                        DrawReferenceEntry(data, 0.7f);
                    }
                }
            }
        }


        void DrawReferenceEntry(ReferenceEntryData data, float ratio)
        {
            var width = position.width - 45; //weird offset but hey !
            EditorGUILayout.BeginHorizontal();
            var style = new GUIStyle(EditorStyles.objectField)
            {
                margin = new RectOffset(4, 0, 2, 2),
                padding = new RectOffset(2, 2, 2, 2),
                alignment = TextAnchor.MiddleLeft,
                fontSize = 10
            };
            if (GUILayout.Button(new GUIContent(data.Content, data.Icon), style,
                    GUILayout.Height(18), GUILayout.Width(width * ratio)))
            {
                EditorGUIUtility.PingObject(data.Obj);
            }

            style = new GUIStyle(EditorStyles.helpBox)
            {
                margin = new RectOffset(0, 0, 0, 0),
                stretchWidth = false,
                wordWrap = false,
                alignment = TextAnchor.MiddleLeft
            };
            EditorGUILayout.LabelField(data.Argument, style, GUILayout.Width(width * (1 - ratio)));
            EditorGUILayout.EndHorizontal();
        }

        Texture GetComponentIcon(Type componentType)
        {
            if (typeof(EventListenerBase).IsAssignableFrom(componentType))
            {
                return _icons[2];
            }

            if (typeof(Button).IsAssignableFrom(componentType))
            {
                return _icons[3];
            }

            return _icons[4];
        }

        private void OnHierarchyChanged()
        {
            _sceneReferences = SoapEditorUtils.FindReferencesInScene(_scriptableBase);
            Repaint();
        }

        private void OnProjectChanged()
        {
            _assetReferences = SoapEditorUtils.FindReferencesInProject(_scriptableBase);
            Repaint();
        }

        private struct ReferenceEntryData
        {
            public Texture Icon;
            public Object Obj;
            public string Content;
            public string Argument;
        }
    }

    [Flags]
    public enum FindReferenceType
    {
        Scene = 1,
        Project = 2,
        All = Scene | Project
    }
}