#if UEE && !UEE_DISABLE_OBJECT_FIELD_DRAWER
using UnityEditor;
namespace Obvious.Soap.Editor
{
    [InitializeOnLoad]
    public static class UEEDisableObjectFieldDrawer
    {
        static UEEDisableObjectFieldDrawer()
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

#if UNITY_2023_1_OR_NEWER
            UnityEditor.Build.NamedBuildTarget buildTarget =
 UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            string currentDefinitions = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, currentDefinitions + ";UEE_DISABLE_OBJECT_FIELD_DRAWER");
#else
            string currentDefinitions = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,
                currentDefinitions + ";UEE_DISABLE_OBJECT_FIELD_DRAWER");
#endif
        }
    }
}
#endif