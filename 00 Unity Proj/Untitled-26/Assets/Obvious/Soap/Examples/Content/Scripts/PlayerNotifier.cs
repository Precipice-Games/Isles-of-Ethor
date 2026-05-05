using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/1_scriptablevariables/solving-dependencies")]
    public class PlayerNotifier : MonoBehaviour
    {
        [SerializeField] private PlayerVariable _playerVariable = null;

        private readonly float _delay = 1.5f;
        private float _timer = 0f;
        
        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= _delay)
            {
                //perform any actions on the player!
                //not need to find the player in the scene, or to access it via a singleton
                _playerVariable.Value.Notify();
                _timer = 0;
            }
        }
    }
}