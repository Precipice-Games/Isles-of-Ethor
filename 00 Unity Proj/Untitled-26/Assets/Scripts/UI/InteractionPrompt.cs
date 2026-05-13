using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// This script controls the interaction prompt that appears when the player
/// is in range of an interactable object. This functionality is designated
/// in its own script because it's possible that the Player paused while in
/// range of an interactable object, which does not make the prompt disappear.
/// </summary>
public class InteractionPrompt : MonoBehaviour
{
    public GameObject interactPromptText;

    public bool isIslandComplete = false;

    /// <summary>
    /// Tracks if the player's raycast is currently hitting an interactable object.
    /// </summary>
    private bool isRaycastHitting;
    /// <summary>
    /// Tracks if the player is standing on a rune circle.
    /// </summary>
    private bool isInCircle;
    /// <summary>
    /// Tracks if the player is standing on the airship.
    /// </summary>
    private bool isOnAirship;

    // Subscribe to events
    private void OnEnable()
    {
        PlayerRaycastInteraction.raycastHitInteractable += ToggleRaycast;
        PlayerGroundcast.groundcastHitInteractable += ToggleRaycast;
        RuneCircle.playerInCircle += ToggleRuneCircle;
        Airship.playerOnAirship += ToggleAirship;
    }
    
    // Unsubscribe from events
    private void OnDisable()
    {
        PlayerRaycastInteraction.raycastHitInteractable -= ToggleRaycast;
        PlayerGroundcast.groundcastHitInteractable -= ToggleRaycast;
        RuneCircle.playerInCircle -= ToggleRuneCircle;
        Airship.playerOnAirship -= ToggleAirship;
    }


    private void Update()
    {
        TogglePrompt();
    }

    /// <summary>
    /// Toggles the interaction prompt on and off, 
    /// depending on the state of the raycast or if the player is on a rune circle.
    /// </summary>
    private void TogglePrompt()
    {
        if(GameStateManager.CurrentGameState != GameStateManager.GameState.Exploration)
        {
            interactPromptText.SetActive(false);
            return;
        }
        
        if(isRaycastHitting || isInCircle || (isOnAirship && isIslandComplete))
        {
            interactPromptText.SetActive(true);
        }
        else
        {
            interactPromptText.SetActive(false);
        }
    }

    /// <summary>
    /// Toggles the boolean variable that tracks if the player's raycast is hitting an interactable object.
    /// If the interaction should be enabled, the prompt is toggled on. If not, the prompt is toggled off.
    /// <param name="isHitting"></param>
    /// </summary>
    private void ToggleRaycast(bool isHitting)
    {
        isRaycastHitting = isHitting;
        TogglePrompt();
    }

    /// <summary>
    /// Toggles the boolean variable that tracks if the player is standing on a rune circle.
    /// If the interaction should be enabled, the prompt is toggled on. If not, the prompt is toggled off.
    /// <param name="inCircle"></param>
    /// </summary>
    private void ToggleRuneCircle(bool inCircle)
    {
        isInCircle = inCircle;
        TogglePrompt();
    }
    
    /// <summary>
    /// Toggles the boolean variable that tracks if the player is standing on the airship.
    /// If the interaction should be enabled, the prompt is toggled on. If not, the prompt
    /// is toggled off.
    /// <param name="onAirship"></param>
    /// </summary>
    private void ToggleAirship(bool onAirship)
    {
        isOnAirship = onAirship;
        TogglePrompt();
    }

    public void ToggleIslandComplete()
    {
        isIslandComplete = true;
    }
}