using UnityEngine;
using UnityEngine.Events;

namespace Obvious.Soap.Example
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private ScriptableListPlayer scriptableListPlayer = null;
        [SerializeField] private PlayerVariable _playerVariable = null;
        [SerializeField] private UnityEvent _onNotified = null;

        private void Awake()
        {
            _playerVariable.Value = this;
            scriptableListPlayer.TryAdd(this);
        }

        private void OnDestroy()
        {
            scriptableListPlayer.Remove(this);
        }

        public void Notify()
        {
            _onNotified?.Invoke();
        }
    }
}