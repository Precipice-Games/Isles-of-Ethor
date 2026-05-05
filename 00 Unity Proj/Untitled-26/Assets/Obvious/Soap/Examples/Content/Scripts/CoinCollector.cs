using UnityEngine;

namespace Obvious.Soap.Example
{
    public class CoinCollector : MonoBehaviour
    {
        [SerializeField] private IntVariable _coinCollected;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.parent.name.Contains("Coin"))
            {
                _coinCollected.Add(1);
                Destroy(other.gameObject);
            }
        }
    }
}