using Obvious.Soap.Attributes;
using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/soap-core-assets/scriptable-variable/runtime-variables")]
    public class RuntimeInjectedHealth : MonoBehaviour
    {
        [Tooltip("This field will be injected at runtime")] 
        [SerializeField]
        [RuntimeInjectable("hp")] 
        private FloatVariable _runtimeHpVariable;

        [SerializeField] private FloatReference _maxHealth;
        
        private void Start()
        {
            _runtimeHpVariable.Value = _maxHealth.Value;
            _runtimeHpVariable.OnValueChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            _runtimeHpVariable.OnValueChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float newValue)
        {
            if (newValue <= 0f)
                gameObject.SetActive(false);
        }

        //In this example, this is called when the enemy collides with the Player.
        public void TakeDamage(int amount) => _runtimeHpVariable.Add(-amount);
    }
}