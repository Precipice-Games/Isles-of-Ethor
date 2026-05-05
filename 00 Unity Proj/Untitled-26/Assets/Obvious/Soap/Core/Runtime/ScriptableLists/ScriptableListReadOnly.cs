using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obvious.Soap
{
    [Serializable]
    public abstract class ScriptableListReadOnly<V, T> : ReadOnlyBase, IEnumerable<T> where V : ScriptableList<T>
    {
        [SerializeField] protected V _soapScriptable;

        public int Count => _soapScriptable.Count;
        public bool IsEmpty => _soapScriptable.IsEmpty;

        public T this[int index]
        {
            get => _soapScriptable[index];
        }

        public event Action Modified
        {
            add
            {
                if (_soapScriptable != null)
                    _soapScriptable.Modified += value;
            }
            remove
            {
                if (_soapScriptable != null)
                    _soapScriptable.Modified -= value;
            }
        }

        public event Action<T> OnItemAdded
        {
            add
            {
                if (_soapScriptable != null)
                    _soapScriptable.OnItemAdded += value;
            }
            remove
            {
                if (_soapScriptable != null)
                    _soapScriptable.OnItemAdded -= value;
            }
        }

        public event Action<T> OnItemRemoved
        {
            add
            {
                if (_soapScriptable != null)
                    _soapScriptable.OnItemRemoved += value;
            }
            remove
            {
                if (_soapScriptable != null)
                    _soapScriptable.OnItemRemoved -= value;
            }
        }
        
        public event Action<IEnumerable<T>> OnItemsAdded
        {
            add
            {
                if (_soapScriptable != null)
                    _soapScriptable.OnItemsAdded += value;
            }
            remove
            {
                if (_soapScriptable != null)
                    _soapScriptable.OnItemsAdded -= value;
            }
        }
        
        public event Action<IEnumerable<T>> OnItemsRemoved
        {
            add
            {
                if (_soapScriptable != null)
                    _soapScriptable.OnItemsRemoved += value;
            }
            remove
            {
                if (_soapScriptable != null)
                    _soapScriptable.OnItemsRemoved -= value;
            }
        }

        public IEnumerator<T> GetEnumerator() => _soapScriptable.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
      

        public int IndexOf(T item) => _soapScriptable.IndexOf(item);
        public bool Contains(T item) => _soapScriptable.Contains(item);

        public void ForEach(Action<T> action)
        {
            for (var i = _soapScriptable.Count - 1; i >= 0; i--)
                action(_soapScriptable[i]);
        }
    }
}