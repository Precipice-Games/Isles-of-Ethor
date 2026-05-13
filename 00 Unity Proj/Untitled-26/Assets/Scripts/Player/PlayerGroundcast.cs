#if UNITY_EDITOR
using UnityEditor.Build;
#endif

using System;
using Sirenix.OdinInspector;
using UnityEngine;

// This script creates a raycast that shoots downwards from the Player.
// It is used to check if the Player is currently on the ground.

public class PlayerGroundcast : MonoBehaviour
{
    [Title("Grounded Checker", "General variables used to handle ground checking.")]
    private Ray groundInteractionRay;
    private RaycastHit groundRaycastHit;
    bool isHitting;
    private LayerMask layerMask;
    public GameObject currentPlatform;
    private bool onGround;
    private bool onAirship;
    public GameObject activeInteractable; //Stores overlapping interactable object
    private CharacterController charController;
    
    [Space]
    [Title("Raycast Settings", "Settings for the ray. Hover over variables for more information.")]
    [PropertyTooltip("Baseline desired length of a ray.")]
    public float rawRayLength;
    [PropertyTooltip("Be sure to add a buffer for the ray length to ensure consistency in ground detection.")]
    public float rayLengthBuffer;
    private float groundRayLength; // Actual ray length
    
    [Space]
    [Title("Debugging Options", "Settings for quick debugging options.")]
    [PropertyTooltip("Print out what ground the Player is standing on. False by default.")]
    public bool printGroundedStatus = false;
    
    // Static events to notify subscribers of grounded raycast hits
    public static event Action<bool> groundCheck;
    public static event Action<bool> airshipCheck;
    public static event Action<bool> groundcastHitInteractable;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        
        // Finalize the ray length by adding the raw length and the buffer together.
        groundRayLength = rawRayLength + rayLengthBuffer;
        
        // Even though the raw value of a ray is often half the size
        // of the object's height, sometimes there are issues detecting
        // the ground. Hence, why adding a tweakable buffer is necessary.
        
        layerMask = LayerMask.GetMask("Ground", "Airship");
    }
    
    /// <summary>
    /// In FixedUpdate(), we emit the ray from the origin of the Player and
    /// point it downwards. Collisions are handled within "if (hittingGround)"
    /// to see if the Player is standing on an object tagged with "Ground".
    /// </summary>

    void FixedUpdate()
    {
        // Set the origin and direction of the ray
        SetOriginAndDirection();
        
        groundInteractionRay.origin = transform.position;
        groundInteractionRay.direction = -transform.up;
        
        Vector3 origin = groundInteractionRay.origin;
        Vector3 direction = groundInteractionRay.direction;

        // Perform the raycast using the ray's origin and downward direction
        isHitting = Physics.Raycast(groundInteractionRay, out groundRaycastHit, groundRayLength, layerMask);

        // If the ray is hitting something
        if (isHitting)
        {
            int hitLayer = groundRaycastHit.collider.gameObject.layer;
            int groundLayer = LayerMask.NameToLayer("Ground");
            int airshipLayer = LayerMask.NameToLayer("Airship");
            
            // First check if it's the ground
            if (hitLayer == groundLayer)
            {
                Debug.DrawRay(origin, direction * groundRayLength, Color.green);
                currentPlatform = groundRaycastHit.collider.gameObject;
                if (printGroundedStatus) Debug.Log("PlayerGroundcast.cs >> Grounded on: " + currentPlatform.name);
                onGround = true;
                onAirship = false;
                activeInteractable = null;
                groundcastHitInteractable?.Invoke(false);
            }
            else if (hitLayer == airshipLayer)
            {
                // If not, assume the Player is standing on the airship
                // (only checking for "Ground" and "Airship" layers)
                Debug.DrawRay(origin, direction * groundRayLength, Color.purple);
                currentPlatform = groundRaycastHit.collider.gameObject;
                if (printGroundedStatus) Debug.Log("PlayerGroundcast.cs >> Grounded on: " + currentPlatform.name);
                onGround = false;
                onAirship = true;
                
                // Also check if the player is standing on an interactable object
                if (groundRaycastHit.collider.GetComponent<IInteractable>() != null)
                {
                    activeInteractable = groundRaycastHit.collider.gameObject;
                    groundcastHitInteractable?.Invoke(true);
                }
                else
                {
                    activeInteractable = null;
                    groundcastHitInteractable?.Invoke(false);
                }
            }
        }
        else
        {
            Debug.DrawRay(origin, direction * groundRayLength, Color.red);
            currentPlatform = null;
            activeInteractable = null;
            onGround = false;
            onAirship = false;
            groundcastHitInteractable?.Invoke(false);
        }
        
        groundCheck?.Invoke(onGround);
        airshipCheck?.Invoke(onAirship);
    }

    /// <summary>
    /// Sets the origin and direction of the raycast.
    /// </summary>
    private void SetOriginAndDirection()
    {
        // Start the ray slightly above the player's position to avoid starting inside a collider
        groundInteractionRay.origin = transform.position + Vector3.up * 0.1f;
        // Cast downward so we hit the ground under the player (use world down for stability)
        groundInteractionRay.direction = Vector3.down;
    }
    
    /// <summary>
    /// If the interact key is pressed the interact function triggers
    /// the Interaction() function of the interactable object.
    /// </summary>
    public void Interact()
    {
        if (activeInteractable != null)
        {
            activeInteractable.GetComponent<IInteractable>().Interaction();
            activeInteractable = null;
            groundcastHitInteractable?.Invoke(true);
        }
    }

    public bool GetOnGrounded()
    {
        return onGround;
    }

}
