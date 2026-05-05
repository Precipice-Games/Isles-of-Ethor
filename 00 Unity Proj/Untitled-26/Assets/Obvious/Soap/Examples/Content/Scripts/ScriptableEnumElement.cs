using System.Collections.Generic;
using UnityEngine;

namespace Obvious.Soap.Example
{
    [CreateAssetMenu(fileName = "scriptable_enum_Element", menuName = "Soap/Examples/ScriptableEnums/Element")]
    public class ScriptableEnumElement : ScriptableEnumBase
    {
        //convenient to have additional data in the ScriptableEnum
        public Sprite Icon = null;
        public Color Color = Color.white;
        public List<ScriptableEnumElement> Defeats = null;
    }
}