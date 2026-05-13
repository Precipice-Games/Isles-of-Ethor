using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

// This script is used to track information about each puzzle on
// a given island. It's attached to the IslandManager of that
// island's scene.

public class IslandPuzzleManager : MonoBehaviour
{
    [Title("Puzzle Prefabs", "Attach each of the Puzzle Prefabs for this island.")]
    public List<GameObject> puzzlePrefabs;
    
    [Space]
    [Title("IslandPuzzlesCompleted", "This event is fired when an island's puzzles are complete.")]
    public UnityEvent islandPuzzlesCompleted;

    // Subscribe to events
    private void OnEnable()
    {
        PlayerFixedMovement.updatePuzzleStatus += UpdateCompletionStatus;
        PuzzleCheatSign.allPuzzlesComplete += CheckAllPuzzlesCompleted;
    }
    
    // Unsubscribe from events
    private void OnDisable()
    {
        PlayerFixedMovement.updatePuzzleStatus -= UpdateCompletionStatus;
        PuzzleCheatSign.allPuzzlesComplete -= CheckAllPuzzlesCompleted;
    }
    
    public void UpdateCompletionStatus(PuzzleInformation completedPuzzle)
    {
        // Update the puzzle's completion status
        PuzzleDialogueTrigger dialogueTrigger = completedPuzzle.GetComponent<PuzzleDialogueTrigger>();
        if (dialogueTrigger != null)
        {
            Debug.Log("Checkpoint reached");
            dialogueTrigger.OnPuzzleExit();
        }
        completedPuzzle.puzzleSolved = true;
        CheckAllPuzzlesCompleted();
    }

    /// <summary>
    /// Used to check if all puzzles on the island have been completed.
    /// </summary>
    private void CheckAllPuzzlesCompleted()
    {
        foreach (GameObject puzzlePrefab in puzzlePrefabs)
        {
            PuzzleInformation puzzleInfo = puzzlePrefab.GetComponent<PuzzleInformation>();

            // Exit if there's any puzzles unsolved
            if (puzzleInfo.puzzleSolved == false)
            {
                return;
            }
            
            Debug.Log("IslandPuzzleManager.cs >> All puzzles completed!");
            
            // This event is assigned in the Unity Editor. It notifies
            // the IslandManager and the InteractableCrystal that all
            // puzzles have been completed.
            islandPuzzlesCompleted.Invoke();
        }
    }
}