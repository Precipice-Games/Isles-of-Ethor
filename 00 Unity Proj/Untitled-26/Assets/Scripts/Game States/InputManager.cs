using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;
using Sirenix.OdinInspector;

public class InputManager : MonoBehaviour
{
    // Get a reference to the PlayerInput component
    public static PlayerInput playerInput { get; private set; }
    // [SerializeField] private PlayerInput playerInput;
    
    // Invoked whenever the input map is switched. This allows other
    // scripts to subscribe to this event and update their references
    // to the current input map as needed.
    public static event Action<string> inputMapSwitched;
    public static event Action<CursorLockMode, bool> cursorChanged;
    public static event Action leftClickEvent;
    public static event Action mousePointerMoved;

    [Space]
    [Title("Debugging Options", "Settings for quick debugging options.")]
    [PropertyTooltip("Print out messages regarding the Player's relation to this manager. False by default.")]
    public bool printPlayerInfo = false;

    private void Awake()
    {
        // Initialize the PlayerInput component and
        // assign it to the static property
        AcquirePlayerInputReference();
    }
    
    // Subscribe to events
    private void OnEnable()
    {
        // Re-acquire the PlayerInput reference in case scene changed
        AcquirePlayerInputReference();
        
        // Always subscribe to the event, we'll check for null in FindCorrectActionMap
        GameStateManager.transitionedToNewState += FindCorrectActionMap;
        GameStateManager.transitionedToNewState += ChangeCursorFunctionality;
        GameStateManager.dialogueToggled += OnDialogueOverlayToggled;
    }

    // Unsubscribe from events
    private void OnDisable()
    {
        GameStateManager.transitionedToNewState -= FindCorrectActionMap;
        GameStateManager.transitionedToNewState -= ChangeCursorFunctionality;
        GameStateManager.dialogueToggled -= OnDialogueOverlayToggled;
    }
    
    private void OnDialogueOverlayToggled(bool active)
    {
        if(playerInput == null) return;

        var uiActionMap = playerInput.actions?.FindActionMap("UI");
        if (uiActionMap == null) return;

        if (active)
        {
            uiActionMap.Enable();
            cursorChanged?.Invoke(CursorLockMode.None, true);
        }
        else
        {
            uiActionMap.Disable();
            cursorChanged?.Invoke(CursorLockMode.None, true);
        }
    }

    /// <summary>
    /// Gets the PlayerInput reference from the Player singleton instance.
    /// </summary>
    private void AcquirePlayerInputReference()
    {
        if (Player.Instance == null)
        {
            if (printPlayerInfo) Debug.LogWarning("InputManager.cs >> Can't find the Player instance. Make sure it is in the scene with Player.cs attached.");
            playerInput = null;
        }
        else
        {
            playerInput = Player.Instance.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                if (printPlayerInfo) Debug.Log("InputManager.cs >> Successfully acquired PlayerInput reference.");
            }
            else
            {
                if (printPlayerInfo) Debug.LogWarning("InputManager.cs >> Player instance found but PlayerInput component is missing!");
            }
        }
    }
    
    // Switches the current action map to the specified action map name
    private void FindCorrectActionMap(GameStateManager.GameState newState)
    {
        if (playerInput != null)
        {
            string actionMapName = "UI"; // Default to UI, will be overridden in switch statement
            
            switch (newState) {
                case GameStateManager.GameState.MainMenu:
                    actionMapName = "UI";
                    break;
                case GameStateManager.GameState.Exploration:
                    actionMapName = "Player";
                    break;
                case GameStateManager.GameState.Puzzle:
                    actionMapName = "Puzzle";
                    // TODO: We should probably create a new action map
                    //       specifically for puzzle solving, even if it
                    //       shares similar inputs to the Player action map.
                    break;
                case GameStateManager.GameState.Dialogue:
                    actionMapName = "UI";
                    break;
                case GameStateManager.GameState.Paused:
                    actionMapName = "UI";
                    break;
                case GameStateManager.GameState.Settings:
                    actionMapName = "UI";
                    break;
            }
            
            inputMapSwitched?.Invoke(actionMapName);
        }
    }
    
    // Switches the current action map to the specified action map name
    private void ChangeCursorFunctionality(GameStateManager.GameState newState)
    {
        if (playerInput != null)
        {
            CursorLockMode lockMode = CursorLockMode.None; // Default to unlocked
            bool visible = true; // Default to visible

            // Cursor should only be locked and invisible during Exploration mode.
            if (newState != GameStateManager.GameState.Exploration)
            {
                lockMode = CursorLockMode.None;
                visible = true;
            }
            else
            {
                lockMode = CursorLockMode.Locked;
                visible = false;
            }
            
            cursorChanged?.Invoke(lockMode, visible);
        }
    }
    
    /// <summary>
    /// This is subscribed to the left click input action in the Puzzle action map.
    /// It is used to invoke the left click event, which TileSelector.cs is
    /// subscribed to. This is important because there is one TileSelector.cs per
    /// puzzle, not one per scene, so we can't handle the callback explicitly.
    /// </summary>
    /// <param name="context"></param>
    public void LeftClickDetected(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Invoke leftClickEvent for any subscribers
            leftClickEvent?.Invoke();
        }
    }
    
    /// <summary>
    /// This is subscribed to the pointer input action in the Puzzle action map.
    /// It is used to invoke mousePointerMoved, which TileSelector.cs is
    /// subscribed to. Like the method above, this is important because there
    /// is one TileSelector.cs per puzzle, not one per scene, so we can't
    /// handle the callback explicitly.
    /// </summary>
    /// <param name="context"></param>
    public void PointerMovementDetected(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Invoke mousePointerMoved for any subscribers
            mousePointerMoved?.Invoke();
        }
    }
}