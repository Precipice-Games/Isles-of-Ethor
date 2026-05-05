#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Obvious.Soap
{
    public abstract class ResettableScriptableObject : ScriptableObject
    {
        protected string _cachedJson;

#if UNITY_EDITOR
        protected virtual void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        protected virtual void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        protected virtual void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.ExitingEditMode)
            {
                CacheState();
            }
            else if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                ResetValue();
            }
        }

        protected virtual void CacheState()
        {
            _cachedJson = EditorJsonUtility.ToJson(this, prettyPrint: true);
        }
        
        [ContextMenu("Reset Value")]
        public virtual void ResetValue()
        {
            if (!string.IsNullOrEmpty(_cachedJson))
            {
                EditorJsonUtility.FromJsonOverwrite(_cachedJson, this);
            }
        }
#endif
    }
}