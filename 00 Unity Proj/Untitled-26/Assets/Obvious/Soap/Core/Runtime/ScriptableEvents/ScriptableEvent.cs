using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Obvious.Soap
{
    public abstract class ScriptableEvent<T> : ScriptableEventBase, IDrawObjectsInInspector
    {
#if ODIN_INSPECTOR
        [HideInInspector]
#endif
        [Tooltip("Value used when raising the event in editor.")]
        [SerializeField] protected T _debugValue = default(T);

        private readonly List<EventListenerGeneric<T>> _eventListeners = new List<EventListenerGeneric<T>>();
        private readonly List<Object> _listenersObjects = new List<Object>();
        protected readonly List<Object> _cachedAllListeners = new List<Object>();
        protected Action<T> _onRaised = null;
        
#if ODIN_INSPECTOR
        [HideInEditorMode]
        [ShowInInspector,EnableGUI]
        public IEnumerable<Object> Listeners => EditorListeners;
#endif

        /// <summary>
        /// Action triggered when this event has been raised.
        /// Subscribe to this action if you want to bind methods to this event.
        /// </summary>
        public event Action<T> OnRaised
        {
            add
            {
                _onRaised += value;
#if UNITY_EDITOR
                var listener = value.Target as Object;
                AddListener(listener);
#endif
            }
            remove
            {
                _onRaised -= value;
#if UNITY_EDITOR
                var listener = value.Target as Object;
                RemoveListener(listener);
#endif
            }
        }

        public override Type GetGenericType => typeof(T);

#if ODIN_INSPECTOR
        [Button]
        [DisableInEditorMode]
#endif
        public virtual void Raise(T param)
        {
            if (!Application.isPlaying)
                return;

            for (var i = _eventListeners.Count - 1; i >= 0; i--)
                _eventListeners[i].OnEventRaised(this, param, _debugLogEnabled);

            _onRaised?.Invoke(param);

#if UNITY_EDITOR
            //As this uses reflection, I only allow it to be called in Editor.
            //If you want to display debug in builds, delete the #if UNITY_EDITOR
            if (_debugLogEnabled)
                Debug();
#endif
        }
        
        internal void RegisterListener(EventListenerGeneric<T> listener)
        {
            if (!_eventListeners.Contains(listener))
                _eventListeners.Add(listener);
        }
        
        internal void UnregisterListener(EventListenerGeneric<T> listener)
        {
            if (_eventListeners.Contains(listener))
                _eventListeners.Remove(listener);
        }

        public IReadOnlyList<Object> EditorListeners
        {
            get
            {
                _cachedAllListeners.Clear();
                _cachedAllListeners.AddRange(_eventListeners);
                _cachedAllListeners.AddRange(_listenersObjects);
                return _cachedAllListeners.AsReadOnly();
            }
        }
        
        protected virtual void Debug()
        {
            if (_onRaised == null)
                return;
            var delegates = _onRaised.GetInvocationList();
            foreach (var del in delegates)
            {
                var sb = new StringBuilder();
                sb.Append("<color=#f75369>[Event] </color>");
                sb.Append(name);
                sb.Append(" => ");
                sb.Append(del.GetMethodInfo().Name);
                sb.Append("()");
                var monoBehaviour = del.Target as MonoBehaviour;
                UnityEngine.Debug.Log(sb.ToString(), monoBehaviour?.gameObject);
            }
        }

        internal override void Reset()
        {
            base.Reset();
            _debugLogEnabled = false;
            _debugValue = default;
        }
        
#if UNITY_EDITOR
        public void AddListener(Object listener)
        {
            if (listener != null && !_listenersObjects.Contains(listener))
                _listenersObjects.Add(listener);
        }

        public void RemoveListener(Object listener)
        {
            if (_listenersObjects.Contains(listener))
                _listenersObjects.Remove(listener);
        }
#endif
    }
}