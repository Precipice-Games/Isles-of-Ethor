using System;
using System.Collections.Generic;
using UnityEngine;

// This script is attached to a cheat sign (optional) on an island and used to mark all
// puzzles as complete. It is helpful for both playtesting and if there are any puzzles
// that are broken and the Player cannot traverse to the next islands. It should be
// removed in the final build.

public class PuzzleCheatSign : MonoBehaviour, IInteractable
{
    private List<GameObject> puzzles;
    public GameObject collectableCrystal;
    public IslandPuzzleManager islandPuzzleManager;
    
    public static event Action allPuzzlesComplete;

    private void OnEnable()
    {
        PlayerControlsInputs.devShortcutTriggered  += Interaction;
    }

    private void OnDisable()
    {
        PlayerControlsInputs.devShortcutTriggered  -= Interaction;
    }
    

    private void Awake()
    {
        // Get the puzzle prefabs
        puzzles = islandPuzzleManager.puzzlePrefabs;
    }
    
    public void Interaction()
    {
        MarkAllPuzzlesComplete();
        MarkCrystalCollected();
    }
    
    /// <summary>
    /// Used to automatically mark all the puzzles as completed.
    /// </summary>
    private void MarkAllPuzzlesComplete()
    {
        Debug.Log($"PuzzleCheatSign.cs >> Message received. Marking {puzzles.Count} puzzles as complete.");
        
        foreach (GameObject puzzle in puzzles)
        {
            PuzzleInformation puzzleInfo = puzzle.GetComponent<PuzzleInformation>();
            puzzleInfo.puzzleSolved = true;
        }
        
        allPuzzlesComplete?.Invoke();
    }
    
    /// <summary>
    /// Used to automatically collect the crystal.
    /// </summary>
    private void MarkCrystalCollected()
    {
        Debug.Log("PuzzleCheatSign.cs >> Message received. Marking the crystal as collected.");
        
        collectableCrystal.GetComponent<IInteractable>().Interaction();
    }
}
