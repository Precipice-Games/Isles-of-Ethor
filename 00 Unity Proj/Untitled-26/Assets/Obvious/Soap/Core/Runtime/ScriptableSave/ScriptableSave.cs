using System;
using System.Diagnostics;
using System.IO;
using Obvious.Soap.Attributes;
using UnityEngine;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Obvious.Soap
{
    public abstract class ScriptableSave<T> : ScriptableSaveBase where T : class, new()
    {
        [SerializeField] protected bool _debugLogEnabled = false;
        [SerializeField] protected ELoadMode _loadMode = ELoadMode.Automatic;
        public ELoadMode LoadMode => _loadMode;
        [SerializeField] protected ESaveMode _saveMode = ESaveMode.Manual;
        public ESaveMode SaveMode => _saveMode;
#if ODIN_INSPECTOR
        [ShowIf("_saveMode", ESaveMode.Interval)]
#else
        [ShowIf(nameof(_saveMode), ESaveMode.Interval)]
#endif
        [SerializeField]
        protected double _saveIntervalSeconds = 120f;

        public double SaveIntervalSeconds
        {
            get => _saveIntervalSeconds;
            set => _saveIntervalSeconds = value;
        }
        
        public Action OnSaved = null;
        public Action OnLoaded = null;
        public Action OnDeleted = null;

        [Header("Runtime Data")] [SerializeField]
        protected T _data = new T();

        protected double _lastSaveTime;

        public virtual string FilePath => $"{Application.persistentDataPath}" +
                                  $"{Path.AltDirectorySeparatorChar}" +
                                  $"{name}.json";

        private string Directory => Path.GetDirectoryName(FilePath);
        public string LastSerializedJson { get; private set; }
        
        public bool FileExists => File.Exists(FilePath);
        public override Type GetGenericType => typeof(T);

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#else
            if (_loadMode == ELoadMode.Automatic)
            {
                Load();
            }
             if (_saveMode == ESaveMode.Interval)
            {
                ScriptableObjectUpdateSystem.RegisterObject(this);
            }
#endif
        }

        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#else
            ScriptableObjectUpdateSystem.UnregisterObject(this);
#endif
        }

        public override void Save()
        {
            LastSerializedJson = JsonUtility.ToJson(_data, prettyPrint: true);

            try
            {
                using (StreamWriter writer = new StreamWriter(FilePath))
                {
                    writer.Write(LastSerializedJson);
                }

                OnSaved?.Invoke();
                if (_debugLogEnabled)
                    Debug.Log("<color=#f75369>Save Saved:</color> \n" + LastSerializedJson);

                _lastSaveTime = Time.time;
#if UNITY_EDITOR
                RepaintRequest?.Invoke();
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving file: {e.Message}");
            }
        }

        public override void Load()
        {
            if (!FileExists)
            {
                Save();
            }

            try
            {
                using (StreamReader reader = new StreamReader(FilePath))
                {
                    LastSerializedJson = reader.ReadToEnd();
                    if (_debugLogEnabled)
                        Debug.Log("<color=#f75369>Save Loaded:</color> \n" + LastSerializedJson);
                    var saveData = JsonUtility.FromJson(LastSerializedJson, GetGenericType);
                    RestoreSaveData(saveData);
                    OnLoaded?.Invoke();
#if UNITY_EDITOR
                    RepaintRequest?.Invoke();
#endif
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading file: {e.Message}");
            }
        }

        private void RestoreSaveData(object data)
        {
            var saveData = data as T;
            if (RequiresUpgrade(saveData))
            {
                Upgrade(saveData);
            }

            _data = saveData;
        }

        public override void Delete()
        {
            if (FileExists)
                File.Delete(FilePath);

            Debug.Log("<color=#f75369>Save Deleted: </color>" + FilePath);
            LastSerializedJson = string.Empty;
            ResetData();
            OnDeleted?.Invoke();
#if UNITY_EDITOR
            RepaintRequest?.Invoke();
#endif
        }

        public virtual void PrintToConsole()
        {
            Debug.Log($"<color=#f75369>Save Data:</color>\n{LastSerializedJson}");
        }

        public override void Update()
        {
            if (Time.time - _lastSaveTime >= _saveIntervalSeconds)
            {
                Save();
                _lastSaveTime = Time.time;
            }
        }

        public void OpenSaveLocation()
        {
            if (!System.IO.Directory.Exists(Directory))
            {
                Debug.LogWarning("Save directory does not exist yet.");
                return;
            }

            try
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                Process.Start("explorer.exe", Directory);
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                Process.Start("open", Directory);
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
                Process.Start("xdg-open", Directory);
#else
                Debug.LogWarning("Opening save location is not supported on this platform.");
#endif

                if (_debugLogEnabled)
                    Debug.Log($"Opened save location: {Directory}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to open save location: {e.Message}");
            }
        }

        internal override void Reset()
        {
            base.Reset();
            _debugLogEnabled = false;
            _loadMode = ELoadMode.Automatic;
            _saveMode = ESaveMode.Manual;
            ResetData();
        }

        private void ResetData() => _data = new T();
        protected virtual bool RequiresUpgrade(T saveData) => false;

        protected virtual void Upgrade(T oldData)
        {
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.ExitingEditMode)
            {
                if (_loadMode == ELoadMode.Automatic)
                {
                    Load();
                }
                if (_saveMode == ESaveMode.Interval)
                {
                    ScriptableObjectUpdateSystem.RegisterObject(this);
                }
            }
            else if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                ScriptableObjectUpdateSystem.UnregisterObject(this);
            }
        }
#endif
    }
}