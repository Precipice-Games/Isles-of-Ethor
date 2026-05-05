using System;
using UnityEngine;

namespace Obvious.Soap
{
    [HelpURL("https://obvious-game.gitbook.io/soap/soap-core-assets/scriptable-save")]
    public abstract class ScriptableSaveBase : ScriptableBase
    {
        //Override these methods to implement your own save/load/delete logic
        public abstract void Save();
        public abstract void Load();
        public abstract void Delete();
        
        /// <summary>
        /// Called by the ScriptableObjectUpdateSystem if SaveMode is set to Interval.
        /// Equivalent to MonoBehaviour.Update().
        /// </summary>
        public abstract void Update();
        
        public enum ELoadMode
        {
            Automatic,
            Manual
        }
        
        public enum ESaveMode
        {
            Manual,
            Interval
        }
    }
}