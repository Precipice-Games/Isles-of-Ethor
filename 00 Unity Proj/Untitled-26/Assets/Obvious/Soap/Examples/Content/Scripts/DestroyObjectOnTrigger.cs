using UnityEngine;

namespace Obvious.Soap.Example
{
    public class DestroyObjectOnTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Destroy(other.gameObject);
        }
    }
}