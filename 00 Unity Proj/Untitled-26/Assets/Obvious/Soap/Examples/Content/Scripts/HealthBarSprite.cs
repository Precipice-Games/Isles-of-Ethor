using Obvious.Soap.Attributes;
using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/8_runtimevariables")]
    public class HealthBarSprite : MonoBehaviour
    {
        [BeginDisabledGroup]
        [SerializeField] private FloatVariable _runtimeHpVariable;
        [SerializeField] private Transform _fillTransform = null;

        //The variable is manually injected by another script (RuntimeHealth in this case).
        public void Init(FloatVariable runtimeVariable)
        {
            _runtimeHpVariable = runtimeVariable;
            _runtimeHpVariable.OnValueChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            _runtimeHpVariable.OnValueChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float currentHpValue)
        {
            var hpRatio = Mathf.Clamp01(currentHpValue / _runtimeHpVariable.Max);
            _fillTransform.localPosition = new Vector3((-1 + hpRatio) * 0.5f, 0,0);
            _fillTransform.localScale = new Vector3(hpRatio,1,1);
        }
    }
}