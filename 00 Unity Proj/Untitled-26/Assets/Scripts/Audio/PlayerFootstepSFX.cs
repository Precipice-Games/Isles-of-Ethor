using UnityEngine;

/// <summary>
/// This script is responsible for playing footstep sound effects based on the player's 
/// movement speed per frame
/// </summary>
public class PlayerFootstepSFX : MonoBehaviour
{
    [SerializeField] private float stepInterval = 0.45f;
    [SerializeField] private float minMoveSpeed = 0.1f;

    //Stores players position each frame to determine move speed
    private Vector3 lastPosition;

    //Timer tracks when to play next step sound
    private float stepTimer;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        //Calculates move speed based on how far the player moved since last frame
        float moveSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        if (moveSpeed > minMoveSpeed)
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
            stepTimer = 0f;
        }

        lastPosition = transform.position;
    }
}
