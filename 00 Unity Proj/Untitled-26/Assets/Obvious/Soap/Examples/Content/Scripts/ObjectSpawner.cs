using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Obvious.Soap.Example
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab = null;
        [SerializeField] private int _amount = 10;
        [SerializeField] private float _radius = 10f;

        [UsedImplicitly]
        public void Spawn()
        {
            for (int i = 0; i < _amount; i++)
            {
                var spawnInfo = GetRandomPositionAndRotation();
                var obj = Instantiate(_prefab, spawnInfo.position, spawnInfo.rotation, transform);
                obj.SetActive(true);
            }
        }
        
        private (Vector3 position, Quaternion rotation) GetRandomPositionAndRotation()
        {
            var randomPosition = Random.insideUnitSphere * _radius;
            randomPosition.y = 0f;
            var spawnPos = transform.position + randomPosition;
            var randomRotation = Quaternion.Euler(
                Random.Range(0, 360),
                Random.Range(0, 360),
                Random.Range(0, 360));
            return (spawnPos, randomRotation);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}