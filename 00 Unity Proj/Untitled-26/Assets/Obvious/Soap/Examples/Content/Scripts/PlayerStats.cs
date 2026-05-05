using Obvious.Soap.Attributes;
using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/soap-core-assets/scriptable-subassets")]
    [CreateAssetMenu(menuName = "Soap/Examples/SubAssets/PlayerStats")]
    public class PlayerStats : ScriptableObject
    {
        [SerializeField] [SubAsset] private FloatVariable _speed;
        [SubAsset] public FloatVariable Health;
        [SubAsset] public FloatVariable MaxHealth;
    }
}