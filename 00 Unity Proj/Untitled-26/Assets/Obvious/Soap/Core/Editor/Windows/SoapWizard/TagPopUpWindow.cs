using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    public class TagPopUpWindow : PopupWindowContent
    {
        private readonly Rect _position;
        private readonly Vector2 _dimensions = new Vector2(350, 350);
        private readonly float _lineHeight = 20f;
        private readonly SoapSettings _soapSettings;
        private readonly List<ScriptableBase> _scriptableBases;
        private Vector2 _scrollPosition = Vector2.zero;
        private Texture[] _icons;
        private bool _isAddingNewCategory = false;
        private int _categoryBeingRenamed = -1;
        private int _categoryBeingDeleted = -1;
        private string _categoryName;

        public override Vector2 GetWindowSize() => _dimensions;

        public TagPopUpWindow(Rect origin)
        {
            _position = origin;
            _scriptableBases = SoapEditorUtils.FindAll<ScriptableBase>();
            _soapSettings = SoapEditorUtils.GetOrCreateSoapSettings();
            LoadIcons();
            SoapWizardWindow.IsPopupOpen = true;
        }
        
        public override void OnClose()
        {
            SoapWizardWindow.IsPopupOpen = false;
        }

        private void LoadIcons()
        {
            _icons = new Texture[4];
            _icons[0] = Resources.Load<Texture>("Icons/icon_edit");
            _icons[1] = Resources.Load<Texture>("Icons/icon_delete");
            _icons[2] = EditorGUIUtility.IconContent("Warning").image;
            _icons[3] = Resources.Load<Texture>("Icons/icon_cancel");
        }

        public override void OnGUI(Rect rect)
        {
            editorWindow.position = SoapInspectorUtils.CenterInWindow(editorWindow.position, _position);
            SoapInspectorUtils.DrawPopUpHeader(editorWindow, "Edit Tags");
            GUILayout.BeginVertical(SoapInspectorUtils.Styles.PopupContent);
            GUILayout.Space(10f);
            DrawCategories();
            GUILayout.Space(10f);
            DrawButtons();
            GUILayout.EndVertical();
        }

        private void DrawCategories()
        {
            GUI.enabled = !_isAddingNewCategory;
            EditorGUILayout.BeginVertical();

            //Draw the default category
            var labelStyle = new GUIStyle(GUI.skin.box);
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            EditorGUILayout.LabelField("None", labelStyle, GUILayout.ExpandWidth(true));

            //Draw the rest of the categories
            for (int i = 1; i < _soapSettings.Tags.Count; i++)
            {
                if (i == _categoryBeingRenamed)
                    DrawCategoryBeingRenamed(i);
                else if (i == _categoryBeingDeleted)
                    DrawCategoryBeingDeleted(i);
                else
                    DrawDefaultCategoryEntry(i);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            GUI.enabled = true;
        }

        private void DrawButtons()
        {
            if (_isAddingNewCategory)
                DrawNewCategoryBeingAdded();
            else
            {
                if (HasReachedMaxCategory)
                {
                    var labelStyle = new GUIStyle(EditorStyles.label);
                    labelStyle.normal.textColor = Color.red;
                    labelStyle.alignment = TextAnchor.MiddleCenter;
                    EditorGUILayout.LabelField("Maximum Amount of Tags reached (32)", labelStyle);
                }
                else
                {
                    DrawAddNewCategoryButton();
                }
            }

            void DrawNewCategoryBeingAdded()
            {
                EditorGUILayout.BeginHorizontal();

                DrawRenameLayout(() =>
                {
                    _isAddingNewCategory = false;
                    _soapSettings.Tags.Add(_categoryName);
                }, () => { _isAddingNewCategory = false; });

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
            }

            void DrawAddNewCategoryButton()
            {
                GUI.enabled = IsAllowedToCreateNewCategory;
                if (SoapInspectorUtils.DrawCallToActionButton("Add New Tag", SoapInspectorUtils.ButtonSize.Large))
                {
                    _categoryName = "";
                    _isAddingNewCategory = true;
                }
                GUI.enabled = true;
            }
        }

        private void DrawCategoryBeingRenamed(int index)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            DrawRenameLayout(() =>
            {
                _categoryBeingRenamed = -1;
                _soapSettings.Tags[index] = _categoryName;
            }, () => { _categoryBeingRenamed = -1; });
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCategoryBeingDeleted(int i)
        {
            if (IsCategoryUsed(i, out var usedBy))
            {
                DrawUseCategoryBeingDeleted();
            }
            else
            {
                DrawConfirmDelete();
            }

            void DrawUseCategoryBeingDeleted()
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal(GUI.skin.box);

                //Warning icon
                var iconStyle = new GUIStyle(GUIStyle.none);
                iconStyle.margin = new RectOffset(0, 0, 5, 0);
                GUILayout.Box(_icons[2], iconStyle, GUILayout.Width(_lineHeight), GUILayout.Height(_lineHeight));

                //Label
                EditorGUILayout.LabelField($"{_soapSettings.Tags[i]} can't be deleted because it's used by:");

                //Cancel Button
                if (GUILayout.Button(_icons[3], SoapInspectorUtils.Styles.EditTagHeaderButton, GUILayout.Width(_lineHeight),
                        GUILayout.Height(_lineHeight)))
                    _categoryBeingDeleted = -1;

                EditorGUILayout.EndHorizontal();

                //Draw the list of ScriptableBases that use this category
                var originalColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red.Lighten(.75f);
                EditorGUILayout.BeginVertical(GUI.skin.box);
                foreach (var scriptableBase in usedBy)
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    EditorGUILayout.LabelField(scriptableBase.name);
                    var categories = _soapSettings.Tags.ToArray();

                    EditorGUI.BeginChangeCheck();
                    int newCategoryIndex = EditorGUILayout.Popup(scriptableBase.TagIndex, categories);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(scriptableBase, "Change Tag");
                        scriptableBase.TagIndex = newCategoryIndex;
                        EditorUtility.SetDirty(scriptableBase);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                GUI.backgroundColor = originalColor;
                if (GUILayout.Button("Set Default category for all", GUILayout.Height(_lineHeight)))
                {
                    foreach (var scriptableBase in usedBy)
                    {
                        scriptableBase.TagIndex = 0;
                        EditorUtility.SetDirty(scriptableBase);
                    }
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
            }

            void DrawConfirmDelete()
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField($"Confirm Delete {_soapSettings.Tags[i]}?");
                if (GUILayout.Button("Yes", GUILayout.Width(35), GUILayout.Height(_lineHeight)))
                {
                    _soapSettings.Tags.RemoveAt(i);
                    EditorUtility.SetDirty(_soapSettings);
                    _categoryBeingDeleted = -1;
                }

                if (GUILayout.Button("No", GUILayout.Width(35), GUILayout.Height(_lineHeight)))
                    _categoryBeingDeleted = -1;
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawDefaultCategoryEntry(int i)
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.LabelField(_soapSettings.Tags[i]);
            if (GUILayout.Button(_icons[0], SoapInspectorUtils.Styles.EditTagHeaderButton, GUILayout.Width(_lineHeight),
                    GUILayout.Height(_lineHeight)))
            {
                _categoryBeingRenamed = i;
                _categoryName = _soapSettings.Tags[i];
            }

            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red.Lighten(.75f);
            if (GUILayout.Button(_icons[1], SoapInspectorUtils.Styles.EditTagHeaderButton, GUILayout.Width(_lineHeight),
                    GUILayout.Height(_lineHeight)))
            {
                _categoryBeingDeleted = i;
            }

            GUI.backgroundColor = originalColor;
            EditorGUILayout.EndHorizontal();
        }


        private void DrawRenameLayout(Action onConfirm, Action onCancel)
        {
            _categoryName = EditorGUILayout.TextField(_categoryName, EditorStyles.textField, GUILayout.Height(_lineHeight));
            GUI.enabled = IsNameValid(_categoryName);
            
            if (SoapInspectorUtils.DrawCallToActionButton("Confirm", SoapInspectorUtils.ButtonSize.Small))
            {
                onConfirm?.Invoke();
                EditorUtility.SetDirty(_soapSettings);
            }
            GUI.enabled = true;
            if (GUILayout.Button("Cancel", GUILayout.Width(60), GUILayout.Height(_lineHeight)))
            {
                _categoryName = "";
                onCancel?.Invoke();
            }
        }

        private bool IsNameValid(string name)
        {
            return !string.IsNullOrEmpty(name) && !_soapSettings.Tags.Contains(name);
        }

        private bool IsCategoryUsed(int categoryIndex, out List<ScriptableBase> usedBy)
        {
            usedBy = _scriptableBases.Where(x => x.TagIndex == categoryIndex).ToList();
            return usedBy.Count > 0;
        }

        private bool HasReachedMaxCategory => _soapSettings.Tags.Count >= 32;

        private bool IsAllowedToCreateNewCategory => _categoryBeingDeleted == -1 && _categoryBeingRenamed == -1;
    }
}