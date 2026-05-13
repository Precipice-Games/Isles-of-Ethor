#if UNITY_EDITOR
using UnityEditor.Build;
#endif

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.PlayerLoop;

// This script is used to snap the Player in a fixed movement style during puzzle mode.
// It is similar to the PlayerMovement.cs script, but it is used to snap the player to
// a grid and only allow movement in four directions (up, down, left, right) instead of
// allowing for diagonal movement.

public class PlayerFixedMovement : MonoBehaviour
{
    // ==== Variables =====
    private PuzzleInformation puzzleInfo;
    private Vector3 playerCurrentPosition; // Current Vector3 position
    private Vector3 startPosition;
    private Vector3 endPosition;
    private GridManager gridManager;
    private ResourceManager resourceManager;
    private GameObject puzzleCam; // The camera object holds relevant scripts

    // Tile that the Player will start at
    // (Not necessarily within the grid)
    private GameObject startTile;
    private GameObject endTile;

    [Title("Player's Grid Coordinates")]
    [SerializeField] private int playerGridX;
    [SerializeField] private int playerGridZ;

    // The starting and ending tile's coordinates
    private int startTileX;
    private int startTileZ;
    private int endTileX;
    private int endTileZ;

    //Keeps track of the potential tile for the player to move to
    /// <summary>
    /// The X coordinate of the tile the Player is attempting to move to. This is used to check if the move is valid.
    /// </summary>
    private int destinationX = 0;

    /// <summary>
    /// The Y coordinate of the tile the Player is attempting to move to. This is used to check if the move is valid.
    /// </summary>
    private int destinationZ = 0;

    private int lastTileX = int.MinValue;
    private int lastTileZ = int.MinValue;

    private int deltaX;
    private int deltaZ;

    private int newDeltaX;
    private int newDeltaZ;

    private int attemptedDestX;
    private int attemptedDestZ;

    private int landingX = 0;
    private int landingZ = 0;

    // The new coordinates as a Vector3
    Vector3 newCoords;
    Vector3 newPosition;
    Vector3 newYAxis;

    // Player's Rigidbody
    private Rigidbody rb;

    // Track status of ManaWell trigger to prevent multiple activations in one puzzle attempt.
    private bool manaWellTriggeredThisEntry = false;

    [Space]
    [Title("Debugging Options", "Settings for quick debugging options.")]
    [PropertyTooltip("Print out what move action was taken. True by default.")]
    public bool printMoveAction = true;

    [PropertyTooltip("Print out the Player's current grid coordinates are (X,Z). True by default.")]
    public bool printPlayerGridCoords = true;

    [PropertyTooltip("Print out the Player's current grid coordinates are as a Vector3. False by default.")]
    public bool printPlayerVector3 = false;

    [PropertyTooltip("Print out the tile type of the attempted tile the Player tried to move to. False by default.")]
    public bool printAttemptedTileType = false;

    // Static event to notify subscribers of the Player's movement
    public static event Action<int, int> playerMoved;

    [Space]
    [Title("Puzzle Completion Event", "Event fired when Player reaches the end tile of the puzzle.")]
    public UnityEvent puzzleCompleted;
    public static event Action<PuzzleInformation> updatePuzzleStatus;

    // This event is fired when the Player attempts an invalid move.
    public static event Action<string, SelectableTile> playerInvalidMove;

    public float maxTime = 0.5f;
    public float currTime = 0.0f;
    public bool runTimer = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Subscribe to events
    private void OnEnable()
    {
        RuneCircle.puzzleTriggered += UpdatePuzzleInformation;
        ResetPuzzle.resetPuzzle += ResetPlayerPosition;
    }
    // Unsubscribe from events
    private void OnDisable()
    {
        RuneCircle.puzzleTriggered -= UpdatePuzzleInformation;
        ResetPuzzle.resetPuzzle -= ResetPlayerPosition;
    }

