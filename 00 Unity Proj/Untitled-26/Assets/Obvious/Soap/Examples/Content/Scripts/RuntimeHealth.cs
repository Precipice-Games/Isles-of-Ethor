using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/soap-core-assets/scriptable-variable/runtime-variables")]
    public class RuntimeHealth : MonoBehaviour
    {
        [Tooltip("Leave this null if you wan it to be instantiated at runtime")]
        [SerializeField]
        private FloatVariable _runtimeHpVariable;
        [SerializeField] 
        private FloatReference _maxHealth = null;

        private void Awake()
        {
            CreateAndInjectRuntimeVariableManually();
        }

        private void CreateAndInjectRuntimeVariableManually()
        {
            //If the variable is not set in the inspector, create a runtime instance and set the reference.
            if (_runtimeHpVariable == null)
                _runtimeHpVariable = SoapRuntimeUtils.CreateRuntimeInstance<FloatVariable>($"{gameObject.name}_hp");
            
            //Set the max and clamp the value (clamping is optional, but why not?).
            _runtimeHpVariable.MaxReference = _maxHealth;
            _runtimeHpVariable.IsClamped = true;
            
            //Initialize the health bar only after the variable has been created.
            //You can use events to decouple components if your health bar is in another scene (UI scene for example). 
            //In this example, it's a local Health bar so a direct reference is fine. 
            GetComponentInChildren<HealthBarSprite>().Init(_runtimeHpVariable);
        }

        private void Start()
        {
            //For the runtime variables, you can do this in Awake, after creating the runtime instance.
            //However, because doing this in Start works for both cases (when creating a runtime variable and referencing an existing one), it's better to do it in Start.
            //Indeed, for the Boss, its value is reset OnSceneLoaded and because of the execution order:
            //Awake-> SceneLoaded -> Start , the value should be Set to the max health in Start.
            //Note that you could also remove Awake entirely and have the logic before these lines.
            _runtimeHpVariable.Value = _maxHealth.Value;
            _runtimeHpVariable.OnValueChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            _runtimeHpVariable.OnValueChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float newValue)
        {
            if (newValue <= 0f)
                gameObject.SetActive(false);
        }

        //In this example, this is called when the enemy collides with the Player.
        public void TakeDamage(int amount) => _runtimeHpVariable.Add(-amount);
    }
}