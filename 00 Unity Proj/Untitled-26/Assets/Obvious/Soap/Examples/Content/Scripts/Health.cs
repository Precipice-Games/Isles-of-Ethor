using JetBrains.Annotations;
using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/1_scriptablevariables")]
    public class Health : MonoBehaviour
    {
        [SerializeField] private FloatVariable _currentHealth = null;
        [SerializeField] private FloatReference _maxHealth = null;

        [Header("Scriptable Events")] [SerializeField] private ScriptableEventInt _onPlayerDamaged = null;
        [SerializeField] private ScriptableEventInt _onPlayerHealed = null;
        [SerializeField] private ScriptableEventNoParam _onPlayerDeath = null;

        private bool _isDead = false;

        private void Start()
        {
            _currentHealth.Value = _maxHealth.Value;
            _currentHealth.OnValueChanged += OnHealthChanged;
        }

        private void OnDestroy()
        {
            _currentHealth.OnValueChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float newValue)
        {
            var diff = newValue - _currentHealth.PreviousValue;
            if (diff < 0)
            {
                OnDamaged(Mathf.Abs(diff));
            }
            else
            {
                OnHealed(diff);
            }
        }

        private void OnDamaged(float value)
        {
            if (_currentHealth <= 0f && !_isDead)
                OnDeath();
            else
                _onPlayerDamaged.Raise(Mathf.RoundToInt(value));
        }

        private void OnHealed(float value)
        {
            _onPlayerHealed.Raise(Mathf.RoundToInt(value));
        }

        private void OnDeath()
        {
            _onPlayerDeath.Raise();
            _isDead = true;
        }

        //if you don't want to modify directly the health, you can also do it like this
        //Used in the Event example.
        [UsedImplicitly]
        public void TakeDamage(int amount)
        {
            _currentHealth.Add(-amount);
        }
    }
}