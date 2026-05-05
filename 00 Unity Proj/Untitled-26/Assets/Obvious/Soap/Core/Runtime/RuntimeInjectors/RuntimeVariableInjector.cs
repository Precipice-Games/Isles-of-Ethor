using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using Obvious.Soap.Attributes;
#endif
using UnityEngine;

namespace Obvious.Soap
{
    [DefaultExecutionOrder(-1)]
    public class RuntimeVariableInjector<TVariable> : MonoBehaviour where TVariable : ScriptableVariableBase
    {
        [SerializeField] private InjectedVariableWrapper[] _runtimeVariables = null;

        [SerializeField] private bool _debugLogEnabled = false;

        private List<InjectedVariable<TVariable>> _runtimeInjectedVariables;

        private void Awake()
        {
            if (_runtimeVariables == null || _runtimeVariables.Length == 0)
            {
                if (_debugLogEnabled)
                    Debug.LogWarning($"{nameof(RuntimeVariableInjector<TVariable>)}: No injected variables specified.");
                return;
            }

            _runtimeInjectedVariables = new List<InjectedVariable<TVariable>>();
            foreach (var wrapper in _runtimeVariables)
            {
                var injectedVariable = new InjectedVariable<TVariable>(wrapper);
                if (injectedVariable.VariableTemplate != null)
                {
                    var deepCopy = SoapRuntimeUtils.CreateCopy(injectedVariable.VariableTemplate);
                    injectedVariable.RuntimeVariable = deepCopy;
                    wrapper.RuntimeVariable = deepCopy;
                }
                else
                {
                    var runtimeVariable =
                        SoapRuntimeUtils.CreateRuntimeInstance<TVariable>($"{gameObject.name}_{injectedVariable.Id}");
                    injectedVariable.RuntimeVariable = runtimeVariable;
                    wrapper.RuntimeVariable = runtimeVariable;
                }
                _runtimeInjectedVariables.Add(injectedVariable);
            }

            foreach (var injectedVariable in _runtimeInjectedVariables)
            {
                SoapRuntimeUtils.InjectInChildren(
                    gameObject,
                    injectedVariable.RuntimeVariable,
                    injectedVariable.Id,
                    _debugLogEnabled
                );
            }
        }
    }
    
    [System.Serializable]
    internal class InjectedVariableWrapper
    {
        [Tooltip("This Id has to match the one used in the attribute for a specific variable.")] [SerializeField]
        private string _id = "";

        [SerializeField] 
#if ODIN_INSPECTOR
        [ReadOnly]
#else
        [BeginDisabledGroup] 
#endif
        private ScriptableVariableBase _runtimeVariable = null;

#if !ODIN_INSPECTOR
        [EndDisabledGroup] 
#endif
        [SerializeField]
        [Tooltip("If set, a deep copy of this variable will be created at runtime. " +
                 "\n If left null, a new instance will be created.")]
        private ScriptableVariableBase _variableTemplate = null;

        public string ID => _id;
        public ScriptableVariableBase VariableTemplate => _variableTemplate;

        public ScriptableVariableBase RuntimeVariable
        {
            get => _runtimeVariable;
            set => _runtimeVariable = value;
        }
    }

    internal class InjectedVariable<TVariable> where TVariable : ScriptableVariableBase
    {
        public string Id { get; }
        public TVariable RuntimeVariable { get; set; }
        public TVariable VariableTemplate { get; }

        public InjectedVariable(InjectedVariableWrapper wrapper)
        {
            Id = wrapper.ID;
            VariableTemplate = wrapper.VariableTemplate as TVariable;
        }
    }
}