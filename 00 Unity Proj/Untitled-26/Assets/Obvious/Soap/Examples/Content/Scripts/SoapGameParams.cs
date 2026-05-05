using UnityEngine;

namespace Obvious.Soap.Example
{
    [CreateAssetMenu(fileName = "SoapGameParams", menuName = "Soap/Examples/ScriptableSingleton/SoapGameParams")]
    public class SoapGameParams : ScriptableSingleton<SoapGameParams>
    {
        //You can add any Soap SO here to manipulate the game parameters
        //They are also accessible from the Soap wizard, but sometimes it's convenient
        //to have them in a single place for easy access
        public FloatVariable PlayerHealth;
        public ScriptableEventNoParam ReloadSceneEvent;

        [Range(0, 100)] public int CoinSpawnedAmount = 10;
        [Range(0, 1000)] public int CoinRotateSpeed = 200;
        public bool RandomPlayerColorMode = false;

        //You can add useful methods here to manipulate the game parameters
        [ContextMenu("Heal Player")]
        public void HealPlayer()
        {
            PlayerHealth.Value += 10;
            Debug.Log("Player healed. Current health: " + PlayerHealth.Value);
        }

        [ContextMenu("Damage Player")]
        public void DamagePlayer()
        {
            PlayerHealth.Value -= 10;
            Debug.Log("Player took damage. Current health: " + PlayerHealth.Value);
        }
    }
}