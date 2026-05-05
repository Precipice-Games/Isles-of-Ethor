using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Reflection;
using Object = UnityEngine.Object;

#if !ODIN_INSPECTOR
namespace Obvious.Soap.Editor
{
    [CustomEditor(typeof(ScriptableDictionaryBase), true)]
    public class ScriptableDictionaryDrawer : UnityEditor.Editor
    {
        private ScriptableDictionaryBase _scriptableDictionaryBase;
        private static bool _repaintFlag;

        public override void OnInspectorGUI()
        {
            if (_scriptableDictionaryBase == null)
                _scriptableDictionaryBase = target as ScriptableDictionaryBase;
            
            DrawDefaultInspector();

            var canBeSerialized = _scriptableDictionaryBase.CanBeSerialized();
            if (!canBeSerialized)
            {
                SoapInspectorUtils.DrawSerializationError(_scriptableDictionaryBase.GetGenericType);
                return;
            }

            if (!EditorApplication.isPlaying)
                return;

            SoapInspectorUtils.DrawLine();
            DisplayAll();
        }

        private void DisplayAll()
        {
            var dictionaryType = _scriptableDictionaryBase.GetType();
            var dictionaryField =
                dictionaryType.GetField("_dictionary", BindingFlags.NonPublic | BindingFlags.Instance);
            if (dictionaryField == null) 
                return;

            var dictionary = dictionaryField.GetValue(_scriptableDictionaryBase) as IDictionary;
            if (dictionary == null) 
                return;

            EditorGUILayout.LabelField($"Dictionary Count: {_scriptableDictionaryBase.Count}");

            foreach (DictionaryEntry entry in dictionary)
            {
                EditorGUILayout.BeginHorizontal();
                DrawField("Key", entry.Key);
                DrawField("Value", entry.Value);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawField(string label, object value)
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(50));
            if (value is Object unityObject)
            {
                EditorGUILayout.ObjectField(unityObject, typeof(Object), true);
            }
            else if (value != null)
            {
                EditorGUILayout.TextField(value.ToString());
            }
        }

        #region Repaint

        private void OnEnable()
        {
            if (_repaintFlag)
                return;

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            _repaintFlag = true;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.EnteredPlayMode)
            {
                _scriptableDictionaryBase.RepaintRequest += OnRepaintRequested;
            }
            else if (obj == PlayModeStateChange.EnteredEditMode)
            {
                _scriptableDictionaryBase.RepaintRequest -= OnRepaintRequested;
            }
        }

        private void OnRepaintRequested() => Repaint();

        #endregion
    }
}
#endif