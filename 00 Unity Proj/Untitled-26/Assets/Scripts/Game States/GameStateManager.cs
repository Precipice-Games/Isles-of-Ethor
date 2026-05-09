using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityCommunity.UnitySingleton;
using UnityEngine.SceneManagement;

/// <summary>
/// This is the GameStateManager class, which acts as an overarching state handler
/// for the game. It fires of several events to different listeners to ensure successful
/// state changes. It is also subscribed to other events that may change the game state.
/// </summary>

public class GameStateManager : MonoSingleton<GameStateManager>
{

    [Title("Game State", "Choose the default game state that this scene will load in.")]
    [EnumToggleButtons, HideLabel]
    public GameState gameState;
    // Note: This variable is not static because each instance of the component
    // needs to maintain its own copy of the current game state for reference.
    
    [Space]
    [Title("Debugging Options", "Settings for quick debugging options.")]
    [PropertyTooltip("Print timescale and pausable values. False by default.")]
    public bool printTimescalePausable = false;
    [PropertyTooltip("Print out messages for scene initialization. True by default.")]
    public bool printSceneDefaults = true;
    [PropertyTooltip("Print out the state transition information. True by default.")]
    public bool printStateTransition = true;
    
    /// <summary>
    /// The set of possible game states.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Exploration,
        Puzzle,
        Dialogue,
        Paused,
        Settings,
        Loading
    }

    /// <summary>
    /// The state the game is currently in.
    /// </summary>
    public static GameState CurrentGameState { get; set; }

    /// <summary>
    /// The most recent previous state the game was in before the current state.
    /// </summary>
    public static GameState prevState;

    /// <summary>
    /// Tracks if the player is able to pause from the current state.
    /// </summary>
    /// <remarks> 
    /// This variable also accounts for if the game can be unpaused, which is only true for the 'Paused' game state.
    /// </remarks>
    public static bool pausable { get; set; }
    
    /// <summary>
    /// Event Action used to notify subscribers of game state changes.
    /// </summary>
    /// <remarks> 
    /// This should be triggered after the new state has been assigned. The event will pass the new game state as a parameter to any subscribed methods.
    /// </remarks>
    
    // Static event to notify subscribers of game state changes
    public static event Action<GameState> transitionedToNewState;
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        ExitPuzzleButton.exitPuzzle += TransitionToState;
        UIChangerButton.optionsMenuToggled += TogglePause;
        SceneChanger.queueLoadingScreen += onLoadingScreenTriggered;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        ExitPuzzleButton.exitPuzzle -= TransitionToState;
        UIChangerButton.optionsMenuToggled -= TogglePause;
        SceneChanger.queueLoadingScreen -= onLoadingScreenTriggered;
    }
    
    // Runs when a scene is loaded
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (printSceneDefaults) Debug.Log("GameStateManager.cs >> OnSceneLoaded() >> New scene has been loaded: " + scene.name);
        if (printSceneDefaults) Debug.Log($"GameStateManager.cs >> Setting the Scene Defaults for defaultState {gameState}...");
        // Initialize the static CurrentGameState from the inspector value
        SetSceneDefaults(gameState);
    }
    
    // Runs when the active scene is changed
    public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
    {
        if (printSceneDefaults) Debug.Log($"GameStateManager.cs >> OnActiveSceneChanged() >> Switched from {prevScene.name} to {nextScene.name}");
        if (printSceneDefaults) Debug.Log($"GameStateManager.cs >> Setting the Scene Defaults for defaultState {gameState}...");
        // Initialize the static CurrentGameState from the inspector value
        SetSceneDefaults(gameState);
    }

    // Method just used to set the scene defaults.
    // All other state changes occur in TransitionToState().
    private void SetSceneDefaults(GameState defaultState)
    {
        // There is no previous state, so just ensure
        // the CurrentGameState is assigned.
        CurrentGameState = defaultState;
        if (printSceneDefaults) Debug.Log("GameStateManager.cs >> Set the CurrentGameState to the defaultState " + CurrentGameState);
        HandlePauseValues(CurrentGameState);
    }

    /// <summary>
    /// Method used to assign game states and update the previous state.
    /// </summary>
    /// <param name="newState"></param>
    private void TransitionToState(GameState newState)
    {
        if (printStateTransition) Debug.Log($"onPuzzleTrigger() >> Current state is {CurrentGameState}. Attempting to transition to {newState}...");
        prevState = CurrentGameState;
        CurrentGameState = newState;
        if (printStateTransition) Debug.Log("GameStateManager.cs >> State transitioned to: " + CurrentGameState + " >> Previous State: " + prevState); // Confirm the state change
        HandlePauseValues(CurrentGameState);
    }

    /// <summary>
    /// Handles the logic for timescale and pausable bool.
    /// </summary>
    /// <remarks> 
    /// Once we've transitioned to the new game state, this method will update the timescale and the pausable variable accordingly.
    /// </remarks>
    private void HandlePauseValues(GameState newState)
    {
        // Update the timescale and pausable variable based on the new game state
        switch (newState)
        {
            case GameState.MainMenu:
                TogglePause(false);
                Time.timeScale = 0.0f;
                break;
            case GameState.Exploration:
                TogglePause(true);
                Time.timeScale = 1.0f;
                break;
            case GameState.Puzzle:
                TogglePause(true);
                Time.timeScale = 1.0f;
                break;
            case GameState.Dialogue:
                TogglePause(true);
                Time.timeScale = 1.0f;
                break;
            case GameState.Paused:
                TogglePause(true);
                Time.timeScale = 0.0f;
                break;
            case GameState.Settings:
                TogglePause(false);
                Time.timeScale = 0.0f;
                break;
            case GameState.Loading:
                TogglePause(false);
                Time.timeScale = 1.0f;
                break;
        }
        
        if (printTimescalePausable) Debug.Log($"GameStateManager.cs >> {newState} loaded, time scale set to {Time.timeScale.ToString()} and pausable set to {pausable}.");
        
        // Trigger this event after game state, timescale,
        // and pausable bool have been updated. 
        transitionedToNewState?.Invoke(CurrentGameState);
    }

    /// <summary>
    /// This function listens for the Pause event (e.g., player pressing 'T' or 'ESC')
    /// and checks current state of the game to pause/resume. When switching from paused
    /// to gameplay, change CurrentGameState to the state we were in prior to pausing.
    /// </summary>
    public void onPause()
    {
        if (CurrentGameState == GameState.Paused && pausable)
        {
            // Possible previous states should be Exploration, Puzzle, and Dialogue.
            TransitionToState(prevState);
        }
        else if (pausable)
        {
            TransitionToState(GameState.Paused);
        }
        else
        {
            if (printStateTransition) Debug.Log("GameStateManager.cs >> Cannot pause from the current state.");
        }
    }

    /// <summary>
    /// This is a listener for OnDialogueStart() and OnDialogueComplete() events invoked by YarnSpinner's DialogueRunner.
    /// It triggers a state transition to the Dialogue state when dialogue starts, and returns to the previous state (exploration or puzzle) when dialogue ends.
    /// </summary>
    public void onDialogueTrigger()
    {
        if (CurrentGameState == GameState.Dialogue)
        {
            // As of now this should only return to Exploration state. However,
            // this accounts for if future dialogue is triggered in a puzzle state.
            TransitionToState(GameState.Exploration);
        }
        else
        {
            TransitionToState(GameState.Dialogue);
        }
    }
    
    /// <summary>
    /// This switches the Player into Puzzle Mode upon the triggering of puzzleSwitchDetected, a UnityEvent defined in ViewManager.cs.
    /// </summary>
    public void onPuzzleTrigger()
    {
        TransitionToState(GameState.Puzzle);
    }
    
    /// <summary>
    /// This switches the Player back to Exploration mode upon the triggering
    /// of puzzleCompleted, a UnityEvent defined in PlayerFixedMovement.cs.
    /// </summary>
    public void onPuzzleCompleted()
    {
        TransitionToState(GameState.Exploration);
    }
    
    /// <summary>
    /// This switches the Player into Puzzle Mode upon the triggering of puzzleSwitchDetected, a UnityEvent defined in ViewManager.cs.
    /// </summary>
    public void onLoadingScreenTriggered()
    {
        TransitionToState(GameState.Loading);
    }

    /// <summary>
    /// Pretty self-explantory. Quits the application.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    public void TogglePause(bool isPausable)
    {
        pausable = isPausable;
    }
}