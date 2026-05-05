using UnityEngine;
using UnityEditor;

namespace Obvious.Soap.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableCollection), true)]
    public class ScriptableCollectionPropertyDrawer : ScriptableBasePropertyDrawer
    {
        private SerializedObject _serializedObject;
        private ScriptableCollection _scriptableCollection;

        protected override void DrawUnExpanded(Rect position, SerializedProperty property, GUIContent label,
            Object targetObject)
        {
            if (_serializedObject == null || _serializedObject.targetObject != targetObject)
                _serializedObject = new SerializedObject(targetObject);

            _serializedObject.UpdateIfRequiredOrScript();
            base.DrawUnExpanded(position, property, label, targetObject);
            if (_serializedObject.targetObject != null) //can be destroyed when using sub assets
                _serializedObject.ApplyModifiedProperties();
        }

        protected override void DrawShortcut(Rect rect, SerializedProperty property, Object targetObject)
        {
            if (_scriptableCollection == null)
                _scriptableCollection = _serializedObject.targetObject as ScriptableCollection;
            
            //can be destroyed when using sub assets
            if (targetObject == null)
                return;
            
            DrawShortcut(rect);
        }
        
        public void DrawShortcut(Rect rect)
        {
            var count = _scriptableCollection.Count;
            EditorGUI.LabelField(rect, "Count: " + count);
        }
        
        public ScriptableCollectionPropertyDrawer(SerializedObject serializedObject, ScriptableCollection scriptableCollection)
        {
            _serializedObject = serializedObject;
            _scriptableCollection = scriptableCollection;
        }
        
        public ScriptableCollectionPropertyDrawer() { }
    }
}