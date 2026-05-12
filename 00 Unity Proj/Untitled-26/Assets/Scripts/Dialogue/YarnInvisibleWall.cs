using System;
using UnityEngine;
using Yarn.Unity;

public class YarnInvisibleWall : MonoBehaviour
{
    public YarnInvisibleWall invisWall;

    [YarnCommand("disableInvisWall")]
    public void DisableInvisWall()
    {
        // Disable collider
        if (invisWall != null)
        {
            for(int i = 0; i < invisWall.GetComponents<BoxCollider>().Length; i++)
            {
                invisWall.GetComponents<BoxCollider>()[i].enabled = false;
            }
        }
    }

    [YarnCommand("enableInvisWall")]
    public void EnableInvisWall()
    {
        // Enable collider
        for (int i = 0; i < invisWall.GetComponents<BoxCollider>().Length; i++)
        {
            invisWall.GetComponents<BoxCollider>()[i].enabled = true;
        }
    }
}
