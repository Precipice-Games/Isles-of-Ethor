using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Obvious.Soap
{
    public abstract class ScriptableList<T> : ScriptableListBase, IList<T>, IDrawObjectsInInspector
    {
        [SerializeField] protected List<T> _list = new List<T>();
        private readonly Dictionary<T, int> _itemCounts = new Dictionary<T, int>();
        
        public override int Count => _list.Count;
        public bool IsReadOnly => false;
        public bool IsEmpty => _list.Count == 0;
        public override Type GetGenericType => typeof(T);

        /// <summary>
        /// Indexer: Access an item in the list by index.
        /// </summary>
        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        /// <summary> Event raised when an item is added or removed from the list. </summary>
        [Obsolete("Use Modified instead")]
        public event Action OnItemCountChanged;

        /// <summary> Event raised  when an item is added to the list. </summary>
        public event Action<T> OnItemAdded;

        /// <summary> Event raised  when an item is removed from the list. </summary>
        public event Action<T> OnItemRemoved;

        /// <summary> Event raised  when multiple item are added to the list. </summary>
        public event Action<IEnumerable<T>> OnItemsAdded;

        /// <summary> Event raised  when multiple items are removed from the list. </summary>
        public event Action<IEnumerable<T>> OnItemsRemoved;
        
        public int IndexOf(T item) => _list.IndexOf(item);
        public bool Contains(T item) => _itemCounts.ContainsKey(item);

        /// <summary>
        /// Adds an item to the list.
        /// Raises OnItemCountChanged and OnItemAdded event.
        /// </summary>
        public void Add(T item)
        {
            _list.Add(item);
            TryAddItemToDictionary(item);
            RaiseItemAddedEvents(item);
        }

        /// <summary>
        /// Adds an item to the list only if it's not in the list.
        /// If success, raises OnItemCountChanged and OnItemAdded event.
        /// </summary>
        public bool TryAdd(T item)
        {
            if (_itemCounts.ContainsKey(item))
                return false;
            
            Add(item);
            return true;
        }

        /// <summary>
        /// Inserts an item at the specified index.
        /// </summary>
        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            TryAddItemToDictionary(item);
            RaiseItemAddedEvents(item);
        }
        
        /// <summary>
        /// Adds a range of items to the list.
        /// Raises OnItemCountChanged and OnItemsAdded event once, after all items have been added.
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            var collection = items.ToArray();
            if (collection.Length == 0)
                return;

            _list.AddRange(collection);
            foreach (var item in collection)
            {
                TryAddItemToDictionary(item);
            }

            RaiseItemsAddedEvents(collection);
        }

        /// <summary>
        /// Adds a range of items to the list. An item is only added if its not in the list.
        /// Raises OnItemCountChanged and OnItemsAdded event once, after all items have been added.
        /// </summary>
        public bool TryAddRange(IEnumerable<T> items)
        {
            var uniqueItems = items.Where(item => !_itemCounts.ContainsKey(item)).ToList();
            if (uniqueItems.Count > 0)
            {
                AddRange(uniqueItems);
                return true;
            }
            return false;
        }
        
        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes an item from the list only if it's in the list.
        /// If Success, raises OnItemCountChanged and OnItemRemoved event.
        /// </summary>
        /// <param name="item"></param>
        public bool Remove(T item)
        {
            if (!_itemCounts.ContainsKey(item))
                return false;
            
            _list.Remove(item);
            _itemCounts[item]--;
            if (_itemCounts[item] == 0)
                _itemCounts.Remove(item);
            
            RaiseItemRemovedEvents(item);
            return true;
        }
        
        bool ICollection<T>.Remove(T item)
        {
            return Remove(item);
        }

        /// <summary>
        /// Removes an item from the list at a specific index.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            var item = _list[index];
            _list.RemoveAt(index);
            _itemCounts[item]--;
            if (_itemCounts[item] == 0)
                _itemCounts.Remove(item);
            
            RaiseItemRemovedEvents(item);
        }

        /// <summary>
        /// Removes a range of items from the list.
        /// Raises OnItemCountChanged and OnItemsAdded event once, after all items have been added.
        /// </summary>
        /// <param name="index">Starting Index</param>
        /// <param name="count">Amount of Items</param>
        public bool RemoveRange(int index, int count)
        {
            if (index < 0 || count < 0 || index + count > _list.Count)
                return false;
            
            var itemsToRemove = _list.GetRange(index, count);
            foreach (var item in itemsToRemove)
            {
                _itemCounts[item]--;
                if (_itemCounts[item] == 0)
                    _itemCounts.Remove(item);
            }
            
            _list.RemoveRange(index, count);
            RaiseItemsRemovedEvents(itemsToRemove);
            return true;
        }
      
        public override void Clear()
        {
            _itemCounts.Clear();
            _list.Clear();
            base.Clear();
#if UNITY_EDITOR
            RepaintRequest?.Invoke();
#endif
        }
        
        private void TryAddItemToDictionary(T item)
        {
            if (_itemCounts.ContainsKey(item))
                _itemCounts[item]++;
            else
                _itemCounts[item] = 1;
        }
        
        private void RaiseItemAddedEvents(T item)
        {
            OnItemCountChanged?.Invoke();
            OnItemAdded?.Invoke(item);
            Modified?.Invoke();
#if UNITY_EDITOR
            RepaintRequest?.Invoke();
#endif
        }
        
        private void RaiseItemsAddedEvents(IEnumerable<T> items)
        {
            OnItemCountChanged?.Invoke();
            OnItemsAdded?.Invoke(items);
            Modified?.Invoke();
#if UNITY_EDITOR
            RepaintRequest?.Invoke();
#endif
        }

        private void RaiseItemRemovedEvents(T item)
        {
            OnItemCountChanged?.Invoke();
            OnItemRemoved?.Invoke(item);
            Modified?.Invoke();
#if UNITY_EDITOR
            RepaintRequest?.Invoke();
#endif
        }

        private void RaiseItemsRemovedEvents(IEnumerable<T> items)
        {
            OnItemCountChanged?.Invoke();
            OnItemsRemoved?.Invoke(items);
            Modified?.Invoke();
#if UNITY_EDITOR
            RepaintRequest?.Invoke();
#endif
        }
        
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void ForEach(Action<T> action)
        {
            for (var i = _list.Count - 1; i >= 0; i--)
                action(_list[i]);
        }

        public IReadOnlyList<Object> EditorListeners => _list.OfType<Object>().ToList().AsReadOnly();
        
        public override bool CanBeSerialized()
        {
            var canBeSerialized = SoapUtils.IsUnityType(typeof(T)) || 
                                  SoapUtils.IsSerializable(typeof(T));
            return canBeSerialized;
        }
    }
}