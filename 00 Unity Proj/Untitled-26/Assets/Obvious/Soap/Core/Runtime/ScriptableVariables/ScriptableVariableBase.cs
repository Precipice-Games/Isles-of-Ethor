using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Obvious.Soap
{
    [HelpURL("https://obvious-game.gitbook.io/soap/soap-core-assets/scriptable-variable")]
    public abstract class ScriptableVariableBase : ScriptableBase
    {
#if ODIN_INSPECTOR
        [PropertyOrder(4)]
        [DisableIf("_saveGuid", SaveGuidType.Auto)]
        [ShowIf("_saved")]
        [Indent]
        [BoxGroup]
#endif
        [SerializeField]
        private string _guid;

#if ODIN_INSPECTOR
        [PropertyOrder(3)]
        [ShowIf("_saved")]
        [Indent]
        [BoxGroup]
        //[LabelWidth(80)]
#endif
        [Tooltip("ID used as the Player Prefs Key.\n" +
                 "Auto: Guid is generated automatically base on the asset path.\n" +
                 "Manual: Guid can be overwritten manually.")]
        [SerializeField]
        private SaveGuidType _saveGuid;

        public SaveGuidType SaveGuid => _saveGuid;

        /// <summary>
        /// Guid is needed to save/load the value to PlayerPrefs.
        /// </summary>
        public string Guid
        {
            get => _guid;
            set => _guid = value;
        }

        public virtual ResetTiming ResetTiming { get; set; }
        
    }

    public enum SaveGuidType
    {
        Auto,
        Manual,
    }
}