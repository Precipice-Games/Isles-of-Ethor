using UnityEngine;

namespace Obvious.Soap.Example
{
    [CreateAssetMenu(fileName = "ScriptableDictionaryElementInt",
        menuName = "Soap/Examples/ScriptableDictionary/ScriptableDictionaryElementInt")]
    public class ScriptableDictionaryElementInt : ScriptableDictionary<ScriptableEnumElement, int>
    {
    }

    [System.Serializable]
    public class
        ScriptableDictionaryElementIntReadOnly : ScriptableDictionaryReadOnly<ScriptableEnumElement, int,
        ScriptableDictionaryElementInt>
    {
    }
}