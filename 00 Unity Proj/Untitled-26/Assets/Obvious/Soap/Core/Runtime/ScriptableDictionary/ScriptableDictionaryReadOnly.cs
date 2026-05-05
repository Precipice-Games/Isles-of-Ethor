using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Obvious.Soap
{
    [Serializable]
    public abstract class ScriptableDictionaryReadOnly<TKey, TValue, TDict> : ReadOnlyBase,
        IEnumerable<KeyValuePair<TKey, TValue>>
        where TDict : ScriptableDictionary<TKey, TValue>
    {
        [SerializeField] protected TDict _soapScriptable;

        public int Count => _soapScriptable.Count;
        public bool IsEmpty => _soapScriptable.IsEmpty;

        public IReadOnlyList<TKey> Keys => _soapScriptable.Keys.ToList().AsReadOnly();
        public IReadOnlyList<TValue> Values => _soapScriptable.Values.ToList().AsReadOnly();

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

        public event Action<TKey, TValue> OnItemAdded
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

        public event Action<TKey, TValue> OnItemRemoved
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

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _soapScriptable.TryGetValue(key, out value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _soapScriptable.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return _soapScriptable.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _soapScriptable.ContainsValue(value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _soapScriptable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}