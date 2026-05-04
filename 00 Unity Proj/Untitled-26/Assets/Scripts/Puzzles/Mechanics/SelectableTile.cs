using System;
using Sirenix.OdinInspector;
using UnityEngine;

// NOTE: The [ExecuteAlways] should ONLY ever be uncommented to accurately place
// puzzles in the scene. This is because tiles are distributed at runtime, so it's
// hard to know where they will actually end up. However, for accurate testing of
// the puzzle system, this could be commented out since it affects the resource
// system and other data.
//
// [ExecuteAlways]
public class SelectableTile : MonoBehaviour
{
    public enum TileType
    {
        Normal,
        Ice,
        ManaWell
    }

    // Default tile type is Normal
    public TileType tileType = TileType.Normal;

    private int startingGridX;
    private int startingGridZ;
    public int gridX;
    public int gridZ;
    public Tuple<int, int> coordinates => Tuple.Create(gridX, gridZ);

    // Reference to the GridManager for this specific puzzle
    public GridManager gridManager;
    private Renderer rend;

    // Making originalColor public for now to tell
    // which are the start and end tiles.
    public Color originalColor;
    public Color highlightColor = Color.yellow;

    [Space]
    [Title("Debugging Options", "Settings for quick debugging options.")]
    [PropertyTooltip("Print out the starting position of a tile. True by default.")]
    public bool printStartingPosition;

    // Event fired when a move is blocked by an occupied cell
    public static event Action<SelectableTile> cellOccupied;

    // Event fired when a move is out of bounds
    public static event Action<SelectableTile> moveOutOfBounds;

    // Subscribe to events
    private void OnEnable()
    {
        if (Application.isPlaying && printStartingPosition)
        {
            Debug.Log($"SelectableTile.cs >> {name} starting at: ({gridX},{gridZ})");
        }

        ResetPuzzle.resetPuzzle += ResetTiles;
    }

    // Unsubscribe from events
    private void OnDisable()
    {
        ResetPuzzle.resetPuzzle -= ResetTiles;
    }

    void Start()
    {
        rend = GetComponent<Renderer>();

        // Ensure we have the GridManager reference since this script is
        // using [ExecuteAlways] and we need that reference before runtime.
        if (gridManager == null) gridManager = GetComponent<GridManager>();

        // Set color based on tile type
        // For now, ManaWell tiles are purple so we can test them visually.
        if (tileType == TileType.ManaWell)
        {
            // rend.material.color = Color.magenta;
            rend.sharedMaterial.color = Color.magenta;
        }

        // Changed to rend.SharedMaterial to prevent memory leaks
        // in the scene since this script is using [ExecuteAlways].
        originalColor = rend.sharedMaterial.color;
        rend.sharedMaterial.color = originalColor;

        startingGridX = gridX;
        startingGridZ = gridZ;

        // if (printPosition) Debug.Log($"SelectableTile.cs >> {name} starting at: ({gridX},{gridZ})");

        gridManager.InitialTilePlacement(this, gridX, gridZ);
        transform.localPosition = gridManager.GridToWorld(gridX, gridZ);
    }

    // Run when a tile is selected
    public void Select()
    {
        rend.material.color = highlightColor;
        Debug.Log($"SelectableTile.cs >> {name} SELECTED at: ({gridX},{gridZ})");
    }

    // Run when a tile is deselected
    public void Deselect()
    {
        rend.material.color = originalColor;
        Debug.Log($"SelectableTile.cs >> {name} DESELECTED");
    }

    public bool TryMove(int xDir, int zDir)
    {
        int newX = gridX + xDir;
        int newZ = gridZ + zDir;

        Debug.Log($"SelectableTile.cs >> {name} trying move to: ({newX},{newZ})");

        // Before anything, check to see if the attempted move is valid.
        if (CheckForOutOfBounds(newX, newZ)) return false; // Must be inside the grid
        if (!CheckForEmptyCell(newX, newZ)) return false; // Must be an empty cell

        gridManager.ClearCell(gridX, gridZ);

        gridX = newX;
        gridZ = newZ;

        gridManager.PlaceTile(this, gridX, gridZ);

        transform.localPosition = gridManager.GridToWorld(gridX, gridZ);

        Debug.Log($"SelectableTile.cs >> {name} moved to: ({gridX},{gridZ})");

        return true;
    }

    /// <summary>
    /// Checks for an empty cell.
    /// </summary>
    /// <param name="coordX"></param>
    /// <param name="coordZ"></param>
    private bool CheckForEmptyCell(int coordX, int coordZ)
    {
        // If it's empty, return true.
        if (gridManager.IsCellEmpty(coordX, coordZ))
        {
            return true;
        }

        Debug.Log($"SelectableTile.cs >> BLOCKED: Cell at ({coordX},{coordZ}) is occupied.");

        // Fire off an event to say that a cell is occupied
        cellOccupied?.Invoke(this);
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
        if (!gridManager.IsInsideGrid(coordX, coordZ))
        {
            Debug.Log("SelectableTile.cs >> BLOCKED: Outside grid – no mana spent");

            // Fire off an event to say that the move is out of bounds
            moveOutOfBounds?.Invoke(this);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Resets tiles to their original positions. This is called when the ResetPuzzle event is triggered.
    /// </summary>
    private void ResetTiles()
    {
        gridX = startingGridX;
        gridZ = startingGridZ;

        transform.localPosition = gridManager.GridToWorld(gridX, gridZ);

        // Reset tile color after reset
        if (tileType == TileType.ManaWell)
        {
            originalColor = Color.magenta;
        }

        rend.material.color = originalColor;
    }
}