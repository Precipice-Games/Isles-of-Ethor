using UnityEditor;

namespace Obvious.Soap.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableEnumBase), true)]
    public class ScriptableEnumPropertyDrawer : ScriptableBasePropertyDrawer
    {
        //inherit from and customize this drawer to fit your enums needs
        protected override float WidthRatio => 1;
    }
}