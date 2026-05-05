using UnityEngine;
using UnityEditor;

namespace Obvious.Soap.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyBase), true)]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty _soapScriptable;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_soapScriptable == null)
            {
                _soapScriptable = property.FindPropertyRelative("_soapScriptable");
            }

            EditorGUI.PropertyField(position, _soapScriptable, label);
        }
    }
}