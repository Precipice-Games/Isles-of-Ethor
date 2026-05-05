using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using Obvious.Soap.Attributes;
#endif

namespace Obvious.Soap
{
    public abstract class ScriptableVariable<T> : ScriptableVariableBase, IReset, IDrawObjectsInInspector
    {
        [Tooltip(
            "The value of the variable. This will be reset on play mode exit to the value it had before entering play mode.")]
        [SerializeField]
#if ODIN_INSPECTOR
        [HideInPlayMode][PropertyOrder(-1)]
#endif
        protected T _value;

        [Tooltip("Log in the console whenever this variable is changed, loaded or saved.")] [SerializeField]
        protected bool _debugLogEnabled;
#if ODIN_INSPECTOR
        [PropertyOrder(1)]
#endif
        [Tooltip("If true, saves the value to Player Prefs and loads it onEnable.")] [SerializeField]
        protected bool _saved;

        [Tooltip(
            "The default value of this variable. When loading from PlayerPrefs the first time, it will be set to this value.")]
        [SerializeField]
#if ODIN_INSPECTOR
        [ShowIf("_saved")]
        [PropertyOrder(2)]
        [Indent]
        [BoxGroup]
#else
        [ShowIf("_saved", true)]
#endif
        private T _defaultValue;

#if ODIN_INSPECTOR
        [PropertyOrder(5)]
#endif
        [Tooltip("Reset to initial value." +
                 " Scene Loaded : when the scene is loaded." +
                 " Application Start : Once, when the application starts." +
                 " None : Never.")]
        [SerializeField]
        private ResetTiming _resetOn = ResetTiming.OnSceneUnload;

        public override ResetTiming ResetTiming
        {
            get => _resetOn;
            set => _resetOn = value;
        }

        protected T _initialValue;

        [SerializeField]
#if ODIN_INSPECTOR
        [HideInEditorMode][PropertyOrder(-1)]
#endif
        protected T _runtimeValue;

        private readonly List<Object> _listenersObjects = new List<Object>();
        private Int32 _activeSceneHandle;
#if ODIN_INSPECTOR
        [HideInEditorMode]
        [ShowInInspector,EnableGUI]
        [PropertyOrder(100)]
        public IEnumerable<Object> ObjectsReactingToOnValueChangedEvent  => _listenersObjects;
#endif

        protected Action<T> _onValueChanged;

        /// <summary> Event raised when the variable value changes. </summary>
        public event Action<T> OnValueChanged
        {
            add
            {
                _onValueChanged += value;
#if UNITY_EDITOR
                var listener = value.Target as Object;
                AddListener(listener);
#endif
            }
            remove
            {
                _onValueChanged -= value;
#if UNITY_EDITOR
                var listener = value.Target as Object;
                RemoveListener(listener);
#endif
            }
        }

        /// <summary>
        /// The previous value just after the value changed.
        /// </summary>
        public T PreviousValue { get; private set; }

        /// <summary>
        /// The default value this variable is reset to. 
        /// </summary>
        public T DefaultValue => _defaultValue;

#if UNITY_EDITOR
        private T Read() => Application.isPlaying ? _runtimeValue : _value;
        private void Write(T v)
        {
            if (Application.isPlaying)
                _runtimeValue = v;
            else
                _value = v;
        }
#else
        private T Read() => _value;
        private void Write(T v) { _value = v; }
#endif

        /// <summary>
        /// Modify this to change the value of the variable.
        /// Triggers OnValueChanged event.
        /// </summary>
        public virtual T Value
        {
            get => Read();
            set
            {
                var current = Read();
                if (EqualityComparer<T>.Default.Equals(current, value))
                    return;

                Write(value);
                ValueChanged(true);
            }
        }

        public virtual void SetValueWithoutNotify(T value)
        {
            var current = Read();
            if (EqualityComparer<T>.Default.Equals(current, value))
                return;

            Write(value);
            ValueChanged(false);
        }

        public virtual void ValueChanged(bool triggerEvent = true)
        {
            if (_saved)
                Save();

            if (_debugLogEnabled)
            {
                var suffix = _saved ? " <color=#f75369>[Saved]</color>" : "";
                Debug.Log($"{GetColorizedString()}{suffix}", this);
            }

            if (triggerEvent)
                _onValueChanged?.Invoke(Read());

            PreviousValue = Read();
#if UNITY_EDITOR
            if (this != null) //for runtime variables, the instance will be destroyed so do not repaint.
                RepaintRequest?.Invoke();
#endif
        }

        public override Type GetGenericType => typeof(T);

        protected virtual void Awake()
        {
            //Prevents from resetting if no reference in a scene
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#else
            Init();
#endif
            if (_resetOn == ResetTiming.OnSceneUnload)
            {
                SceneManager.sceneUnloaded += OnSceneUnloaded;
                SceneManager.activeSceneChanged += OnActiveSceneChanged;
            }
        }
        
        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#endif
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        //Does not fire when a scene is loaded additively.
        private void OnActiveSceneChanged(Scene previousScene, Scene nextScene)
        {
            _activeSceneHandle = nextScene.handle;
        }

        //Fires just before Awake.
        private void OnSceneUnloaded(Scene scene)
        {
            if (_resetOn != ResetTiming.OnSceneUnload)
                return;
            
            if (_activeSceneHandle == 0 || _activeSceneHandle == scene.handle)
            { 
                if (_saved)
                    Load();
                else
                    ResetValue();
            }
        }

#if UNITY_EDITOR
        public void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.ExitingEditMode)
            {
                Init();
            }
            else if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                if (!_saved)
                {
                    ResetValue();
                }
                else
                {
                    if (EqualityComparer<T>.Default.Equals(_value, _runtimeValue))
                        return;

                    // Set the value to the runtime value
                    _value = _runtimeValue;
                    _runtimeValue = _initialValue;
                    EditorUtility.SetDirty(this);
                    RepaintRequest?.Invoke();
                }
            }
        }

        protected virtual void OnValidate()
        {
            //In default play mode, this get called before OnEnable(). Therefore a saved variable can get saved before loading. 
            //This check prevents the latter.
            if (EqualityComparer<T>.Default.Equals(Read(), PreviousValue))
                return;
            ValueChanged();
        }

        /// <summary> Reset the SO to default.</summary>
        internal override void Reset()
        {
            base.Reset();
            _listenersObjects.Clear();
            Value = default;
            _initialValue = default;
            _runtimeValue = default;
            PreviousValue = default;
            _saved = false;
            _resetOn = ResetTiming.OnSceneUnload;
            _debugLogEnabled = false;
        }

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

        private void Init()
        {
            if (_saved)
                Load();

            _initialValue = _value;
            _runtimeValue = _value;
            PreviousValue = _value;
            _listenersObjects.Clear();
        }

        /// <summary> Reset to initial value</summary>
        public void ResetValue()
        {
            Value = _initialValue;
            PreviousValue = _initialValue;
            _runtimeValue = _initialValue;
        }

        public virtual void Save()
        {
        }

        public virtual void Load()
        {
            PreviousValue = _value;

            if (_debugLogEnabled)
                Debug.Log($"{GetColorizedString()} <color=#f75369>[Loaded].</color>", this);
        }

        public override string ToString()
        {
            return $"{name} : {Value}";
        }

        protected virtual string GetColorizedString() => $"<color=#f75369>[Variable]</color> {ToString()}";

        public IReadOnlyList<Object> EditorListeners => _listenersObjects.AsReadOnly();

        public static implicit operator T(ScriptableVariable<T> variable) => variable.Value;
    }
}