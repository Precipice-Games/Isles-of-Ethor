using UnityEditor;
using UnityEngine;

#if !ODIN_INSPECTOR
namespace Obvious.Soap.Editor
{
    [CustomEditor(typeof(ScriptableListBase), true)]
    public class ScriptableListDrawer : UnityEditor.Editor
    {
        private ScriptableBase _scriptableBase = null;
        private ScriptableListBase _scriptableListBase;
        private static bool _repaintFlag;

        public override void OnInspectorGUI()
        {
            if (_scriptableListBase == null)
                _scriptableListBase = target as ScriptableListBase;

            var isMonoBehaviourOrGameObject = _scriptableListBase.GetGenericType.IsSubclassOf(typeof(MonoBehaviour))
                                              || _scriptableListBase.GetGenericType == typeof(GameObject);
            if (isMonoBehaviourOrGameObject)
            {
                SoapInspectorUtils.DrawPropertiesExcluding(serializedObject, new[] { "_list" });
            }
            else
            {
                //we still want to display the native list for non MonoBehaviors (like SO for examples)
                DrawDefaultInspector();

                //Check for Serializable
                var genericType = _scriptableListBase.GetGenericType;
                var canBeSerialized = _scriptableListBase.CanBeSerialized();
                if (!canBeSerialized)
                {
                    SoapInspectorUtils.DrawSerializationError(genericType);
                    return;
                }
            }

            if (!EditorApplication.isPlaying)
                return;

            SoapInspectorUtils.DrawLine();
            DisplayAllObjects();
        }

        private void DisplayAllObjects()
        {
            var container = (IDrawObjectsInInspector)target;
            var gameObjects = container.EditorListeners;
            var title = $"List Count : {_scriptableListBase.Count}";
            EditorGUILayout.LabelField(title);
            foreach (var obj in gameObjects)
                EditorGUILayout.ObjectField(obj, typeof(Object), true);
        }

        #region Repaint

        private void OnEnable()
        {
            if (_repaintFlag)
                return;

            _scriptableBase = target as ScriptableBase;
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
                if (_scriptableBase == null)
                    _scriptableBase = target as ScriptableBase;
                _scriptableBase.RepaintRequest += OnRepaintRequested;
            }
            else if (obj == PlayModeStateChange.EnteredEditMode)
            {
                _scriptableBase.RepaintRequest -= OnRepaintRequested;
            }
        }

        private void OnRepaintRequested() => Repaint();
    }

    #endregion
}
#endif