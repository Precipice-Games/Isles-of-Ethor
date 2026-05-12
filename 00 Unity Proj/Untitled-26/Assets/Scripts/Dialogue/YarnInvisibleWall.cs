using System;
using UnityEngine;
using Yarn.Unity;

public class YarnInvisibleWall : MonoBehaviour
{
    public Collider colliderToDisable;

    [YarnCommand("disableInvisWall")]
    public void DisableInvisWall()
    {
        // Disable collider
        if (colliderToDisable != null)
        {
            colliderToDisable.enabled = false;
        }
    }
}
