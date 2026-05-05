using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/4_scriptableevents/registering-to-events-from-code")]
    public class HealVignette : MonoBehaviour
    {
        [SerializeField] private ScriptableEventInt _onPlayerHealedEvent = null;
        [SerializeField] private FadeOut _fadeOut = null;

        private void Awake()
        {
            _onPlayerHealedEvent.OnRaised += OnPlayerHealed;
        }

        private void OnDestroy()
        {
            _onPlayerHealedEvent.OnRaised -= OnPlayerHealed;
        }

        private void OnPlayerHealed(int amount)
        {
            _fadeOut.Activate();
        }
    }
}