using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

// This is SelectableTilesParent.cs. Because every single tile in a given puzzle has its own SelectableTile.cs
// script, it can be hard to manage events. This script was created as a way to manage all the SelectableTiles
// in a given puzzle.

public class SelectableTilesParent : MonoBehaviour
{
    [Title("Lacking Resources", "This event is fired when the a move is blocked due to grid-related variables.")]
    public UnityEvent<string, SelectableTile> gridMoveBlocked;
}