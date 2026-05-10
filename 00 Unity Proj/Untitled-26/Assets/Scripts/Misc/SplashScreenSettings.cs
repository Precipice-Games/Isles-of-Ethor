using System;
using UnityEngine;

public class SplashScreenSettings : MonoBehaviour
{
    public void Awake()
    {
        CursorSettings();
    }
    
    public void CursorSettings()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
