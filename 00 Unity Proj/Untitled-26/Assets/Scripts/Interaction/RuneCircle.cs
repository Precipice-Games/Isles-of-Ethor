using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class RuneCircle : MonoBehaviour
{
    [Title("Puzzle Information")]
    [InfoBox("Attach the data of the puzzle that this rune circle corresponds to.")]
    public PuzzleInformation puzzleInfo;
    public ExitPuzzleButton exitPuzzleButton;
    /// <summary>
    /// The transform of the other rune circle that the player will be teleported to when they interact with this rune circle after completing the puzzle. 
    /// This is used in the TeleportPlayer() method.
    /// </summary>
    public Transform otherRuneCircle;

    /// <summary>
    /// Tracks if player is standing on an active rune circle.
    /// </summary>
    private bool inCircle;
    private GameObject player;

    public static RuneCircle activeRuneCircle;

    // An event that is invoked when the player stands on the rune circle
    public static event Action<bool> playerInCircle;

    // Static event to notify subscribers of game state changes
    public static event Action<PuzzleInformation> puzzleTriggered;

    // Subscribe to events
    private void OnEnable()
    {
        PlayerInteraction.playerInteraction += Interaction;
    }

    // Unsubscribe from events
    private void OnDisable()
    {
        PlayerInteraction.playerInteraction -= Interaction;
    }

    /// <summary>
    /// Called when the Player's collider enters the rune circle collider.
    /// This sets inCircle to true and invokes the playerInCircle event
    /// (true), which is picked up by InteractionPrompt.cs.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInCircle?.Invoke(true);
            inCircle = true;
            player = other.gameObject;

            activeRuneCircle = this;
        }
    }

    /// <summary>
    /// Called when the Player's collider exits the rune circle collider.
    /// This sets inCircle to false and invokes the playerInCircle event
    /// (false), which is picked up by InteractionPrompt.cs.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInCircle?.Invoke(false);
            inCircle = false;

            if (activeRuneCircle == this)
            {
                activeRuneCircle = null;
            }
        }
    }

    /// <summary>
    /// Subscribed to the playerInteraction event in PlayerInteraction.cs. This is
    /// triggered when the Player presses 'E' while standing in the rune circle. It
    /// first checks if the puzzle has already been completed. If so, the Player is
    /// teleported to the next rune circle. Otherwise, puzzleTriggered is fired to
    /// set up the puzzle state.
    /// </summary>
    public void Interaction()
    {
        // If the player is not standing on the rune circle, break out.
        if (!inCircle) return;

        // If there's no puzzle info, break out.
        if (!PuzzleInfoFound()) return;

        // Play rune circle sound effect
        SFXManager.Instance.PlayRuneCircle();
        
        // If the puzzle has already been completed, teleport to the other rune circle.
        if (puzzleInfo.puzzleSolved == true)
        {
            TeleportPlayer();
            return;
        }

        // Assign the transform position of the player's respawn location to be on the starting rune circle.
        exitPuzzleButton.currentRuneCircle = this;
        exitPuzzleButton.respawnLocation = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);

        // If the puzzle has not been completed, trigger the
        // event to notify subscribers.
        puzzleTriggered.Invoke(puzzleInfo);
    }

    /// <summary>
    /// Verifies that this rune circle has the puzzleInfo variable for its corresponding puzzle.
    /// </summary>
    /// <returns></returns>
    private bool PuzzleInfoFound()
    {
        if (puzzleInfo != null)
        {
            return true;
        }

        Debug.LogError("RuneCircle.cs >> No puzzle information attached to this rune circle.");
        return false;
    }

    /// <summary>
    /// Teleport the player to the other rune circle.
    /// This is triggered when the player interacts with a rune circle of a completed puzzle.
    /// </summary>
    private void TeleportPlayer()
    {
        Debug.Log("RuneCircle.cs >> Teleporting player to other rune circle.");
        if (player)
        {
            Vector3 otherCirclePosition = otherRuneCircle.position;
            // Have sky teleport to slightly above rune circle to prevent player from getting stuck in the ground or bouncing.
            Vector3 newPlayerPos = new Vector3(otherCirclePosition.x, otherCirclePosition.y + 1.0f, otherCirclePosition.z);

            Player.Instance.TeleportPlayer(newPlayerPos);
        }
        else
        {
            Debug.Log("RuneCircle.cs >> Could not find player.");
        }
    }
}
