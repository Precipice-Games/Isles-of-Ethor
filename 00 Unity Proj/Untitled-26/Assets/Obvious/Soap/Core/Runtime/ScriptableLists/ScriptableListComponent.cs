using UnityEngine;

namespace Obvious.Soap
{
    [CreateAssetMenu(fileName = "scriptable_list_component.asset", menuName = "Soap/ScriptableLists/Component")]
    public class ScriptableListComponent : ScriptableList<Component>
    {
    }
    
    [System.Serializable]
    public class ScriptableListComponentReadOnly : ScriptableListReadOnly<ScriptableListGameObject, GameObject>
    {
    }
}