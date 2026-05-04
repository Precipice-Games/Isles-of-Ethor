using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

// NOTE: The [ExecuteAlways] should ONLY ever be uncommented to accurately place
// puzzles in the scene. This is because tiles are distributed at runtime, so it's
// hard to know where they will actually end up. However, for accurate testing of
// the puzzle system, this could be commented out since it affects the resource
// system and other data.
//
// [ExecuteAlways]
public class GridManager : MonoBehaviour
{
    [Title("Grid Settings", "Settings for the puzzle grid.")]
    [PropertyTooltip("Grid Width.")]
    public int width = 4;
    [PropertyTooltip("Grid Height.")]
    public int height = 3;
    [PropertyTooltip("Size of tiles.")]
    public float tileSize = .25f;
    
    [Space]
    [Title("Debugging Options", "Settings for quick debugging options.")]
    [PropertyTooltip("Print out occupancy map info. False by default.")]
    public bool printOccupancyMap = false;

    // Snapshot of occupancy state (true = occupied, false = vacant)
    private Dictionary<Vector2Int, bool> initialOccupancy = new();

    // Grid of selectable tiles
    private SelectableTile[,] grid;

    // Subscribe to events
    private void OnEnable()
    {
        ResetPuzzle.resetPuzzle += ResetGridData;
    }

    // Unsubscribe from events
    private void OnDisable()
    {
        ResetPuzzle.resetPuzzle -= ResetGridData;
    }

    void Awake()
    {
        // An additional 2 rows and columns are added to the
        // grid to allow for the start and end rune tiles.
        width += 2;
        height += 2;
        grid = new SelectableTile[width, height];
        CreateOccupancyMap(width, height);
    }

    /// <summary>
    /// For grid initialization, we want to create a snapshot of the occupancy state of the grid.
    /// This will allow us to reset the grid to its initial state when the puzzle is reset.
    /// </summary>
    private void CreateOccupancyMap(int w, int h)
    {
        for (int x = 0; x < w; x++)
        {
            for (int z = 0; z < h; z++)
            {
                Vector2Int coords = new Vector2Int(x, z);
                initialOccupancy[coords] = false;
            }
        }
    }

    /// <summary>
    /// Called during initial tile placement to update the occupancy map with the
    /// initial state of the grid.
    /// </summary>
    /// <param name="coords"></param>
    /// <param name="occupancy"></param>
    private void UpdateOccupancyMap(Vector2Int coords, bool occupancy)
    {
        initialOccupancy[coords] = occupancy;
        if (printOccupancyMap) Debug.Log($"GridManager.cs >> Updated occupancy map at {coords} to {occupancy}");
    }

    /// <summary>
    /// Check if the movement action is within the grid.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool IsInsideGrid(int x, int z)
    {
        return x > 0 && x < width - 1 && z > 0 && z < height - 1;
    }

    /// <summary>
    /// Check if movement action is within grid for player. 
    /// This is used to allow the player to move onto the start and end tiles, outside the area allowed for other tiles.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool IsInsideGridPlayer(int x, int z)
    {
        return x >= 0 && x < width && z >= 0 && z < height;
    }

    /// <summary>
    /// Check if a cell is vacant. If it is, return true.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public bool IsCellEmpty(int x, int z)
    {
        return grid[x, z] == null;
    }

    /// <summary>
    /// Returns the TileType of a given tile.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public SelectableTile.TileType GetTileType(int x, int z)
    {
        SelectableTile.TileType type = grid[x, z].tileType;
        return type;
    }

    /// <summary>
    /// Place a tile in the grid at the given coordinates.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void PlaceTile(SelectableTile tile, int x, int z)
    {
        grid[x, z] = tile;
    }

    /// <summary>
    /// Place a tile in the grid at the given coordinates during puzzle initialization.
    /// This method also updates the occupancy map to reflect the initial state of the grid.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void InitialTilePlacement(SelectableTile tile, int x, int z)
    {
        grid[x, z] = tile;
        Vector2Int coords = new Vector2Int(x, z);
        UpdateOccupancyMap(coords, true);
    }

    /// <summary>
    /// Clear the cell at the given coordinates. This is used when a tile
    /// moves out of its current cell and into a new one.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void ClearCell(int x, int z)
    {
        grid[x, z] = null;
    }

    /// <summary>
    /// Converts grid coordinates to world coordinates. This is used to
    /// position the tiles in the world based on their grid location.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Vector3 GridToWorld(int x, int z)
    {
        return new Vector3(x * tileSize, 0, z * tileSize);
    }

    /// <summary>
    /// Resets data of which cells are empty vs occupied using the initial occupany map.
    /// </summary>
    private void ResetGridData()
    {
        foreach (KeyValuePair<Vector2Int, bool> entry in initialOccupancy)
        {
            Vector2Int coords = entry.Key;
            bool occupied = entry.Value;

            if (occupied)
            {
                if (printOccupancyMap) Debug.Log($"GridManager.cs >> Cell at {coords} in {transform.root.name} is OCCUPIED.");
            }
            else
            {
                if (printOccupancyMap) Debug.Log($"GridManager.cs >> Cell at {coords} in {transform.root.name} is VACANT.");
                ClearCell(coords.x, coords.y);
            }
        }
    }
}