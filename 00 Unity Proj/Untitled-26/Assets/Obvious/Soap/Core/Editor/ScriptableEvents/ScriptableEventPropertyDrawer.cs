using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Obvious.Soap.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableEventBase), true)]
    public class ScriptableEventPropertyDrawer : ScriptableBasePropertyDrawer
    {
        private SerializedObject _serializedObject;
        private ScriptableEventBase _scriptableEventBase;
        private static SoapSettings _settings;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            if (_canBeSubAsset == null)
                _canBeSubAsset = SoapEditorUtils.CanBeSubAsset(property,fieldInfo);
            
            var targetObject = property.objectReferenceValue;
            if (targetObject == null)
            {
                DrawIfNull(position, property, label);
                return;
            }
            
            //TODO: make this more robust. Disable foldout fo all arrays of serializable class that contain ScriptableBase
            var isEventListener = property.serializedObject.targetObject is EventListenerBase;
            if (isEventListener)
            {
                DrawUnExpanded(position, property, label, targetObject);
                EditorGUI.EndProperty();
                return;
            }
            
            DrawIfNotNull(position, property, label, property.objectReferenceValue);
            
            EditorGUI.EndProperty();
        }

        protected override void DrawShortcut(Rect rect, SerializedProperty property, Object targetObject)
        {
            if (_serializedObject == null || _serializedObject.targetObject != targetObject)
                _serializedObject = new SerializedObject(targetObject);
            
            //can be destroyed when using sub assets
            if (targetObject == null)
                return;
            
            _serializedObject.UpdateIfRequiredOrScript();
            
            if (_scriptableEventBase == null)
                _scriptableEventBase = _serializedObject.targetObject as ScriptableEventBase;
            
            var genericType = _scriptableEventBase.GetGenericType;
            var canBeSerialized = SoapUtils.IsUnityType(genericType) || SoapUtils.IsSerializable(genericType);
            if (!canBeSerialized)
            {
                SoapInspectorUtils.DrawSerializationError(genericType,rect);
                return;
            }

            if (_settings == null)
                _settings = SoapEditorUtils.GetOrCreateSoapSettings();
            
            GUI.enabled = EditorApplication.isPlaying || _settings.CanEventsBeRaisedInEditor;
            if (GUI.Button(rect, "Raise"))
            {
                var method = property.objectReferenceValue.GetType().BaseType.GetMethod("Raise",
                    BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
                var asset = new SerializedObject(property.objectReferenceValue);
                var valueProp = asset.FindProperty("_debugValue");
                method.Invoke(targetObject, new[] { GetDebugValue(valueProp) });
            }

            GUI.enabled = true;
        }
        
        //Drawn from the Soap window
        public void DrawShortcut(Rect rect)
        {
            var genericType = _scriptableEventBase.GetGenericType;
            var canBeSerialized = SoapUtils.IsUnityType(genericType) || SoapUtils.IsSerializable(genericType);
            if (!canBeSerialized)
            {
                SoapInspectorUtils.DrawSerializationError(genericType,rect);
                return;
            }

            if (_settings == null)
                _settings = SoapEditorUtils.GetOrCreateSoapSettings();
            
            GUI.enabled = EditorApplication.isPlaying || _settings.CanEventsBeRaisedInEditor;
            SerializedProperty valueProp = _serializedObject.FindProperty("_debugValue");
            rect.width *= 0.5f;
            if (GUI.Button(rect, "Raise"))
            {
                var method = _scriptableEventBase.GetType().BaseType.GetMethod("Raise",
                    BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
                valueProp = _serializedObject.FindProperty("_debugValue");
                method.Invoke(_serializedObject.targetObject, new[] { GetDebugValue(valueProp) });
            }
            var valueRect = new Rect(rect);
            valueRect.x += rect.width + 5f;
            EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);

            GUI.enabled = true;
        }
        
        public ScriptableEventPropertyDrawer(SerializedObject serializedObject, ScriptableEventBase scriptableEventBase)
        {
            _serializedObject = serializedObject;
            _scriptableEventBase = scriptableEventBase;
        }
        
        public ScriptableEventPropertyDrawer() { }

        private object GetDebugValue(SerializedProperty property)
        {
            var targetType = property.serializedObject.targetObject.GetType();
            var targetField = targetType.GetField("_debugValue", BindingFlags.Instance | BindingFlags.NonPublic);
            return targetField.GetValue(property.serializedObject.targetObject);
        }
    }
}