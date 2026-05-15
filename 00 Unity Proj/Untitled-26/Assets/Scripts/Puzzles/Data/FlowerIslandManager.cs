using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class FlowerIslandManager : MonoBehaviour
{

    // [PropertyTooltip("Attach the relevant data for this island's puzzles.")]
    // public IslandPuzzleManager islandFlowerManager;
    [PropertyTooltip("Attach the DialogueSystem object here, which contains the InMemoryVariableStorage.")]
    public InMemoryVariableStorage variableStorage;
    
    [Space]
    [Title("IslandCompleted", "This event is fired when all puzzles are complete and the crystal has been collected.")]
    public UnityEvent islandCompleted;
    
    // Variables to track and update the island's completion status
    [SerializeField]private bool allFlowersCollected;
    [SerializeField]private bool crystalCollected;

    private int flowersCollected;

    private void OnEnable()
    {
        CollectableFlower.flowerCollected += CheckIslandCompleted;
    }

    private void OnDisable()
    {
        CollectableFlower.flowerCollected -= CheckIslandCompleted;
    }

    /// <summary>
    /// Used to verify that all the current island's puzzles have been completed. Also
    /// updates the variable for the specified island in the YarnSpinner variable storage.
    /// </summary>
    public void IslandPuzzlesCompleted()
    {
        // case IslandName.FlowerIsland:
        //     Debug.Log("IslandManager.cs >> Flower Island puzzles completed!");
        //     variableStorage.SetValue("$flowerFinished", true);
        //     allPuzzlesCompleted = true;
        //     break;
        
        CheckIslandCompleted();
    }

    /// <summary>
    /// Used to verify that the island's end crystal has been collected. This is needed
    /// for the island itself to be completed in its entirety, and for the Player to be
    /// able to traverse to other islands using the Airship.
    /// </summary>
    public void CrystalCollected()
    {
        Debug.Log("FlowerIslandManager.cs >> Crystal collected!");
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
        if (variableStorage.TryGetValue<int>("$flowers", out var flowersCollected)){
            Debug.Log("flowers collected= " + flowersCollected);
            if (flowersCollected >= 6){
                allFlowersCollected = true;
            }
        }
        if (allFlowersCollected && crystalCollected)
        {
            Debug.Log("FlowerIslandManager.cs >> Island completed!");
            variableStorage.SetValue("$flowerFinished", true);
            islandCompleted.Invoke();
        }
    }
}