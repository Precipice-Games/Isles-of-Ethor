using System;
using UnityEngine;

namespace Obvious.Soap.Example
{
    [HelpURL("https://obvious-game.gitbook.io/soap/soap-core-assets/scriptable-subassets")]
    public class HealthPowerUp : MonoBehaviour
    {
        [SerializeField] private PlayerStats _playerStats;

        private void OnTriggerEnter(Collider other)
        {
            _playerStats.Health.Add(30);
        }
    }
}