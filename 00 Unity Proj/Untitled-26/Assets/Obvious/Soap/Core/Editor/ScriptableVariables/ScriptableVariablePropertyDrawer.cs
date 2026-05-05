namespace Obvious.Soap.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(ScriptableVariableBase), true)]
    public class ScriptableVariablePropertyDrawer : ScriptableBasePropertyDrawer
    {
        private SerializedObject _serializedObject;
        private ScriptableVariableBase _scriptableVariable;
        private float? _propertyWidthRatio;
        private ScriptableVariableEditor _scriptableVariableEditor;

        protected override string GetFieldName()
        {
            //fieldInfo.Name does not work for VariableReferences so we have to make an edge case for that.
            var isAbstract = fieldInfo.DeclaringType?.IsAbstract == true;
            var fieldName = isAbstract ? fieldInfo.FieldType.Name : fieldInfo.Name;
            return fieldName;
        }

        protected override void DrawEmbeddedEditor(Object targetObject)
        {
            if (_scriptableVariableEditor == null)
            {
                var editorType = typeof(ScriptableVariableEditor);
#if !ODIN_INSPECTOR
                editorType = typeof(ScriptableVariableDrawer); //Override to non-odin drawer as cannot instantiate abstract class
#endif
                Editor.CreateCachedEditor(targetObject, editorType, ref _editor);
                _scriptableVariableEditor = _editor as ScriptableVariableEditor;
                _scriptableVariableEditor.SetIsReadOnly(IsReadOnly.HasValue && IsReadOnly.Value);
            }
            _scriptableVariableEditor.OnInspectorGUI();
        }

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
            if (_scriptableVariable == null)
                _scriptableVariable = _serializedObject.targetObject as ScriptableVariableBase;

            //can be destroyed when using sub assets
            if (targetObject == null)
                return;

            DrawShortcut(rect);
        }

        public void DrawShortcut(Rect rect)
        {
            var genericType = _scriptableVariable.GetGenericType;
            var canBeSerialized = SoapUtils.IsUnityType(genericType) || SoapUtils.IsSerializable(genericType);
            if (!canBeSerialized)
            {
                SoapInspectorUtils.DrawSerializationError(genericType, rect);
                return;
            }

            var propertyName = Application.isPlaying ? "_runtimeValue" : "_value";
            var value = _serializedObject.FindProperty(propertyName);

            var isSceneObject = typeof(Component).IsAssignableFrom(genericType) ||
                                typeof(GameObject).IsAssignableFrom(genericType);

            if (isSceneObject)
            {
                GUI.enabled = !IsReadOnly.HasValue || !IsReadOnly.Value;
                var objectValue = EditorGUI.ObjectField(rect, value.objectReferenceValue, genericType, true);
                if (objectValue != value.objectReferenceValue)
                {
                    _serializedObject.targetObject.GetType().GetProperty("Value")?
                        .SetValue(_serializedObject.targetObject, objectValue);
                }
                GUI.enabled = true;
            }
            else
            {
                if (value != null)
                {
                    GUI.enabled = !IsReadOnly.HasValue || !IsReadOnly.Value;
                    EditorGUI.PropertyField(rect, value, GUIContent.none);
                    GUI.enabled = true;
                }
            }
        }

        public ScriptableVariablePropertyDrawer(SerializedObject serializedObject,
            ScriptableVariableBase scriptableVariableBase)
        {
            _serializedObject = serializedObject;
            _scriptableVariable = scriptableVariableBase;
            _propertyWidthRatio = null;
            
        }

        public ScriptableVariablePropertyDrawer()
        {
        }

        protected override float WidthRatio
        {
            get
            {
                if (_scriptableVariable == null)
                {
                    _propertyWidthRatio = null;
                    return 0.82f;
                }

                if (_propertyWidthRatio.HasValue)
                    return _propertyWidthRatio.Value;

                var genericType = _scriptableVariable.GetGenericType;
                if (genericType == typeof(Vector2))
                    _propertyWidthRatio = 0.72f;
                else if (genericType == typeof(Vector3))
                    _propertyWidthRatio = 0.62f;
                else
                    _propertyWidthRatio = 0.82f;
                return _propertyWidthRatio.Value;
            }
        }
    }
}