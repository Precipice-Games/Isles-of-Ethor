using UnityEngine;

namespace Obvious.Soap.Example
{
    public class AutoRotator : MonoBehaviour
    {
        [SerializeField] private float _speed = 350f;

        public void Update()
        {
            transform.localEulerAngles += _speed * Vector3.up * Time.deltaTime;
        }

        public void SetRotationSpeed(float value)
        {
            _speed = value;
        }
    }
}