using UnityEngine;

public class InteractableDoor : MonoBehaviour
{

    public Transform teleportLocation;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Player.Instance.TeleportPlayer(teleportLocation.position);
        }
    }

}
