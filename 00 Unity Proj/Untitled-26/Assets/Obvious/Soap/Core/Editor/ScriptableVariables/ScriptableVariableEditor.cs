using System.Collections.Generic;
using UnityEditor;

namespace Obvious.Soap.Editor
{
    [CustomEditor(typeof(ScriptableVariableBase), true)]
#if ODIN_INSPECTOR
    public class ScriptableVariableEditor : Sirenix.OdinInspector.Editor.OdinEditor
#else
    public abstract class ScriptableVariableEditor: UnityEditor.Editor
#endif
    {
        protected bool _isReadOnly;
        
        public void SetIsReadOnly(bool isReadOnly)
        {
            _isReadOnly = isReadOnly;
        }
#if ODIN_INSPECTOR
        private ScriptableVariableBase _scriptableVariable;
        private System.Type _genericType;
        
        private static readonly HashSet<string> s_propertiesToHide = new HashSet<string>
        {
            "m_Script",
            "_guid",
            "_saveGuid",
        };

        public override void OnInspectorGUI()
        {
            if (_isReadOnly)
            {
                DrawSoapCustom();
            }
            else
            {
                DrawDefaultInspector();
            }
        }

        private void DrawSoapCustom()
        {
            if (_scriptableVariable == null)
            {
                _scriptableVariable = target as ScriptableVariableBase;
                _genericType = _scriptableVariable.GetGenericType;
            }
            serializedObject.DrawOnlyField("m_Script", true);
            serializedObject.DrawCustomInspector(s_propertiesToHide, _genericType, _isReadOnly);
        }
#endif
    }
}