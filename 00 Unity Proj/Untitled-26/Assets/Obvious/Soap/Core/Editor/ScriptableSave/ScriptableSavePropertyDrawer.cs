using UnityEditor;

namespace Obvious.Soap.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableSaveBase), true)]
    public class ScriptableSavePropertyDrawer : ScriptableBasePropertyDrawer
    {
        //inherit from and customize this drawer to fit your enums needs
        protected override float WidthRatio => 1;
    }
}