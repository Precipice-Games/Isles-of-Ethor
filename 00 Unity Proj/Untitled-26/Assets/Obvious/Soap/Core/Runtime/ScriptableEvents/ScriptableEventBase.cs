using UnityEngine;

namespace Obvious.Soap
{
    [HelpURL("https://obvious-game.gitbook.io/soap/soap-core-assets/scriptable-event")]
    public abstract class ScriptableEventBase : ScriptableBase
    {
        [Tooltip("Enable console logs when this event is raised.")]
        [SerializeField] 
        protected bool _debugLogEnabled = false;
        public bool DebugLogEnabled => _debugLogEnabled;
    }
}