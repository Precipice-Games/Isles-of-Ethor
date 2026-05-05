using UnityEditor;

namespace Obvious.Soap.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableSingleton<>), true)]
    public class ScriptableSingletonPropertyDrawer : ScriptableBasePropertyDrawer
    {
        //inherit from and customize this drawer to fit your enums needs
        protected override float WidthRatio => 1;
    }
}