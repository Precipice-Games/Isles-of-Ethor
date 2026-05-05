using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

// This script is used to grab information about the current puzzle.
// This includes the starting tile, ending tile, etc. It is unique to
// each puzzle and will be used by PlayerFixedMovement.cs to handle
// movement across each puzzle.

public class PuzzleInformation : MonoBehaviour
{
    [Title("Puzzle Information", "Attach the relevant data of this puzzle.")]
    public CinemachineCamera camera;
    public GameObject canvas;
    public GameObject startTile;
    public GameObject endTile;
    public GameObject gridManager;
    public GameObject resourceManager;
    public bool puzzleSolved; // Has the puzzle been solved?
}