    /// <summary>
    /// This method is called on the puzzleTriggered event. It receives data
    /// regarding the current puzzle, such as the starting and ending tiles.
    /// This data is packaged and sent from the InteractablePillar.cs script.
    /// </summary>
    /// <param name="info"></param>
    private void UpdatePuzzleInformation(PuzzleInformation info)
    {
        puzzleInfo = info;
        gridManager = puzzleInfo.gridManager.GetComponent<GridManager>();
        resourceManager = puzzleInfo.resourceManager.GetComponent<ResourceManager>();
        startTile = puzzleInfo.startTile;
        endTile = puzzleInfo.endTile;

        transform.parent = puzzleInfo.gameObject.transform.GetChild(0);

        // Get the grid coordinates of the starting tile
        startTileX = startTile.GetComponent<SelectableTile>().gridX;
        startTileZ = startTile.GetComponent<SelectableTile>().gridZ;

        Debug.Log($"PlayerFixedMovement.cs >> Starting X,Z: ({startTileX},{startTileZ})");

        // Get the grid coordinates of the end tile
        endTileX = endTile.GetComponent<SelectableTile>().gridX;
        endTileZ = endTile.GetComponent<SelectableTile>().gridZ;

        // Reset tile info upon new puzzle start
        playerGridX = 0;
        playerGridZ = 0;

        destinationX = 0;
        destinationZ = 0;

        manaWellTriggeredThisEntry = false;

        // After gathering data, move Player to the startTile
        SetPlayersYPosition(startTile);
        MovePlayerToStartTile(startTileX, startTileZ);
    }

    /// <summary>
    /// This method simply moves the Player to the starting tile. It was created as a
    /// way to break down the complexity of the movement system itself. We may want to
    /// optimize it later on by reducing code redundancy, but for now, it is functional
    /// and should probably be kept for bug testing.
    /// </summary>
    /// <param name="startX"></param>
    /// <param name="startZ"></param>
    private void MovePlayerToStartTile(int startX, int startZ)
    {
        Debug.Log($"PlayerFixedMovement.cs >> Moving Player to the starting tile ({startX},{startZ})...");
        SnapPlayerToTile(startX, startZ);
    }

    // The following methods listen to callback events from the Puzzle map from the
    // PlayerControls.inputactions asset. They are subscribed to these events via
    // the PlayerInput component in the Inspector menu of the Unity Editor. This is
    // the same methodology used for the freeform movement in PlayerMovement.cs.

    public void MoveUp(InputAction.CallbackContext context)
    {
        // Ensures that the action is only performed once per key press.
        if (!context.performed) return;

        if (printMoveAction) Debug.Log("PlayerFixedMovement.cs >> MoveUp performed.");
        MoveDirection(0, 1);
    }

    public void MoveDown(InputAction.CallbackContext context)
    {
        // Ensures that the action is only performed once per key press.
        if (!context.performed) return;

        if (printMoveAction) Debug.Log("PlayerFixedMovement.cs >> MoveDown called.");
        MoveDirection(0, -1);
    }

    public void MoveLeft(InputAction.CallbackContext context)
    {
        // Ensures that the action is only performed once per key press.
        if (!context.performed) return;

        if (printMoveAction) Debug.Log("PlayerFixedMovement.cs >> MoveLeft called.");
        MoveDirection(-1, 0);
    }

    public void MoveRight(InputAction.CallbackContext context)
    {
        // Ensures that the action is only performed once per key press.
        if (!context.performed) return;

        if (printMoveAction) Debug.Log("PlayerFixedMovement.cs >> MoveRight called.");
        MoveDirection(1, 0);
    }

    public void PuzzleReset(InputAction.CallbackContext context)
    {
        // Ensures that the action is only performed once per key press.
        if (!context.performed) return;

        Debug.Log("PlayerFixedMovement.cs >> PuzzleReset called.");
        ResetPuzzle.OnReset();
    }

    /// <summary>
    /// This method is used to reduce redundancy in the directional movement methods.
    /// </summary>
    /// <param name="xDir"></param>
    /// <param name="zDir"></param>
    private void MoveDirection(int xDir, int zDir)
    {
        // reset values of destination coords
        destinationX = 0;
        destinationZ = 0;
        TryToMovePlayer(xDir, zDir);
    }

