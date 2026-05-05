using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Attributes
{
    [CustomPropertyDrawer(typeof(EndDisabledGroup))]
    public class EndDisabledGroupDrawer : DecoratorDrawer
    {
        public override float GetHeight() => 0;

        public override void OnGUI(Rect position)
        {
            EditorGUI.EndDisabledGroup();
        }
    }
}