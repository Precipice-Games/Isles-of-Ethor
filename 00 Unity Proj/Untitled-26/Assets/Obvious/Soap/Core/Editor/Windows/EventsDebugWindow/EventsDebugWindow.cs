using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Obvious.Soap.Editor
{
    public class EventsDebugWindow : EditorWindow
    {
        private string _methodName = string.Empty;
        private bool _hasClicked = false;
        private bool _wasFound = false;

        [MenuItem("Window/Obvious/\ud83e\uddfc Soap/Events Debug Window")]
        public new static void Show()
        {
            var window = GetWindow<EventsDebugWindow>(typeof(SceneView));
            window.titleContent = new GUIContent("Events Debug Window", EditorGUIUtility.ObjectContent(CreateInstance<ScriptableEventNoParam>(), typeof(ScriptableEventNoParam)).image);
        }

        [MenuItem("Tools/Obvious/\ud83e\uddfc Soap/Event Debug Window")]
        private static void OpenEventDebugWindow() => Show();

        private void OnLostFocus()
        {
            GUI.FocusControl(null);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Find which events are calling a method in currently open scenes.");
            SoapInspectorUtils.DrawLine(2);

            EditorGUI.BeginChangeCheck();
            var textFieldStyle = new GUIStyle(GUI.skin.textField);
            _methodName = EditorGUILayout.TextField("Method Name", _methodName, textFieldStyle);
            if (EditorGUI.EndChangeCheck())
                _hasClicked = false;

            GUI.enabled = _methodName.Length > 0;
            if (SoapInspectorUtils.DrawCallToActionButton("Find", SoapInspectorUtils.ButtonSize.Large))
            {
                _hasClicked = true;
                var invocationCount = FindMethodInvocationCount(_methodName);
                _wasFound = invocationCount > 0;
            }
            GUI.enabled = true;

            if (!_hasClicked)
                return;

            DrawFeedbackText();
        }

        private void DrawFeedbackText()
        {
            var feedbackText = _methodName;
            var guiStyle = new GUIStyle(EditorStyles.label);
            guiStyle.normal.textColor = _wasFound ? Color.white : SoapEditorUtils.SoapColor;
            guiStyle.fontStyle = FontStyle.Bold;
            feedbackText += _wasFound ? " was found!" : " was not found!";
            EditorGUILayout.LabelField(feedbackText, guiStyle);
            if (_wasFound)
                EditorGUILayout.LabelField("Check the console for more details.", guiStyle);
        }

        private int FindMethodInvocationCount(string methodName)
        {
            var eventListeners = FindAllInOpenScenes<EventListenerBase>();
            var count = 0;
            foreach (var listener in eventListeners)
            {
                if (listener.ContainsCallToMethod(methodName))
                    count++;
            }

            return count;
        }

        private static List<T> FindAllInOpenScenes<T>()
        {
            var results = new List<T>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (!s.isLoaded)
                    continue;

                var allGameObjects = s.GetRootGameObjects();
                for (int j = 0; j < allGameObjects.Length; j++)
                {
                    var go = allGameObjects[j];
                    results.AddRange(go.GetComponentsInChildren<T>(true));
                }
            }

            return results;
        }
    }
}