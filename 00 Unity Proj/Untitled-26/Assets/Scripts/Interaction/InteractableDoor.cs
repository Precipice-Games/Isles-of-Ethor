using UnityEngine;
using System;
using UnityEngine.Events;

public class InteractableDoor : MonoBehaviour
{

    public Transform teleportLocation;

    public UnityEvent<Vector3> doorEntered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Player.Instance.TeleportPlayer(teleportLocation.position);
            doorEntered.Invoke(teleportLocation.position);
        }
    }

}
