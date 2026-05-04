using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileSelector : MonoBehaviour
{
    [Title("Tile Selector Variables", "Variables used in the Tile Selection process.")]
    [PropertyTooltip("Please assign the ResourceManager for this specific puzzle prefab.")]
    public ResourceManager resourceManager;
    
    // Reference to the currently selected tile
    private SelectableTile selectedTile;
    
    // Reference to the Player's coordinates on the grid
    private int playerGridX;
    private int playerGridZ;

    // The starting and ending tile's coordinates
    private int startTileX;
    private int startTileZ;
    private int endTileX;
    private int endTileZ;
    
    [Space]
    [Title("Debugging Options", "Settings for quick debugging options.")]
    [PropertyTooltip("Prints out invalid moves. True by default.")]
    public bool printInvalidMoves = true;

    // Subscribe to events
    private void OnEnable()
    {
        PlayerFixedMovement.playerMoved += UpdatePlayerCoordinates;
        RuneCircle.puzzleTriggered += AssignStartAndEndTiles;
        InputManager.leftClickEvent += ClickDetected;
    }
    
    // Unsubscribe from events
    private void OnDisable()
    {
        PlayerFixedMovement.playerMoved -= UpdatePlayerCoordinates;
        RuneCircle.puzzleTriggered -= AssignStartAndEndTiles;
        InputManager.leftClickEvent -= ClickDetected;
    }

    /// <summary>
    /// This method is subscribed to the leftClickEvent invoked by InputManager.cs.
    /// This is because there is one TileSelector.cs per puzzle rather than one for
    /// the entire scene, so it cannot be subscribed explicitly.
    /// </summary>
    public void ClickDetected()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            SelectableTile tile = hit.collider.GetComponent<SelectableTile>();
            if (tile != null)
            {
                if (selectedTile != null)
                    selectedTile.Deselect();

                selectedTile = tile;
                selectedTile.Select();
            }
        }
    }

    /// <summary>
    /// Used to update the reference to the Player's coordinates on the grid. It
    /// ensures that we're not moving the tile that the Player is currently on.
    /// </summary>
    private void UpdatePlayerCoordinates(int playerX, int playerZ)
    {
        playerGridX = playerX;
        playerGridZ = playerZ;
    }

    /// <summary>
    /// Assigns the coordinates of the start and end tiles for the current puzzle.
    /// This is used to prevent the Player from moving these tiles.
    /// </summary>
    private void AssignStartAndEndTiles(PuzzleInformation puzzleInfo)
    {
        // Assign the Start & End tiles
        GameObject startTile = puzzleInfo.startTile;
        GameObject endTile = puzzleInfo.endTile;
        
        // Get the grid coordinates of the starting tile
        startTileX = startTile.GetComponent<SelectableTile>().gridX;
        startTileZ = startTile.GetComponent<SelectableTile>().gridZ;
        
        // Get the grid coordinates of the end tile
        endTileX = endTile.GetComponent<SelectableTile>().gridX;
        endTileZ = endTile.GetComponent<SelectableTile>().gridZ;
    }
    
    /// <summary>
    /// Checks if the Player is currently on the selected tile. If so, it prevents
    /// the system from continuing before even attempting to move the tile.
    /// </summary>
    private bool PlayerOnSelectedTile()
    {
        if (selectedTile.gridX == playerGridX && selectedTile.gridZ == playerGridZ)
        {
            if (printInvalidMoves) Debug.Log("TileSelector.cs >> Cannot the tile the Player is currently on.");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the current tile is either the start or end tile.
    /// </summary>
    private bool SelectedTileIsStartOrEnd()
    {
        // Check for Start tile
        if (selectedTile.gridX == startTileX && selectedTile.gridZ == startTileZ)
        {
            if (printInvalidMoves) Debug.Log("TileSelector.cs >> Cannot move the Start tile.");
            return true;
        }

        // Check for End tile
        if (selectedTile.gridX == endTileX && selectedTile.gridZ == endTileZ)
        {
            if (printInvalidMoves) Debug.Log("TileSelector.cs >> Cannot move the End tile.");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Used to move the currently selected tile to the right.
    /// </summary>
    public void MoveSelectedRight()
    {
        // Ensure that a tile is selected, and that we're not moving
        // the start tile, the end tile, or the Player-occupied tile.
        if (selectedTile == null) return;
        if (PlayerOnSelectedTile()) return;
        if (SelectedTileIsStartOrEnd()) return;

        // If we're out of Mana
        if (resourceManager.GetMana() <= 0)
        {
            resourceManager.OutOfMana();
            return;
        }

        // If we don't have enough Right move cards
        if (resourceManager.GetRightUses() <= 0)
        {
            resourceManager.NoMoreCardUses("Right");
            return;
        }

        // Only spend mana if the move actually succeeds
        if (selectedTile.TryMove(1, 0))
        {
            resourceManager.UseMove("Right");
        }
    }

    /// <summary>
    /// Used to move the currently selected tile to the left.
    /// </summary>
    public void MoveSelectedLeft()
    {
        // Ensure that a tile is selected, and that we're not moving
        // the start tile, the end tile, or the Player-occupied tile.
        if (selectedTile == null) return;
        if (PlayerOnSelectedTile()) return;
        if (SelectedTileIsStartOrEnd()) return;

        // Ensure there are enough resources before attempting the move.
        Debug.Log("TileSelector.cs >> Attempting to move left. Remaining uses: " + resourceManager.moveLeftUses + ", Remaining mana: " + resourceManager.GetMana());

        // If we're out of Mana
        if (resourceManager.GetMana() <= 0)
        {
            resourceManager.OutOfMana();
            return;
        }

        // If we don't have enough Left move cards
        if (resourceManager.GetLeftUses() <= 0)
        {
            resourceManager.NoMoreCardUses("Left");
            return;
        }

        // Only spend mana if the move actually succeeds
        if (selectedTile.TryMove(-1, 0))
        {
            resourceManager.UseMove("Left");
        }
    }

    /// <summary>
    /// Used to move the currently selected tile to the up/forwards.
    /// </summary>
    public void MoveSelectedForward()
    {
        // Ensure that a tile is selected, and that we're not moving
        // the start tile, the end tile, or the Player-occupied tile.
        if (selectedTile == null) return;
        if (PlayerOnSelectedTile()) return;
        if (SelectedTileIsStartOrEnd()) return;
        
        // If we're out of Mana
        if (resourceManager.GetMana() <= 0)
        {
            resourceManager.OutOfMana();
            return;
        }

        // If we don't have enough Forward/Up move cards
        if (resourceManager.GetForwardUses() <= 0)
        {
            resourceManager.NoMoreCardUses("Up");
            return;
        }

        // Only spend mana if the move actually succeeds
        if (selectedTile.TryMove(0, 1))
        {
            resourceManager.UseMove("Forward");
        }
    }

    /// <summary>
    /// Used to move the currently selected tile to the down/backwards.
    /// </summary>
    public void MoveSelectedBack()
    {
        // Ensure that a tile is selected, and that we're not moving
        // the start tile, the end tile, or the Player-occupied tile.
        if (selectedTile == null) return;
        if (PlayerOnSelectedTile()) return;
        if (SelectedTileIsStartOrEnd()) return;
        
        // If we're out of Mana
        if (resourceManager.GetMana() <= 0)
        {
            resourceManager.OutOfMana();
            return;
        }

        // If we don't have enough Back/Down move cards
        if (resourceManager.GetBackUses() <= 0)
        {
            resourceManager.NoMoreCardUses("Down");
            return;
        }

        // Only spend mana if the move actually succeeds
        if (selectedTile.TryMove(0, -1))
        {
            resourceManager.UseMove("Back");
        }
    }
}