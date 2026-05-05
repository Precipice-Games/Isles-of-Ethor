using System;
using System.Collections.Generic;
using UnityEngine;

namespace Obvious.Soap
{
    //Unity cannot serialize generic types, so we need to create a non-generic base class
    public abstract class VariableReferenceBase { }
    
    [Serializable]
    public abstract class VariableReference<V, T> :  VariableReferenceBase, ISerializationCallbackReceiver where V : ScriptableVariable<T>
    {
        public bool UseLocal;
        public T LocalValue;
        public V Variable;

        private Action<T> _onValueChanged;

        [NonSerialized] private T _lastSerializedLocalValue;

        public T Value
        {
            get
            {
                if (UseLocal)
                    return LocalValue;
                
                if (Variable == null)
                    return default;
                return Variable.Value;
            }
            set
            {
                if (UseLocal)
                {
                    if (Equals(LocalValue, value))
                        return;

                    LocalValue = value;
                    _onValueChanged?.Invoke(LocalValue);
                }
                else
                {
                    Variable.Value = value;
                }
            }
        }

        /// <summary> Event raised when the variable reference value changes.
        /// Is triggered for both local and variable. </summary>
        public event Action<T> OnValueChanged
        {
            add
            {
                _onValueChanged += value;
                if (Variable != null)
                    Variable.OnValueChanged += value;
            }
            remove
            {
                _onValueChanged -= value;
                if (Variable != null)
                    Variable.OnValueChanged -= value;
            }
        }

        public static implicit operator T(VariableReference<V, T> reference)
        {
            return reference.Value;
        }

        public void OnBeforeSerialize()
        {
            _lastSerializedLocalValue = LocalValue;
        }

        public void OnAfterDeserialize()
        {
            if (UseLocal && !EqualityComparer<T>.Default.Equals(_lastSerializedLocalValue, LocalValue))
            {
                _onValueChanged?.Invoke(LocalValue);
            }
        }
    }
}