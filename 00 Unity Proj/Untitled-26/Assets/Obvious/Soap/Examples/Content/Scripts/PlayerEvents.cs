using Obvious.Soap.Attributes;
using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/scene-documentation/7_scriptablesubassets")]
    [CreateAssetMenu(menuName = "Soap/Examples/SubAssets/PlayerEvents")]
    public class PlayerEvents : ScriptableObject
    {
        [SerializeField] [SubAsset] private ScriptableEventInt _onDamaged;
        [SerializeField] [SubAsset] private ScriptableEventInt _onHealed;
        [SerializeField] [SubAsset] private ScriptableEventNoParam _onDeath;
    }
}