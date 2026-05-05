using UnityEngine;

namespace Obvious.Soap.Example
{
    [CreateAssetMenu(fileName = "scriptable_variable_" + nameof(Player), menuName = "Soap/Examples/ScriptableVariables/"+ nameof(Player))]
    public class PlayerVariable : ScriptableVariable<Player>
    {
        
    }

    [System.Serializable]
    public class PlayerVariableReadOnly : ScriptableVariableReadOnly<PlayerVariable, Player>
    {
        
    }
}
