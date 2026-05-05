using UnityEngine;
using UnityEditor;

namespace Obvious.Soap.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableEventNoParam), true)]
    public class ScriptableEventNoParamPropertyDrawer : ScriptableBasePropertyDrawer
    {
        private static SoapSettings _settings;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            if (_canBeSubAsset == null)
                _canBeSubAsset = SoapEditorUtils.CanBeSubAsset(property, fieldInfo);
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
            var eventNoParam = (ScriptableEventNoParam)property.objectReferenceValue;
            DrawShortcut(rect, eventNoParam);
        }

        public void DrawShortcut(Rect rect, ScriptableEventNoParam scriptableEventNoParam)
        {
            if (_settings == null)
                _settings = SoapEditorUtils.GetOrCreateSoapSettings();

            GUI.enabled = EditorApplication.isPlaying || _settings.CanEventsBeRaisedInEditor;
            if (GUI.Button(rect, "Raise"))
            {
                scriptableEventNoParam.Raise();
            }

            GUI.enabled = true;
        }
    }
}