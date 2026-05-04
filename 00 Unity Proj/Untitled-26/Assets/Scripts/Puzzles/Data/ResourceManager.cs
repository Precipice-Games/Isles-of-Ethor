using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// This script stores data regarding the resources provided during each puzzle.
/// Specifically, it keeps track of Mana points and available movement cards for
/// the Player to use. Note that it maintains a tight relationship with
/// TileSelector.cs to check resource availability for performing a move.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    // NOTE: The only reason lackOfResources takes in a SelectableTile is so that we only need
    // one method in PuzzleFeedback.cs to handle all different message types. Likewise, any
    // usages of this event in this class should just pass in a null value as a placeholder.
    
    [Title("Lacking Resources", "This event is fired when the Player has no Mana left or is lacking uses of a specific movement card.")]
    public UnityEvent<string, SelectableTile> lackOfResources;
    
    [Title("Resource Data", "Set the appropriate Mana and Move Card count for this puzzle.")]
    [PropertyTooltip("The starting Mana count. Set to 5 by default, but should be updated per puzzle.")]
    public int startingMana = 5;

    [PropertyTooltip("The starting Left Card move count. Set to 3 by default, but should be updated per puzzle.")]
    public int moveLeftUses = 3;

    [PropertyTooltip("The starting Right Card move count. Set to 3 by default, but should be updated per puzzle.")]
    public int moveRightUses = 3;

    [PropertyTooltip("The starting Up/Forward Card move count. Set to 3 by default, but should be updated per puzzle.")]
    public int moveForwardUses = 3;

    [PropertyTooltip("The starting Down/Back Card move count. Set to 3 by default, but should be updated per puzzle.")]
    public int moveBackUses = 3;

    private int currentMana;
    private int currentLeftUses;
    private int currentRightUses;
    private int currentForwardUses;
    private int currentBackUses;

    [Space]
    [Title("UI Data", "Attach the UI elements for this puzzle.")]
    [PropertyTooltip("Game object that holds the PuzzleFeedback script.")]
    public PuzzleFeedback puzzleFeedback;
    public TMP_Text manaLabel;
    public TMP_Text leftLabel;
    public TMP_Text rightLabel;
    public TMP_Text upLabel;
    public TMP_Text downLabel;

    [Space]
    [Title("Debugging Options", "Settings for quick debugging options.")]
    [PropertyTooltip("Prints out the starting value for each resource. True by default.")]
    public bool printStartingValues = true;

    [PropertyTooltip("Print out the value of the Mana resource after being used. True by default.")]
    public bool printManaDeductions = true;

    [PropertyTooltip("Print out the value of a movement card after being used. True by default.")]
    public bool printCardDeductions = true;

    [PropertyTooltip("Print out when a specific movement card has no uses left. True by default.")]
    public bool printCardDrainage = true;

    // Subscribe to events
    private void OnEnable()
    {
        ResetPuzzle.resetPuzzle += ResetResources;
    }

    // Unsubscribe from events
    private void OnDisable()
    {
        ResetPuzzle.resetPuzzle -= ResetResources;
    }

    void Start()
    {
        if (puzzleFeedback == null)
        {
            Debug.LogError("ResourceManager.cs >> PuzzleFeedback reference is missing!");
        }
        
        currentMana = startingMana;
        currentLeftUses = moveLeftUses;
        currentRightUses = moveRightUses;
        currentForwardUses = moveForwardUses;
        currentBackUses = moveBackUses;

        if (printStartingValues)
        {
            Debug.Log("ResourceManager.cs >> Starting Mana: " + currentMana);
            Debug.Log("ResourceManager.cs >> Starting Left Card Uses: " + moveLeftUses);
            Debug.Log("ResourceManager.cs >> Starting Right Card Uses: " + moveRightUses);
            Debug.Log("ResourceManager.cs >> Starting Up Card Uses: " + moveForwardUses);
            Debug.Log("ResourceManager.cs >> Starting Down Card Uses: " + moveBackUses);
        }

        UpdateManaText(currentMana);
        UpdateLeftText(moveLeftUses);
        UpdateRightText(moveRightUses);
        UpdateUpText(moveForwardUses);
        UpdateDownText(moveBackUses);
    }

    /// <summary>
    /// Called in TileSelector.cs when the Player tries to use a movement card.
    /// Checks if the Player has enough resources before deducting anything.
    /// If a move is valid, return true. Otherwise, return false.
    /// </summary>
    public bool UseMove(string moveType)
    {
        // if (currentMana <= 0)
        // {
        //     Debug.Log("ResourceManager.cs >> No mana to move.");
        //     string message = "No more mana!";
        //     // noCardUsagesLeft?.Invoke();
        //     // TODO: put Mana message here somehow with event.
        //     return false;
        // }

        switch (moveType)
        {
            case "Left":
                currentLeftUses--;
                UpdateLeftText(currentLeftUses);
                break;

            case "Right":
                currentRightUses--;
                UpdateRightText(currentRightUses);
                break;

            case "Forward":
                currentForwardUses--;
                UpdateUpText(currentForwardUses);
                break;

            case "Back":
                currentBackUses--;
                UpdateDownText(currentBackUses);
                break;
        }

        currentMana--;

        if (printManaDeductions)
            Debug.Log("ResourceManager.cs >> Mana remaining: " + currentMana);

        if (printCardDeductions)
            Debug.Log("ResourceManager.cs >> Card used: " + moveType);

        UpdateManaText(currentMana);

        return true;
    }

    /// <summary>
    /// Called by TileSelector.cs when the Player tries to
    /// use a movement card but has no Mana left.
    /// </summary>
    public void OutOfMana()
    {
        string message = $"You have no Mana left!";
        lackOfResources?.Invoke(message, null);
    }

    /// <summary>
    /// Called by TileSelector.cs when the Player tries to use a movement card
    /// that has no uses left. Builds a new string based on the direction type
    /// and fires it in the noCardUsagesLeft event, which is utilized by
    /// PuzzleFeedback.cs.
    /// </summary>
    /// <param name="direction"></param>
    public void NoMoreCardUses(string direction)
    {
        string message = $"No more {direction} cards left.";
        lackOfResources?.Invoke(message, null);
    }

    /// <summary>
    /// Adds mana (used for ManaWell tiles).
    /// </summary>
    public void AddMana(int amount)
    {
        currentMana += amount;

        Debug.Log("ResourceManager.cs >> Mana gained: +" + amount);
        Debug.Log("ResourceManager.cs >> Current Mana: " + currentMana);

        UpdateManaText(currentMana);
    }

    /// <summary>
    /// Returns the current Mana count.
    /// </summary>
    public int GetMana()
    {
        return currentMana;
    }

    public int GetLeftUses()
    {
        return currentLeftUses;
    }

    public int GetRightUses()
    {
        return currentRightUses;
    }

    public int GetForwardUses()
    {
        return currentForwardUses;
    }

    public int GetBackUses()
    {
        return currentBackUses;
    }

    /// <summary>
    /// Updates the Mana text on the UI.
    /// </summary>
    private void UpdateManaText(int amount)
    {
        manaLabel.text = $"Mana\n{amount}";
    }

    /// <summary>
    /// Updates the Left card uses text on the UI.
    /// </summary>
    private void UpdateLeftText(int amount)
    {
        leftLabel.text = amount.ToString();
    }

    /// <summary>
    /// Updates the Right card uses text on the UI.
    /// </summary>
    private void UpdateRightText(int amount)
    {
        rightLabel.text = amount.ToString();
    }

    /// <summary>
    /// Updates the Up card uses text on the UI.
    /// </summary>
    private void UpdateUpText(int amount)
    {
        upLabel.text = amount.ToString();
    }

    /// <summary>
    /// Updates the Down card uses text on the UI.
    /// </summary>
    private void UpdateDownText(int amount)
    {
        downLabel.text = amount.ToString();
    }

    /// <summary>
    /// Resets the Mana and movement card resources to their starting
    /// values. Called when the resetPuzzle event is triggered.
    /// </summary>
    private void ResetResources()
    {
        currentMana = startingMana;

        currentLeftUses = moveLeftUses;
        currentRightUses = moveRightUses;
        currentForwardUses = moveForwardUses;
        currentBackUses = moveBackUses;

        UpdateManaText(currentMana);
        UpdateLeftText(moveLeftUses);
        UpdateRightText(moveRightUses);
        UpdateUpText(moveForwardUses);
        UpdateDownText(moveBackUses);
    }
}