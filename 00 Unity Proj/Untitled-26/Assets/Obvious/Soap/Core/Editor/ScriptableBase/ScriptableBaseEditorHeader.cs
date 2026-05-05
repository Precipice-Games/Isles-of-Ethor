using System;
using System.Collections.Generic;
using UnityEngine;

namespace Obvious.Soap.Editor
{
    using UnityEditor;

    [InitializeOnLoad]
    static class ScriptableBaseEditorHeader
    {
        private static SoapSettings _soapSettings;
        private static readonly GUIContent _tagContent;

        static ScriptableBaseEditorHeader()
        {
            _soapSettings = SoapEditorUtils.GetOrCreateSoapSettings();
            Editor.finishedDefaultHeaderGUI += DrawHeader;
            _tagContent = new GUIContent();
            _tagContent.image = SoapInspectorUtils.Icons.EditTags;
            _tagContent.tooltip = "Edit Tags";
        }
        
        private static void DrawHeader(Editor editor)
        {
            if (!EditorUtility.IsPersistent(editor.target))
                return;

            if (editor.targets.Length > 1)
            {
                //If there is more than one target, we check if they are all ScriptableBase
                var scriptableBases = new List<ScriptableBase>();
                foreach (var target in editor.targets)
                {
                    var scriptableBase = target as ScriptableBase;
                    if (scriptableBase == null)
                        return;
                    scriptableBases.Add(scriptableBase);
                }

                DrawHeaderForMultipleAssetsSelected(scriptableBases);
            }
            else if (editor.target is ScriptableBase scriptableBase)
            {
                DrawHeaderForOneAssetSelected(scriptableBase);
            }
        }

        private static void DrawHeaderForOneAssetSelected(ScriptableBase scriptableBase)
        {
            EditorGUILayout.BeginHorizontal();
            //Draw Description label
            GUIStyle labelStyle = new GUIStyle(EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField("Description:", labelStyle, GUILayout.Width(65));
            GUILayout.FlexibleSpace();
            
            //Draw Tag
            if (_soapSettings!= null)
                DrawTag(new List<ScriptableBase> { scriptableBase });
            EditorGUILayout.EndHorizontal();

            //Draw Description text area
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;
            EditorGUI.BeginChangeCheck();

            var description = EditorGUILayout.TextArea(scriptableBase.Description);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(scriptableBase, "Change Description");
                scriptableBase.Description = description;
                EditorUtility.SetDirty(scriptableBase);
            }
        }

        private static void DrawHeaderForMultipleAssetsSelected(List<ScriptableBase> scriptableBases)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (_soapSettings!= null) //sometimes when you change branch on VC, this is null
                DrawTag(scriptableBases);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawTag(List<ScriptableBase> scriptableBases)
        {
            EditorGUILayout.LabelField("Tag:", EditorStyles.miniBoldLabel, GUILayout.Width(30f));
            var currentTag = HaveSameTags(scriptableBases)
                ?  _soapSettings.GetTagName(scriptableBases[0])
                : "-";
            var style = new GUIStyle(EditorStyles.popup);
            if (GUILayout.Button(currentTag, style, GUILayout.Width(150f)))
            {
                var onlyOneAsset = scriptableBases.Count == 1;
                var currentTagIndex = onlyOneAsset ? scriptableBases[0].TagIndex : -1;
                SoapInspectorUtils.ShowTagMenu(currentTagIndex,(newTag) =>
                {
                    foreach (var scriptableBase in scriptableBases)
                    {
                        Undo.RecordObject(scriptableBase, "Change Tag");
                        scriptableBase.TagIndex = newTag;
                        EditorUtility.SetDirty(scriptableBase);
                    }
                });
            }
        }
        
        private static bool HaveSameTags(List<ScriptableBase> scriptableBases)
        {
            if (scriptableBases.Count <= 1)
                return true;
            var tagIndex = scriptableBases[0].TagIndex;
            return scriptableBases.TrueForAll(sb => sb.TagIndex == tagIndex);
        }
    }
}