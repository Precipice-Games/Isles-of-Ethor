using UnityEditor;

#if !ODIN_INSPECTOR
namespace Obvious.Soap.Editor
{
    [CustomEditor(typeof(BindSliderInt))]
    [CanEditMultipleObjects]
    public class BindSliderIntDrawer : UnityEditor.Editor
    {
        SerializedProperty _floatVariableProperty;
        SerializedProperty _useMaxValueFromVariableProperty;
        SerializedProperty _maxValueProperty;

        private void OnEnable()
        {
            _floatVariableProperty = serializedObject.FindProperty("_intVariable");
            _useMaxValueFromVariableProperty = serializedObject.FindProperty("_useMaxValueFromVariable");
            _maxValueProperty = serializedObject.FindProperty("_maxValue");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            Undo.RecordObject(target, "Modified Custom Inspector");
            
            EditorGUILayout.PropertyField(_floatVariableProperty);
            _useMaxValueFromVariableProperty.boolValue =
                EditorGUILayout.Toggle("Use Variable Max Value",
                    _useMaxValueFromVariableProperty.boolValue);
            if (!_useMaxValueFromVariableProperty.boolValue)
            {
                EditorGUILayout.PropertyField(_maxValueProperty);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif