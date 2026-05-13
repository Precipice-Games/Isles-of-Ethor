using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// Inherits from GameStateManager. 
/// ViewManager is responsible for managing the game's UI and camera views based on game state. 
/// This script includes the methods for changing the UI for each game state.
/// It also includes listeners for unity events that trigger state changes.
/// TO DO: add logic for changing camera views between exploration and puzzle states.
/// </summary>
public class ViewManager : MonoBehaviour
{
    [Title("UI Canvases", "Canvases used to drive the game's UI experience. Please do not assign the puzzleCanvas, as it will be assigned dynamically.")]
    private List<GameObject> uiCanvases;
    public GameObject mainMenuUI;
    public GameObject explorationUI;
    public GameObject puzzleUI;
    public GameObject dialogueUI;
    public GameObject pausedUI;
    public GameObject settingsUI;
    public GameObject loadingUI;
    private VideoPlayer loadingVideoPlayer;
    private GameObject _targetUI;

    [Space]
    [Title("Cameras", "Cinemachine Cameras used to drive the game experience. Please do not assign the puzzleCamera, as it will be assigned dynamically.")]
    private List<CinemachineCamera> cameras;
    public CinemachineCamera playerCamera;
    public CinemachineCamera puzzleCamera;
    public CinemachineCamera dialogueCamera;
    public CinemachineCamera menuCamera;
    private CinemachineCamera _targetCamera;

    [Space]
    [Title("Puzzle Triggering Event", "Event fired when the Player interacts with an InteractablePillar.")]
    public UnityEvent puzzleSwitchDetected;
    
    [Space]
    [Title("Loading Event", "Event fired when the loading screen animation has played.")]
    public UnityEvent loadingComplete;

    // Static event to notify subscribers of game state changes
    public static event Action<bool> toggleDepthOfField;
    public static event Action<bool> toggleBloom;
    
    [Space]
    [Title("Debugging Options", "Settings for quick debugging options.")]
    [PropertyTooltip("Print out messages for scene initialization. True by default.")]
    public bool printSceneDefaults = false;
    [PropertyTooltip("Print out canvas updates. False by default.")]
    public bool printCanvasUpdate = false;
    [PropertyTooltip("Print out camera updates. False by default.")]
    public bool printCameraUpdate = false;

    public bool fadePuzzleTransition = false;
    public Image transitionCanvasColor;
    public float fadeInFadeOutInBetween = 0.5f;
    public float currentFadeInFadeOutTime = 0f;

    private void Awake()
    {
        
        // Initialize cameras list if null
        if (cameras == null)
        {
            // cameras = new List<Camera>();
            cameras = new List<CinemachineCamera>();
        }
        
        // Clear and rebuild the list to ensure it's up to date
        cameras.Clear();
        
        // Add present cameras to cameras list
        if (playerCamera != null) cameras.Add(playerCamera);
        if (puzzleCamera != null) cameras.Add(puzzleCamera);
        if (dialogueCamera != null) cameras.Add(dialogueCamera);
        if (menuCamera != null) cameras.Add(menuCamera);
        
        if (printSceneDefaults) Debug.Log($"ViewManager.cs >> Initialized with {cameras.Count} cameras.");
        
        // Initialize UI canvases list if null
        if (uiCanvases == null)
        {
            uiCanvases = new List<GameObject>();
        }
        
        // Clear and rebuild the list to ensure it's up to date
        uiCanvases.Clear();
        
        // Add present UI Canvases to uiCanvases list
        if (mainMenuUI != null) uiCanvases.Add(mainMenuUI);
        if (explorationUI != null) uiCanvases.Add(explorationUI);
        if (puzzleUI != null) uiCanvases.Add(puzzleUI);
        if (pausedUI != null) uiCanvases.Add(pausedUI);
        if (dialogueUI != null) uiCanvases.Add(dialogueUI);
        if (loadingUI != null)
        {
            uiCanvases.Add(loadingUI);
            loadingVideoPlayer = loadingUI.GetComponentInChildren<VideoPlayer>();
        }
        
        if (printSceneDefaults) Debug.Log($"ViewManager.cs >> Initialized with {uiCanvases.Count} UI Canvases.");
    }

    // Subscribe to events
    private void OnEnable()
    {
        GameStateManager.transitionedToNewState += HandleViewChange;
        GameStateManager.transitionedToNewState += HandlePostProcessor;
        GameStateManager.dialogueToggled += OnDialogueOverlayToggled;
        RuneCircle.puzzleTriggered += UpdatePuzzleInformation;
        loadingVideoPlayer.loopPointReached += OnLoadingVideoFinished;
    }
    
    // Unsubscribe from events
    private void OnDisable()
    {
        GameStateManager.transitionedToNewState -= HandleViewChange;
        GameStateManager.transitionedToNewState -= HandlePostProcessor;
        GameStateManager.dialogueToggled -= OnDialogueOverlayToggled;
        RuneCircle.puzzleTriggered -= UpdatePuzzleInformation;
        loadingVideoPlayer.loopPointReached -= OnLoadingVideoFinished;
    }

    private void OnDialogueOverlayToggled(bool active)
    {
        if (active)
        {
            // If the dialogue overlay is active, we want to switch to the dialogue camera and UI
            dialogueUI.SetActive(true);
            //HandleCameraChange(dialogueCamera);
        }
        else
        {
            // If the dialogue overlay is not active, we want to switch back to the exploration camera and UI
            dialogueUI.SetActive(false);
            //HandleCameraChange(playerCamera);
        }
    }

    /// <summary>
    /// This method takes in the PuzzleInformation data from the InteractablePillar.cs script.
    /// The parameter is used to update the puzzleUI and puzzleCamera accordingly. Finally, the
    /// game state will be transitioned to the Puzzle state, which will trigger the normal game
    /// state change logic.
    /// </summary>
    /// <param name="puzzleInfo">Information about the puzzle being switched to.</param>
    private void UpdatePuzzleInformation(PuzzleInformation puzzleInfo)
    {
        puzzleCamera = puzzleInfo.camera;
        cameras.Add(puzzleCamera);
        puzzleUI = puzzleInfo.canvas;
        uiCanvases.Add(puzzleUI);

        fadePuzzleTransition = true;

    }

    /// <summary>
    /// Handles logic regarding what UI and camera to toggle.
    /// </summary>
    /// <remarks>
    /// Once we've transitioned to the new game state, this method will figure out what the new UI and camera should be based on the new game state, and then call the methods to enable the correct UI and camera while disabling all others.
    /// </remarks>
    private void HandleViewChange(GameStateManager.GameState newState)
    {
        // Set the current UI based on the new game state
        switch (newState)
        {
            case GameStateManager.GameState.MainMenu:
                _targetUI = mainMenuUI;
                _targetCamera = menuCamera;
                break;
            case GameStateManager.GameState.Exploration:
                _targetUI = explorationUI;
                _targetCamera = playerCamera;
                break;
            case GameStateManager.GameState.Puzzle:
                _targetUI = puzzleUI;
                _targetCamera = puzzleCamera;
                break;
            case GameStateManager.GameState.Dialogue:
                _targetUI = dialogueUI;
                _targetCamera = dialogueCamera;
                break;
            case GameStateManager.GameState.Paused:
                _targetUI = pausedUI;
                _targetCamera = menuCamera;
                break;
            case GameStateManager.GameState.Settings:
                _targetUI = settingsUI;
                _targetCamera = menuCamera;
                break;
            case GameStateManager.GameState.Loading:
                _targetUI = loadingUI;
                _targetCamera = menuCamera;
                break;
        }
        
        HandleCanvasChange(_targetUI);
        HandleCameraChange(_targetCamera);
    }
    
    /// <summary>
    /// Handles UI Canvas switching.
    /// </summary>
    /// <remarks> 
    /// This method enables the correct UI and disables all others.
    /// </remarks>
    private void HandleCanvasChange(GameObject targetCanvas)
    {
        foreach (GameObject canvas in uiCanvases)
        {
            if (canvas != null)
            {
                if (canvas == targetCanvas)
                {
                    canvas.SetActive(true);
                    if (printCanvasUpdate) Debug.Log($"ViewManager.cs >> Enabled UI Canvas: {canvas.name}");
                    CheckIfLoadingScreen(canvas);
                }
                else
                {
                    canvas.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// If we just switched to the loading screen, ensure that the video is playing.
    /// </summary>
    /// <param name="canvas"></param>
    private void CheckIfLoadingScreen(GameObject canvas)
    {
        if (canvas == loadingUI)
        {
            Debug.Log("The loading screen is active. Playing loading video...");
            loadingVideoPlayer.Play();
        }
    }
    
    /// <summary>
    /// Handles camera switching.
    /// </summary>
    /// <remarks> 
    /// This method enables the correct camera and disables all others.
    /// </remarks>
    private void HandleCameraChange(CinemachineCamera targetCamera)
    {
        foreach (CinemachineCamera cam in cameras)
        {
            if (cam != null)
            {
                if (cam == targetCamera)
                {
                    // Set the priority of the target camera
                    // higher than the others to make it active
                    cam.Priority = 10;
                    cam.gameObject.SetActive(true);
                    if (printCameraUpdate) Debug.Log($"ViewManager.cs >> Enabled camera: {cam.name}");
                }
                else
                {
                    cam.Priority = 0;
                    cam.gameObject.SetActive(false);
                }
            }
        }
    }
    
    /// <summary>
    /// Used to update the post processor according to the game state. Fires the
    /// togglePostProcessor event, which is picked up by PostProcessorManager.cs.
    /// </summary>
    /// <param name="newState"></param>
    private void HandlePostProcessor(GameStateManager.GameState newState)
    {
        // Set the post processing effects based on the new game state
        switch (newState)
        {
            case GameStateManager.GameState.MainMenu:
                toggleDepthOfField?.Invoke(false);
                toggleBloom?.Invoke(false);
                break;
            case GameStateManager.GameState.Exploration:
                toggleDepthOfField?.Invoke(false);
                toggleBloom?.Invoke(false);
                break;
            case GameStateManager.GameState.Puzzle:
                toggleDepthOfField?.Invoke(false);
                toggleBloom?.Invoke(true);
                break;
            case GameStateManager.GameState.Dialogue:
                toggleDepthOfField?.Invoke(false);
                toggleBloom?.Invoke(false);
                break;
            case GameStateManager.GameState.Paused:
                toggleDepthOfField?.Invoke(true);
                toggleBloom?.Invoke(false);
                break;
            case GameStateManager.GameState.Settings:
                toggleDepthOfField?.Invoke(false);
                toggleBloom?.Invoke(false);
                break;
            case GameStateManager.GameState.Loading:
                toggleDepthOfField?.Invoke(false);
                toggleBloom?.Invoke(false);
                break;
        }
        
        // if (newState == GameStateManager.GameState.Paused)
        // {
        //     toggleDepthOfField?.Invoke(true);
        // }
        // else
        // {
        //     toggleBloom?.Invoke(false);
        // }
        //
        // if (newState == GameStateManager.GameState.Puzzle)
        // {
        //     toggleBloom.Invoke(true);
        // }
        // else
        // {
        //     toggleBloom.Invoke(false);
        // }
    }
    
    /// <summary>
    /// When the loading screen animation is done, stop the video and invoke
    /// loadingComplete, which is picked up by GameStateManager.cs to transition
    /// to the next scene.
    /// </summary>
    /// <param name="vp"></param>
    private void OnLoadingVideoFinished(VideoPlayer vp)
    {
        loadingVideoPlayer.Stop();
        loadingVideoPlayer.time = 0;
        loadingComplete?.Invoke();
    }

    private void FixedUpdate()
    {

        if (fadePuzzleTransition)
        {
            //transitionCanvasColor = GameObject.Find("TransitionCanvas").GetComponent<Image>();
            transitionCanvasColor.gameObject.SetActive(true);

            if (transitionCanvasColor != null && transitionCanvasColor.color.a < 1)
            {
                Color newColor = transitionCanvasColor.color;

                newColor.a = newColor.a + Time.deltaTime;

                transitionCanvasColor.color = newColor; //fade in over time

            }
            else
            {

                //timer in between fade in and out to give some visual rest
                if (currentFadeInFadeOutTime < fadeInFadeOutInBetween)
                {
                    currentFadeInFadeOutTime += Time.deltaTime;
                }
                else
                {
                    fadePuzzleTransition = false;
                    // Notify GameStateManager that we've detected a puzzle switch.
                    puzzleSwitchDetected.Invoke();
                }

            }

        }
        else
        {

            if (transitionCanvasColor != null && transitionCanvasColor.color.a >= 0)
            {

                Color newColor = transitionCanvasColor.color;

                newColor.a = newColor.a - Time.deltaTime;

                transitionCanvasColor.color = newColor; //fade out over time

            }
            else if (transitionCanvasColor.gameObject.activeInHierarchy && transitionCanvasColor.color.a <= 0)
            {
                transitionCanvasColor.gameObject.SetActive(false);
                currentFadeInFadeOutTime = 0;
            }
        }

    }

}