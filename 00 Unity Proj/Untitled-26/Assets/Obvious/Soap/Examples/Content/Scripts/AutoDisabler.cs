using UnityEngine;

namespace Obvious.Soap.Example
{


    public class AutoDisabler : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.5f;
      
        public void OnEnable()
        {
            Invoke(nameof(Disable),_duration);
        }

        private void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}