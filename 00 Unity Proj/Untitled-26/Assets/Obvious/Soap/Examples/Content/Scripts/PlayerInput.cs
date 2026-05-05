using UnityEngine;

namespace Obvious.Soap.Example   
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/1_scriptablevariables/solving-dependencies")]
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private BoolVariable _inputsEnabled = null;
        [SerializeField] private Vector2Variable _inputs = null;
        
        private Vector2 _currentInput = Vector2.zero;
        
        void Update()
        {
            if (!_inputsEnabled.Value)
                return;

            _currentInput.x = Input.GetAxis("Horizontal");
            _currentInput.y = Input.GetAxis("Vertical");
            
            _inputs.Value = _currentInput;
        }
    }
}