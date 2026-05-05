using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if !ODIN_INSPECTOR
namespace Obvious.Soap.Editor
{
    [CustomEditor(typeof(ScriptableEventNoParam))]
    public class ScriptableEventNoParamDrawer : UnityEditor.Editor
    {
        private static SoapSettings _settings;
        
        private void OnEnable()
        {
            if (_settings == null)
                _settings = SoapEditorUtils.GetOrCreateSoapSettings();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUI.enabled = EditorApplication.isPlaying || _settings.CanEventsBeRaisedInEditor;
            if (GUILayout.Button("Raise"))
            {
                var eventNoParam = (ScriptableEventNoParam)target;
                eventNoParam.Raise();
            }
            GUI.enabled = true;

            if (!EditorApplication.isPlaying)
            {
                return;
            }

            SoapInspectorUtils.DrawLine();

            var goContainer = (IDrawObjectsInInspector)target;
            var gameObjects = goContainer.EditorListeners;
            if (gameObjects.Count > 0)
                DisplayAll(gameObjects);
        }

        private void DisplayAll(IReadOnlyList<Object> objects)
        {
            var title = $"Listeners : {objects.Count}";
            EditorGUILayout.LabelField(title);
            foreach (var obj in objects)
                EditorGUILayout.ObjectField(obj, typeof(Object), true);
        }
    }
}
#endif