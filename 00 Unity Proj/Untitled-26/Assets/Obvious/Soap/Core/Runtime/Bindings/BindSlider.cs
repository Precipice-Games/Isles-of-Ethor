using UnityEngine;
using UnityEngine.UI;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
namespace Obvious.Soap
{
    /// <summary>
    /// Binds a float variable to a slider
    /// </summary>
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/2_bindings/bindslider")]
    [AddComponentMenu("Soap/Bindings/BindSlider")]
    [RequireComponent(typeof(Slider))]
    public class BindSlider : CacheComponent<Slider>
    {
        [SerializeField] private FloatVariable _floatVariable = null;
        [Tooltip("If true, the max value is taken from the bound variable's max value")]
        [SerializeField] private bool _useMaxValueFromVariable = false;
#if ODIN_INSPECTOR
        [HideIf("_useMaxValueFromVariable")]
#endif
        [SerializeField] private FloatReference _maxValue = new FloatReference(100, true);

        protected override void Awake()
        {
            base.Awake();
            OnValueChanged(_floatVariable);

            _component.onValueChanged.AddListener(SetBoundVariable);
            _floatVariable.OnValueChanged += OnValueChanged;

            if (_useMaxValueFromVariable)
            {
                OnMaxValueChanged(_floatVariable.MaxReference.Value);
                _floatVariable.MaxReference.OnValueChanged += OnMaxValueChanged;
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
            _floatVariable.OnValueChanged -= OnValueChanged;
            if (_useMaxValueFromVariable)
                _floatVariable.MaxReference.OnValueChanged -= OnMaxValueChanged;
            else
                _maxValue.OnValueChanged -= OnMaxValueChanged;
        }

        private void OnValueChanged(float value)
        {
            _component.value = value;
        }

        private void OnMaxValueChanged(float value)
        {
            _component.maxValue = value;
        }

        private void SetBoundVariable(float value)
        {
            _floatVariable.Value = value;
        }
        
        private void OnValidate()
        {
            if (_component == null)
                return;
            
            OnValueChanged(_floatVariable.Value);
            
            if (_useMaxValueFromVariable)
                OnMaxValueChanged(_floatVariable.MaxReference.Value);
            else
                OnMaxValueChanged(_maxValue.Value);
        }
    }
}