using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

// This script is used to track data about a given island. It is
// attached to the IslandManager GameObject in each island's scene.
// Right now, it contains the IslandPuzzleManager, which tracks the
// data of the puzzles on the island. The reason it is a different
// script is so that it can be turned into a functional prefab, and
// also handle any other island-related data in the scene.

public class IslandManager : MonoBehaviour
{
    public enum IslandName
    {
        MotherIsland,
        IceIsland,
        OasisIsland,
        FlowerIsland
    }

    [Title("Island Manager Data", "Data regarding the current island.")]
    [PropertyTooltip("Please assign the current island of this scene.")]
    public IslandName islandName = IslandName.IceIsland; // Default is Ice Island
    [PropertyTooltip("Attach the relevant data for this island's puzzles.")]
    public IslandPuzzleManager islandPuzzleManager;
    [PropertyTooltip("Attach the DialogueSystem object here, which contains the InMemoryVariableStorage.")]
    public InMemoryVariableStorage variableStorage;
    
    [Space]
    [Title("IslandCompleted", "This event is fired when all puzzles are complete and the crystal has been collected.")]
    public UnityEvent islandCompleted;
    
    // Variables to track and update the island's completion status
    [SerializeField]private bool allPuzzlesCompleted;
    [SerializeField]private bool crystalCollected;
    // public static event Action<PuzzleInformation> islandCompleted;

    /// <summary>
    /// Used to verify that all the current island's puzzles have been completed. Also
    /// updates the variable for the specified island in the YarnSpinner variable storage.
    /// </summary>
    public void IslandPuzzlesCompleted()
    {
        switch (islandName)
        {
            case IslandName.MotherIsland:
                Debug.Log("IslandManager.cs >> Mother Island puzzles completed!");
                variableStorage.SetValue("$motherFinished", true);
                allPuzzlesCompleted = true;
                break;
            case IslandName.IceIsland:
                Debug.Log("IslandManager.cs >> Ice Island puzzles completed!");
                variableStorage.SetValue("$iceFinished", true);
                allPuzzlesCompleted = true;
                break;
            case IslandName.OasisIsland:
                Debug.Log("IslandManager.cs >> Oasis Island puzzles completed!");
                // variableStorage.SetValue("$oasisFinished", true);
                allPuzzlesCompleted = true;
                break;
            case IslandName.FlowerIsland:
                Debug.Log("IslandManager.cs >> Flower Island puzzles completed!");
                variableStorage.SetValue("$flowerFinished", true);
                allPuzzlesCompleted = true;
                break;
            
            // TODO: Add more cases for each island and check with Matthew about
            //       the specific variable names in the YarnSpinner variable storage.
        }
        
        CheckIslandCompleted();
    }

    /// <summary>
    /// Used to verify that the island's end crystal has been collected. This is needed
    /// for the island itself to be completed in its entirety, and for the Player to be
    /// able to traverse to other islands using the Airship.
    /// </summary>
    public void CrystalCollected()
    {
        Debug.Log("IslandManager.cs >> Crystal collected!");
        crystalCollected = true;
        CheckIslandCompleted();
    }

    /// <summary>
    /// Checks if the island has been completed by verifying that all puzzles have been completed
    /// and the end crystal has been collected. If so, it invokes the islandCompleted event, which
    /// is picked up by the Airship script to allow the Player to traverse to other islands.
    /// </summary>
    private void CheckIslandCompleted()
    {
        if (allPuzzlesCompleted && crystalCollected)
        {
            Debug.Log("IslandManager.cs >> Island completed!");
            islandCompleted.Invoke();
        }
    }
}