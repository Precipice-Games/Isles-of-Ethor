using UnityEngine;

namespace Obvious.Soap.Example
{
    public class AutoRotatorWithSingleton : MonoBehaviour
    {
        public void Update()
        {
            transform.localEulerAngles += SoapGameParams.Instance.CoinRotateSpeed * Vector3.up * Time.deltaTime;
        }
    }
}