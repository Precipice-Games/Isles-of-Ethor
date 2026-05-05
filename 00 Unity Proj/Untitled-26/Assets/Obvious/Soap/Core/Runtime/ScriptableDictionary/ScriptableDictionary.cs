using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Obvious.Soap
{
    public abstract class ScriptableDictionary<T, V> : ScriptableDictionaryBase, IDictionary<T, V>
    {
        [SerializeField] protected Dictionary<T, V> _dictionary = new Dictionary<T, V>();
        
        public override int Count => _dictionary.Count;
        public bool IsReadOnly => false;
        public bool IsEmpty => _dictionary.Count == 0;
        
        /// <summary> Event raised  when an item is added to the list. </summary>
        public event Action<T, V> OnItemAdded;

        /// <summary> Event raised  when an item is removed from the list. </summary>
        public event Action<T, V> OnItemRemoved;
        
        public V this[T key]
        {
            get => _dictionary[key];
            set
            {
                _dictionary[key] = value;
                Modified?.Invoke();
            }
        }

        public ICollection<T> Keys => _dictionary.Keys;
        public ICollection<V> Values => _dictionary.Values;
        public override Type GetGenericType => typeof(T);

       
        /// <summary>
        /// Adds an item to the dictionary.
        /// Raises OnItemAdded and OnModified event.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<T, V> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Adds a key and value to the dictionary.
        /// Raises OnItemAdded and OnModified event.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(T key, V value)
        {
            _dictionary.Add(key, value);
            OnItemAdded?.Invoke(key, value);
            Modified?.Invoke();
#if UNITY_EDITOR
            RepaintRequest?.Invoke();
#endif
        }

        /// <summary>
        /// Checks if the dictionary contains a key.
        /// Then adds the key and value to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>True if succeeded</returns>
        public bool TryAdd(T key, V value)
        {
            if (_dictionary.ContainsKey(key))
            {
                return false;
            }

            Add(key, value);
            return true;
        }
        
        /// <summary>
        /// Removes an item from the dictionary.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<T, V> item)
        {
            return Remove(item.Key);
        }

        /// <summary>
        /// Tries to Remove an item from the dictionary using a key.
        /// If Success, raises OnItemRemoved and OnModified event.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(T key)
        {
            if (!_dictionary.TryGetValue(key, out var value))
                return false;
            var removedFromList = _dictionary.Remove(key);
            if (removedFromList)
            {
                OnItemRemoved?.Invoke(key,value);
                Modified?.Invoke();
#if UNITY_EDITOR
                RepaintRequest?.Invoke();
#endif
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to get a value from the dictionary using a key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(T key, out V value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public override void Clear()
        {
            _dictionary.Clear();
            base.Clear();
#if UNITY_EDITOR
            RepaintRequest?.Invoke();
#endif
        }

        public bool Contains(KeyValuePair<T, V> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<T, V>[] array, int arrayIndex)
        {
            var i = arrayIndex;
            foreach (var pair in _dictionary)
            {
                array[i] = pair;
                i++;
            }
        }

        public bool ContainsKey(T key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool ContainsValue(V value)
        {
            return _dictionary.ContainsValue(value);
        }
        
        public IEnumerator<KeyValuePair<T, V>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public override bool CanBeSerialized()
        {
            var canKeyBeSerialized = SoapUtils.IsUnityType(typeof(T)) || 
                                  SoapUtils.IsSerializable(typeof(T));
            var canValueBeSerialized = SoapUtils.IsUnityType(typeof(V)) ||
                                       SoapUtils.IsSerializable(typeof(V));
            var canBeSerialized = canKeyBeSerialized && canValueBeSerialized;
            return canBeSerialized;
        }
    }
}