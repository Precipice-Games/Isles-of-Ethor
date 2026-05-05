using UnityEditor;
using UnityEngine;

namespace Obvious.Soap.Attributes
{
    [CustomPropertyDrawer(typeof(BeginDisabledGroup))]
    public class BeginDisabledGroupDrawer : DecoratorDrawer
    {
        public override float GetHeight() => 0;

        public override void OnGUI(Rect position)
        {
            EditorGUI.BeginDisabledGroup(true);
        }
    }
}