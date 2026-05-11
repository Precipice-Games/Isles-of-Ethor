using UnityEngine;

/// <summary>
/// This script is responsible for playing footstep sound effects based on the player's 
/// movement speed per frame
/// </summary>
public class PlayerFootstepSFX : MonoBehaviour
{
    [SerializeField] private float stepInterval = 0.45f;
    [SerializeField] private float minMoveSpeed = 0.1f;

    private PlayerMovement playerMovement;

    //Timer tracks when to play next step sound
    private float stepTimer;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        stepTimer = stepInterval;
    }

    private void Update()
    {
        if (playerMovement == null)
        {
            return;
        }

        if (playerMovement.IsGrounded && playerMovement.CurrentSpeed > minMoveSpeed)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                if (SFXManager.Instance != null)
                {
                    SFXManager.Instance.PlayFootstep();
                }

                stepTimer = stepInterval;
            }
        }
        else
        {
            //Timer is reset if standing still
            stepTimer = stepInterval;
        }
    }
}
