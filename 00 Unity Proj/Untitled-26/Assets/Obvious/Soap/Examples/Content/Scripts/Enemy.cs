using UnityEngine;
using UnityEngine.Events;

namespace Obvious.Soap.Example
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onHitPlayerEvent = null;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                _onHitPlayerEvent?.Invoke();
        }
    }
}