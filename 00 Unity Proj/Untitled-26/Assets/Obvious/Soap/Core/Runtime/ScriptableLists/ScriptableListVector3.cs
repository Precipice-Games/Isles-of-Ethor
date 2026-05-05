using UnityEngine;

namespace Obvious.Soap
{
    [CreateAssetMenu(fileName = "scriptable_list_vector3.asset", menuName = "Soap/ScriptableLists/Vector3")]
    public class ScriptableListVector3 : ScriptableList<Vector3>
    {
    }
    
    [System.Serializable]
    public class ScriptableListVector3ReadOnly : ScriptableListReadOnly<ScriptableListVector3, Vector3>
    {
    }
}