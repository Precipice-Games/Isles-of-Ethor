using UnityEngine;
using UnityEngine.UI;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
namespace Obvious.Soap
{
    /// <summary>
    /// Binds an int variable to a slider
    /// </summary>
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/2_bindings/bindslider")]
    [AddComponentMenu("Soap/Bindings/BindSliderToInt")]
    [RequireComponent(typeof(Slider))]
    public class BindSliderInt : CacheComponent<Slider>
    {
        [SerializeField] private IntVariable _intVariable = null;
        [SerializeField] private bool _useMaxValueFromVariable = false;
#if ODIN_INSPECTOR
        [ShowIf("_useMaxValueFromVariable", false)]
#endif
        [SerializeField] private IntReference _maxValue = new IntReference(100, true);

        protected override void Awake()
        {
            base.Awake();
            OnValueChanged(_intVariable.Value);

            _component.onValueChanged.AddListener(SetBoundVariable);
            _intVariable.OnValueChanged += OnValueChanged;

            if (_useMaxValueFromVariable)
            {
                OnMaxValueChanged(_intVariable.MaxReference.Value);
                _intVariable.MaxReference.OnValueChanged += OnMaxValueChanged;
            }
            else
            {
                OnMaxValueChanged(_maxValue.Value);
                _maxValue.OnValueChanged += OnMaxValueChanged;
            }
        }

        private void OnDestroy()
        {
            _component.onValueChanged.RemoveListener(SetBoundVariable);
            _intVariable.OnValueChanged -= OnValueChanged;
            if (_useMaxValueFromVariable)
                _intVariable.MaxReference.OnValueChanged -= OnMaxValueChanged;
            else
                _maxValue.OnValueChanged -= OnMaxValueChanged;
        }

        private void OnValueChanged(int value)
        {
            _component.value = value;
        }

        private void OnMaxValueChanged(int value)
        {
            _component.maxValue = value;
        }

        private void SetBoundVariable(float value)
        {
            _intVariable.Value = Mathf.RoundToInt(value);
        }

        private void OnValidate()
        {
            if (_component == null)
                return;
            
            OnValueChanged(_intVariable.Value);
            
            if (_useMaxValueFromVariable)
                OnMaxValueChanged(_intVariable.MaxReference.Value);
            else
                OnMaxValueChanged(_maxValue.Value);
        }
    }
}