    /// <summary>
    /// Attempts to move the Player in the specified direction on the puzzle grid.
    /// </summary>
    /// <param name="xDir"></param>
    /// <param name="zDir"></param>
    // TODO: Should refactor this to handle different tile types and mechanics. Right now,
    //       it's only checking for normal moves, but we will need additional code to handle
    //       special tiles, like the ice mechanic.
    public void TryToMovePlayer(int xDir, int zDir)
    {
        // Directional Coordinates (Up, Down, Left, Right)
        deltaX = xDir;
        deltaZ = zDir;

        Debug.Log($"PlayerFixedMovement.cs >> Directional Coordinates: ({xDir},{zDir})");

        destinationX += deltaX;
        destinationZ += deltaZ;

        // Calculate the attempted destination based on a move.
        // This is used just for the first tile.
        attemptedDestX = playerGridX + destinationX;
        attemptedDestZ = playerGridZ + destinationZ;

        Debug.Log($"PlayerFixedMovement.cs >> Attempted Destination Coordinates: ({attemptedDestX},{attemptedDestZ})");

        // Before anything, check to see if the attempted move is valid. This is performed
        // as though the Player is only traveling one tile. We want to ensure that first
        // tile is not empty and is within the bounds before doing anything else.
        if (CheckForOutOfBounds(attemptedDestX, attemptedDestZ)) return;
        if (CheckForEmptyCell(attemptedDestX, attemptedDestZ))
        {
            SnapPlayerToTile(startTileX, startTileZ);
            return;
        }

        // If the move is valid, we've reached this part of the code.
        // Check what type of tile the Player is attempting to move to.
        // The tile type determines the behavior of this move.
        switch (CheckAttemptedTileType(attemptedDestX, attemptedDestZ))
        {
            case SelectableTile.TileType.Normal:
                NormalTile(attemptedDestX, attemptedDestZ);
                break;

            case SelectableTile.TileType.Ice:
                IceTile(attemptedDestX, attemptedDestZ);
                break;

            case SelectableTile.TileType.ManaWell:
                NormalTile(attemptedDestX, attemptedDestZ);
                break;
        }
    }

    /// <summary>
    /// Returns the type of tile the Player is attempting to move to.
    /// </summary>
    /// <param name="coordX"></param>
    /// <param name="coordZ"></param>
    private SelectableTile.TileType CheckAttemptedTileType(int coordX, int coordZ)
    {
        Debug.Log($"PlayerFixedMovement.cs >> Current coordinates: ({playerGridX},{playerGridZ})");
        Debug.Log($"PlayerFixedMovement.cs >> The Player wants to move to: ({coordX},{coordZ})");
        Debug.Log("PlayerFixedMovement.cs >> Checking the type of tile at the attempted destination...");

        // Print out the type of tile that the Player is attempting to move to. This will
        // be used to determine how the Player should move and interact with the tile.
        SelectableTile.TileType tileType = gridManager.GetTileType(coordX, coordZ);

        if (printAttemptedTileType)
            Debug.Log($"PlayerFixedMovement.cs >> Tile at ({coordX},{coordZ}) is {tileType} tile.");

        return tileType;
    }

