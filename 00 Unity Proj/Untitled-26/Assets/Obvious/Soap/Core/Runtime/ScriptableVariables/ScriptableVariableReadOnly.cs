using System;
using UnityEngine;

namespace Obvious.Soap
{
    [Serializable]
    public abstract class ScriptableVariableReadOnly<V, T> : ReadOnlyBase where V : ScriptableVariable<T>
    {
        [SerializeField] protected V _soapScriptable;

        public T Value
        {
            get
            {
                if (_soapScriptable == null)
                    return default;
                return _soapScriptable.Value;
            }
        }

        /// <summary>
        /// Event raised when the variable reference value changes.
        /// </summary>
        public event Action<T> OnValueChanged
        {
            add
            {
                if (_soapScriptable != null)
                    _soapScriptable.OnValueChanged += value;
            }
            remove
            {
                if (_soapScriptable != null)
                    _soapScriptable.OnValueChanged -= value;
            }
        }

        public static implicit operator T(ScriptableVariableReadOnly<V, T> reference)
        {
            return reference.Value;
        }
    }
}