using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/1_scriptablevariables/saving-variable-values")]
    public class PositionSaver : MonoBehaviour
    {
        [SerializeField] private Vector3Variable _vector3Variable = null;

        private void Start()
        {
            transform.position = _vector3Variable.Value;
        }

        private void Update()
        {
            _vector3Variable.Value = transform.position;
        }
    }
}