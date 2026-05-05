using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Obvious.Soap
{
    /// <summary>
    /// Base classes of all ScriptableObjects in Soap
    /// </summary>
#if ODIN_INSPECTOR
    public abstract class ScriptableBase : SerializedScriptableObject
#else
    public abstract class ScriptableBase : ScriptableObject
#endif
    {
        internal virtual void Reset()
        {
            TagIndex = 0;
            Description = "";
        }
        [HideInInspector]
        public Action RepaintRequest;
        [HideInInspector]
        public int TagIndex = 0;
        [HideInInspector]
        public string Description = "";
        public virtual Type GetGenericType { get; }
    }
}