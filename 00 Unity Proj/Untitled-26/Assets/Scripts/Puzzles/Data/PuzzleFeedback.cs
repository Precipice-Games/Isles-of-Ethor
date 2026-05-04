using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using SimpleTimer;

/// <summary>
/// This script takes in and communicates data regarding the feedback of the puzzle.
/// Handles puzzle feedback for invalid moves and other messages.
/// </summary>
public class PuzzleFeedback : MonoBehaviour
{
    [Title("Puzzle Feedback Variables", "Variables used in the Puzzle Feedback system.")]
    [PropertyTooltip("Please assign the feedbackText child object here.")]
    public GameObject feedbackText;
    
    // Puzzle variables
    private TMP_Text messageText;
    
    // SelectableTile variables
    private SelectableTile tile;
    private Renderer tileRenderer;
    private Color originalColor;
    private Color flashColor = Color.red;
    
    // Timer variables
    private TimerManager.Timer flickerTimer;
    private bool isFlashing = false;
    private float errorLastIntervalTime = 0f;
    private float errorInterval = 0.25f;
    
    [Space]
    [Title("Debugging Options", "Settings for quick debugging options.")]
    [PropertyTooltip("Print feedback text updates. False by default.")]
    public bool printFeedbackUpdates = false;
    [PropertyTooltip("Print out info regarding the flicker timer. False by default.")]
    public bool printTimerInfo = false;
    
    void Start()
    {
        // Get reference to the text component
        messageText = feedbackText.GetComponent<TMP_Text>();
        
        // Hide popup text at start
        feedbackText.SetActive(false);
    }
    
    /// <summary>
    /// General method to show feedback on screen and flash the tile. If it's a resource-related
    /// event, the tile parameter will be null and only the text feedback will be shown. If it's
    /// an invalid move, the tile parameter will be used to make the tile flash red.
    /// </summary>
    public void DisplayFeedback(string message, SelectableTile tile)
    {
        StartFeedback(tile, message);
    }

    /// <summary>
    /// If the SFXManager is available, play the invalid move SFX.
    /// </summary>
    private void PlayInvalidMoveSFX()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayInvalidMove();
        }
    }
    
    /// <summary>
    /// Used to start the visual feedback process, including flashing the
    /// SelectableTile and updating the onscreen feedback text.
    /// </summary>
    /// <param name="selectableTile"></param>
    /// <param name="message"></param>
    private void StartFeedback(SelectableTile selectableTile, string message)
    {
        // If there is a tile, grab it. If not, set it to null.
        tile = selectableTile;
        tileRenderer = tile != null ? tile.GetComponent<Renderer>() : null;

        // Store tile's original color to reset to later
        if (tileRenderer != null) originalColor = tileRenderer.material.color;

        // Reset interval timing every time an error happens
        errorLastIntervalTime = 0f;
        isFlashing = false;

        // Play SFX for invalid move
        PlayInvalidMoveSFX();
        
        // Stop previous timer if it exists
        if (flickerTimer != null) TimerManager.DeleteTimer(flickerTimer);

        // Show popup text
        if (feedbackText != null)
        {
            messageText.text = message;
            feedbackText.SetActive(true);
            if (printFeedbackUpdates) Debug.Log($"PuzzleFeedback.cs >> The onScreenText message was set to {messageText.text}");
        }

        // Create a 2-second timer with tick for flashing
        flickerTimer = TimerManager.CreateTimer(2.0f, OnTimerFinished, OnTimerTick);
    }

    /// <summary>
    /// Called when the flickerTimer is done. It is mainly used to restore
    /// the SelectableTile to its normal color and disable the onscreen text.
    /// </summary>
    private void OnTimerFinished()
    {
        if (printTimerInfo) Debug.Log("PuzzleFeedback.cs >> Timer finished!");

        // If there is a tile, reset its color
        if (tileRenderer != null) tileRenderer.material.color = originalColor;
        
        // Disable popup text
        feedbackText.SetActive(false);

        // Ensure the timer is deleted
        TimerManager.DeleteTimer(flickerTimer);
    }

    /// <summary>
    /// Called with each tick of the timer. This is assigned when a new
    /// flickerTimer is created and is mainly used to properly alternate
    /// between the normal tile color and the flash color at set intervals.
    /// </summary>
    private void OnTimerTick()
    {
        if (tileRenderer == null) return;
        
        float elapsedTime = flickerTimer.GetElapsedTime();
        bool intervalHit = elapsedTime - errorLastIntervalTime >= errorInterval;

        if (intervalHit)
        {
            errorLastIntervalTime = elapsedTime;
            
            Debug.Log("PuzzleFeedback.cs >> Timer tick: " + elapsedTime + " seconds elapsed. Flashing tile color.");

            // Toggle color
            isFlashing = !isFlashing;

            tileRenderer.material.color = isFlashing ? flashColor : originalColor;
        }
    }
}