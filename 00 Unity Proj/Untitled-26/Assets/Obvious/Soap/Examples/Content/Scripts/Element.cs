using TMPro;
using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/6_scriptableenums")]
    [SelectionBase]
    public class Element : MonoBehaviour
    {
        [SerializeField] private ScriptableEnumElement _elementType = null;
        public ScriptableEnumElement ElementType => _elementType;

        private void Start()
        {
            GetComponent<Renderer>().material.color = _elementType.Color;
            GetComponentInChildren<TextMeshPro>().text = _elementType.Icon.name;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent<Element>(out var element))
            {
                if (_elementType.Defeats.Contains(element.ElementType))
                    Destroy(other.gameObject);
            }
        }
    }
}