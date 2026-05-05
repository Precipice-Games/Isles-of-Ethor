using JetBrains.Annotations;
using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/3_scriptablelists/adding-and-removing-elements")]
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private ScriptableListPlayer _scriptableListPlayer = null;
        [SerializeField] private Player _prefab = null;

        [UsedImplicitly]
        public void Spawn()
        {
            var randomPosition = Random.insideUnitSphere * 10f;
            randomPosition.y = 0f;

            var player = Instantiate(_prefab, randomPosition, Quaternion.identity, transform);
            player.name = $"prefab_player_{_scriptableListPlayer.Count}";
        }

        [UsedImplicitly]
        public void DestroyOldest()
        {
            if (_scriptableListPlayer.IsEmpty)
                return;

            _scriptableListPlayer.DestroyFirst();
        }
    }
}