using UnityEngine;

namespace Obvious.Soap
{
    [HelpURL("https://obvious-game.gitbook.io/soap/soap-core-assets/scriptable-singleton")]
    public abstract class ScriptableSingleton<T> : ScriptableBase where T : ScriptableObject
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance)
                    return _instance;

                var assetName = typeof(T).Name;
                _instance = Resources.Load<T>(assetName);

                if (_instance)
                    return _instance;

                var all = Resources.LoadAll<T>(string.Empty);
                if (all != null && all.Length > 0)
                {
                    _instance = all[0];
                    return _instance;
                }

                Debug.LogError("[Soap] No instance of Singleton:" + assetName + " found in Resources folder.");
                return null;
            }
        }

        /// <summary>
        /// Need for editor if direct access from classes (without passing through Instance property)
        /// </summary>
        protected virtual void OnEnable()
        {
            if (_instance == null)
                _instance = this as T;
        }
    }
}