using System.Collections;
using Obvious.Soap.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Obvious.Soap.Example
{
    public class EnemyMovementInjected : MonoBehaviour
    {
        [RuntimeInjectable("speed")] 
        [SerializeField]
        private FloatVariable _speed = null;
        [SerializeField] private UnityEvent _onStartMoving = null;
        [SerializeField] private UnityEvent _onStopMoving = null;

        private void Awake()
        {
            _speed.Value = 10; //(or whatever dynamic value you want);
        }

        private IEnumerator Start()
        {
            while (true)
            {
                _onStartMoving?.Invoke();
                yield return Cr_MoveTo(FindRandomPositionInRadius(10f));
                _onStopMoving?.Invoke();
                yield return Cr_WaitRandom();
            }
        }

        private IEnumerator Cr_WaitRandom()
        {
            var delay = Random.Range(0.5f, 2f);
            yield return new WaitForSeconds(delay);
        }

        private Vector3 FindRandomPositionInRadius(float radius)
        {
            var randomPos = Random.insideUnitSphere * radius;
            randomPos.y = 0;
            return randomPos;
        }

        private IEnumerator Cr_MoveTo(Vector3 destination)
        {
            var direction = (destination - transform.position).normalized;

            while (!IsAtDestination())
            {
                transform.position += direction * _speed * Time.deltaTime;
                yield return null;
            }

            bool IsAtDestination()
            {
                var sqrDistance = (destination - transform.position).sqrMagnitude;
                return sqrDistance <= 0.5f * 0.5f;
            }
        }
    }
}