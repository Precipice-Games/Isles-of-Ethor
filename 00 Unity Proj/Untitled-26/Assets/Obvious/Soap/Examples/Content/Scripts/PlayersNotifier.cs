using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/3_scriptablelists/iteration")]
    public class PlayersNotifier : MonoBehaviour
    {
        [SerializeField] private ScriptableListPlayer scriptableListPlayer = null;
        [SerializeField] private IterationType _iterationType = IterationType.IEnumerable;

        private float _delay = 1.5f;
        private float _timer = 0f;

        private enum IterationType
        {
            IEnumerable,
            Foreach,
            Index
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= _delay)
            {
                NotifyPlayers();
                _timer = 0;
            }
        }

        private void NotifyPlayers()
        {
            switch (_iterationType)
            {
                case IterationType.IEnumerable:
                    foreach (var player in scriptableListPlayer)
                        player.Notify();
                    break;
                case IterationType.Foreach:
                    scriptableListPlayer.ForEach(player => player.Notify());
                    break;
                case IterationType.Index:
                    for (int i = scriptableListPlayer.Count - 1; i >= 0; i--)
                        scriptableListPlayer[i].Notify();
                    break;
            }
        }
    }
}