    /// <summary>
    /// Checks for an empty cell.
    /// </summary>
    /// <param name="coordX"></param>
    /// <param name="coordZ"></param>
    private bool CheckForEmptyCell(int coordX, int coordZ)
    {
        // Check if this cell is empty
        if (gridManager.IsCellEmpty(coordX, coordZ))
        {
            // Check if the Player attempted to slide over ice tiles without a normal tile at the end
            if (IsComingFromIceTile(coordX, coordZ))
            {
                Debug.Log($"PlayerFixedMovement.cs >> Ice slide ended: No tile at ({coordX},{coordZ})");
                string message = "You can't slide there! There is no Normal tile to stop your slide.";
                SnapPlayerToTile(startTileX, startTileZ);
                playerInvalidMove?.Invoke(message, null);
            }
            else
            {
                Debug.Log($"PlayerFixedMovement.cs >> There is no tile to jump to at: ({coordX},{coordZ})");
                string message = "There's no tile to move to!";
                playerInvalidMove?.Invoke(message, null);
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// This method assumes the Player tried to move into an empty cell. Its purpose is to specifically
    /// check if the tile before the empty cell was an ice tile.
    /// </summary>
    /// <param name="coordX"></param>
    /// <param name="coordZ"></param>
    private bool IsComingFromIceTile(int coordX, int coordZ)
    {
        // Check the previous tile based on current direction
        int prevTileX = coordX - deltaX;
        int prevTileZ = coordZ - deltaZ;

        if (gridManager.IsInsideGridPlayer(prevTileX, prevTileZ) && !gridManager.IsCellEmpty(prevTileX, prevTileZ))
        {
            return gridManager.GetTileType(prevTileX, prevTileZ) == SelectableTile.TileType.Ice;
        }

        return false;
    }

    /// <summary>
    /// Checks that a tile is out of bounds of the grid. If it's
    /// out of bounds, return true.
    /// </summary>
    /// <param name="coordX"></param>
    /// <param name="coordZ"></param>
    private bool CheckForOutOfBounds(int coordX, int coordZ)
    {
        // If it's out of bounds, return true.
        if (!gridManager.IsInsideGridPlayer(coordX, coordZ))
        {
            Debug.Log($"PlayerFixedMovement.cs >> Move blocked: ({coordX},{coordZ}) is outside the grid.");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Used to handle movement on normal tiles. This is the default tile type, so it is used as
    /// a baseline for the other tile types and mechanics. It simply checks for an empty cell and
    /// bounds before moving the Player to the new tile.
    /// </summary>
    private void NormalTile(int coordX, int coordZ)
    {
        // Update the landing coordinates and
        // Player coordinates based on move
        landingX = coordX;
        landingZ = coordZ;
        playerGridX = coordX;
        playerGridZ = coordZ;

        SnapPlayerToTile(landingX, landingZ);
    }

    /// <summary>
    /// Used to handle movement on ice tiles. This differs in that
    /// it handles recursion for the ice sliding mechanic.
    /// </summary>
    private void IceTile(int coordX, int coordZ)
    {
        Debug.Log($"PlayerFixedMovement.cs >> Recursively checking through ice tiles. Currently at ({coordX},{coordZ}).");

        // Create new values that can be passed into TryToMovePlayer()
        // without affecting the original destination coordinates. This is
        // necessary to ensure that the Player continues to slide in the
        // correct direction until they reach a non-ice tile or an obstacle.
        newDeltaX = deltaX;
        newDeltaZ = deltaZ;

        Debug.Log($"PlayerFixedMovement.cs >> newDeltaX/Z Coordinates: ({newDeltaX},{newDeltaZ})");

        newCoords = gridManager.GridToWorld(coordX, coordZ);
        newPosition = new Vector3(newCoords.x, transform.localPosition.y, newCoords.z);

        transform.localPosition = newPosition;

        Debug.Log("Run Timer");

        runTimer = true;

        //TryToMovePlayer(newDeltaX, newDeltaZ);
        
    }

    /// <summary>
    /// Adjust the Player's Y position to be slightly above the tile's Y position.
    /// This is used to prevent the Player from being below the tile.
    /// </summary>
    /// <param name="start"></param>
    public void SetPlayersYPosition(GameObject start)
    {
        float newY = start.transform.position.y + 0.05f; // Adjust the Y offset as needed
        newYAxis = new Vector3(transform.position.x, newY, transform.position.z);
        transform.position = newYAxis;
    }

    /// <summary>
    /// Snaps a Player to a tile based on the grid coordinates.
    /// </summary>
    /// <param name="coordX"></param>
    /// <param name="coordZ"></param>
    public void SnapPlayerToTile(int coordX, int coordZ)
    {
        // Grab the X and Z coordinates in Vector3 from the GridManager
        newCoords = gridManager.GridToWorld(coordX, coordZ);
        newPosition = new Vector3(newCoords.x, transform.localPosition.y, newCoords.z);

        transform.localPosition = newPosition;

        playerGridX = coordX;
        playerGridZ = coordZ;

        // Only runs if tile is changed
        if (coordX != lastTileX || coordZ != lastTileZ)
        {
            lastTileX = coordX;
            lastTileZ = coordZ;
            // Handle tile effects (ManaWell, etc.)
            CheckTileEffects(coordX, coordZ);
        }

        IsPlayerOnEndTile();
    }

    /// <summary>
    /// Handles special tile effects such as ManaWell.
    /// </summary>
    private void CheckTileEffects(int coordX, int coordZ)
    {
        SelectableTile.TileType tileType = gridManager.GetTileType(coordX, coordZ);

        if (tileType == SelectableTile.TileType.ManaWell)
        {
            if (manaWellTriggeredThisEntry) return;

            manaWellTriggeredThisEntry = true;

            Debug.Log("PlayerFixedMovement.cs >> Player stepped on ManaWell! +2 Mana");

            if (gridManager.GetTile(coordX, coordZ).GetComponentInChildren<ParticleSystem>() != null)
            {
            
                gridManager.GetTile(coordX, coordZ).GetComponentInChildren<ParticleSystem>().Stop();
                gridManager.GetTile(coordX, coordZ).GetComponentInChildren<ParticleSystem>().Clear();
            
            }
            
            gridManager.GetTile(coordX, coordZ).GetComponent<MeshRenderer>().material = gridManager.GetTile(coordX, coordZ).GetComponent<OasisTexturing>().oasisNormalTexture;
            
            if (resourceManager != null)
            {
                resourceManager.AddMana(2);
            }
        }
    }

    /// <summary>
    /// Used to check if the Player has reached the end tile. If so, the puzzle
    /// has been completed and the appropriate events can be triggered.
    /// </summary>
    private void IsPlayerOnEndTile()
    {
        Debug.Log("PlayerFixedMovement.cs >> Checking if Player is on the end tile...");
        Debug.Log($"PlayerFixedMovement.cs >> endTileX: {endTileX} | endTileZ: {endTileZ}");
        Debug.Log($"PlayerFixedMovement.cs >> playerGridX: " + playerGridX + " | playerGridZ: " + playerGridZ);
        
        
        // Check if the Player's coordinates match the end tile's coordinates
        if (endTileX == playerGridX && endTileZ == playerGridZ)
        {
            Debug.Log($"PlayerFixedMovement.cs >> Player has reached the end tile at ({endTileX}, {endTileZ}).");

            endPosition = new Vector3(
                endTile.transform.position.x,
                transform.position.y + 1.0f,
                endTile.transform.position.z
            );

            transform.parent = null;
            transform.position = endPosition;

            puzzleCompleted.Invoke(); // For the GameStateManager
            updatePuzzleStatus?.Invoke(puzzleInfo); // For the IslandPuzzleManager
        }

        playerMoved?.Invoke(playerGridX, playerGridZ);
    }

    /// <summary>
    /// Resets the Player's position to the starting tile. This is used for when the
    /// Reset Puzzle button is pressed.
    /// </summary>
    private void ResetPlayerPosition()
    {
        manaWellTriggeredThisEntry = false;

        // Get the grid coordinates of the starting tile
        startTileX = startTile.GetComponent<SelectableTile>().gridX;
        startTileZ = startTile.GetComponent<SelectableTile>().gridZ;

        Debug.Log("Starting X,Z: " + startTileX + "," + startTileZ);

        // After gathering data, move Player to the startTile
        SnapPlayerToTile(startTileX, startTileZ);
    }

    private void FixedUpdate()
    {
        if (runTimer)
        {
            if (currTime < maxTime)
            {

                currTime += Time.deltaTime;

            }
            else
            {

                runTimer = false;
                currTime = 0;
                TryToMovePlayer(newDeltaX, newDeltaZ);

            }
        }

    }

}