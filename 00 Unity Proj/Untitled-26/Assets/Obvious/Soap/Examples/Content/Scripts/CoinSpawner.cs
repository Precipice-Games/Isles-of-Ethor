using UnityEngine;

namespace Obvious.Soap.Example
{
    public class CoinSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab = null;
        [SerializeField] private float _radius = 10f;

        private void Start()
        {
            Spawn(SoapGameParams.Instance.CoinSpawnedAmount);
        }
        
        private void Spawn(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var spawnInfo = GetRandomPositionAndRotation();
                var obj = Instantiate(_prefab, spawnInfo.position, spawnInfo.rotation, transform);
                obj.AddComponent<AutoRotatorWithSingleton>();
                obj.SetActive(true);
            }
        }
        
        private (Vector3 position, Quaternion rotation) GetRandomPositionAndRotation()
        {
            var randomPosition = Random.insideUnitSphere * _radius;
            randomPosition.y = 0f;
            var spawnPos = transform.position + randomPosition;
            var randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            return (spawnPos, randomRotation);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}