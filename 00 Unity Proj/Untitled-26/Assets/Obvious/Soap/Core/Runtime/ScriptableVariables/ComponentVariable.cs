using UnityEngine;

namespace Obvious.Soap
{
    [CreateAssetMenu(fileName = "scriptable_variable_component.asset", menuName = "Soap/ScriptableVariables/Component")]
    public class ComponentVariable : ScriptableVariable<Component>
    {
        public override string ToString()
        {
            if (Value == null) 
                return $"{name} : null";
            return $"{name} : {Value.GetType().Name} (GameObject: {Value.gameObject.name})";
        }
    }
}