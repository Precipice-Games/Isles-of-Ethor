using UnityEngine;

/// <summary>
/// This script listens for when the player is on the airship
/// and calls the SFXManager to change the current footstep SFX
/// to the asset used for the airship and resets the SFX if the player
/// exits 
/// </summary>
public class FootstepSFXListener : MonoBehaviour
{
    [SerializeField] private SFXManager.FootstepSFX footstepSFX;
    
    private void OnEnable()
    {
        //Subscribes to the airship to tell when the player is on it
        Airship.playerOnAirship += OnPlayerOnAirship;
    }

    private void OnDisable()
    {
        //Unsubscribes when the player exits
        Airship.playerOnAirship -= OnPlayerOnAirship;
    }

    private void Start()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.SetFootstepSFX(footstepSFX);
        }
    }

    //Calls SFXManager to change SFX when the player enters or exits the airship
    private void OnPlayerOnAirship(bool isOnAirship)
    {
        if (SFXManager.Instance == null)
        {
            return;
        }

        if (isOnAirship)
        {
            SFXManager.Instance.SetFootstepSFX(SFXManager.FootstepSFX.Airship);
        }
        else
        {
            SFXManager.Instance.SetFootstepSFX(footstepSFX);
        }
    }
}
