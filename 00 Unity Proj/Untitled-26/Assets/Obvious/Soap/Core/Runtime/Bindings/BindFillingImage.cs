using UnityEngine;
using UnityEngine.UI;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Obvious.Soap
{
    /// <summary>
    /// Binds a float variable to a filling image
    /// </summary>
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/2_bindings/bindfillingimage")]
    [AddComponentMenu("Soap/Bindings/BindFillingImage")]
    [RequireComponent(typeof(Image))]
    public class BindFillingImage : CacheComponent<Image>
    {
        [SerializeField] private FloatVariable _floatVariable = null;
        [Tooltip("If true, the max value is taken from the bound variable's max value")]
        [SerializeField] bool _useMaxValueFromVariable = false;
#if ODIN_INSPECTOR
        [HideIf("_useMaxValueFromVariable")]
#endif
        [SerializeField] private FloatReference _maxValue = new FloatReference(100,true);

        protected override void Awake()
        {
            base.Awake();
            _component.type = Image.Type.Filled;
            
            Refresh(_floatVariable);
            _floatVariable.OnValueChanged += Refresh;
            
            if (_useMaxValueFromVariable)
                _floatVariable.MaxReference.OnValueChanged += Refresh;
            else
                _maxValue.OnValueChanged += Refresh;
        }

        private void OnDestroy()
        {
            _floatVariable.OnValueChanged -= Refresh;
            if (_useMaxValueFromVariable)
                _floatVariable.MaxReference.OnValueChanged -= Refresh;
            else
                _maxValue.OnValueChanged -= Refresh;
        }

        private void Refresh(float currentValue)
        {
            var maxValue = _useMaxValueFromVariable ? _floatVariable.MaxReference.Value : _maxValue.Value;
            _component.fillAmount = _floatVariable.Value / maxValue;
        }
    }
}