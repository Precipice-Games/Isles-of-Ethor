using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using Obvious.Soap.Attributes;
#endif
namespace Obvious.Soap
{
    [CreateAssetMenu(fileName = "scriptable_variable_float.asset", menuName = "Soap/ScriptableVariables/float")]
    public class FloatVariable : ScriptableVariable<float>
    {
        [Tooltip("Clamps the value of this variable to a minimum and maximum.")] 
#if ODIN_INSPECTOR
        [PropertyOrder(5)]
#endif
        [SerializeField] private bool _isClamped = false;
        public bool IsClamped
        {
            get => _isClamped;
            set => _isClamped = value;
        }
        [Tooltip("If clamped, sets the minimum and maximum")] 
        [SerializeField]
#if ODIN_INSPECTOR
        [ShowIf("_isClamped")]
        [PropertyOrder(6)][Indent]
#else
        [ShowIf("_isClamped")]
#endif
        private FloatReference _min = new FloatReference(0,true);
        public FloatReference MinReference
        {
            get => _min;
            set => _min = value;
        }
        public float Min => _min.Value;
        
        [Tooltip("If clamped, sets the minimum and maximum")] 
        [SerializeField]
#if ODIN_INSPECTOR
        [ShowIf("_isClamped")]
        [PropertyOrder(7)][Indent]
#else
        [ShowIf("_isClamped")]
#endif
        private FloatReference _max = new FloatReference(float.MaxValue,true);
        public FloatReference MaxReference
        {
            get => _max;
            set => _max = value;
        }
        public float Max => _max.Value;
        /// <summary>
        /// Returns the percentage of the value between the minimum and maximum.
        /// </summary>
        public float Ratio => Mathf.InverseLerp( _min.Value, _max.Value, Value);

        public override void Save()
        {
            PlayerPrefs.SetFloat(Guid, Value);
            base.Save();
        }

        public override void Load()
        {
            Value = PlayerPrefs.GetFloat(Guid, DefaultValue);
            base.Load();
        }

        public void Add(float value)
        {
            Value += value;
        }

        public override float Value
        {
            get => base.Value;
            set
            {
                var clampedValue = _isClamped ? Mathf.Clamp(value, _min.Value, _max.Value) : value;
                base.Value = clampedValue;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (_isClamped)
            {
                var clampedValue = Mathf.Clamp(Value, _min.Value, _max.Value);
                if (Value < clampedValue || Value > clampedValue)
                    Value = clampedValue;
            }

            base.OnValidate();
        }
#endif
    }
    
    [System.Serializable]
    public class FloatVariableReadOnly : ScriptableVariableReadOnly<FloatVariable,float>
    {
        public bool IsClamped => _soapScriptable.IsClamped;
        public float Min => _soapScriptable.Min;
        public FloatReference MinReference => _soapScriptable.MinReference;
        public FloatReference MaxReference => _soapScriptable.MaxReference;
        public float Max => _soapScriptable.Max;
        public float Ratio => _soapScriptable.Ratio;
    }
}