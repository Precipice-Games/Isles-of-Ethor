#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Obvious.Soap
{
    /// <summary>
    /// Interface for objects that can be reset to their initial value.
    /// </summary>
    public interface IReset
    {
        void ResetValue();
#if UNITY_EDITOR
        void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange);
#endif
    }
}