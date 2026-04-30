using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This script stores data regarding the resources provided during each puzzle.
/// Specifically, it keeps track of Mana points and available movement cards for
/// the Player to use. Note that it maintains a tight relationship with
/// TileSelector.cs to check resource availability for performing a move.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static event Action noMoreCardUses;

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
    [Title("UI Data", "Attach the UI elements for this puzzle's resources.")]
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
        if (currentMana <= 0)
        {
            Debug.Log("ResourceManager.cs >> No mana to move.");
            PuzzleFeedback.Instance?.ShowMessage("No more mana!");
            return false;
        }

        switch (moveType)
        {
            case "Left":
                if (currentLeftUses <= 0)
                {
                    if (printCardDrainage)
                        Debug.Log("ResourceManager.cs >> No Left card uses remaining");

                    PuzzleFeedback.Instance?.ShowMessage("No more Left card usages!");
                    noMoreCardUses?.Invoke();
                    return false;
                }

                currentLeftUses--;
                UpdateLeftText(currentLeftUses);
                break;

            case "Right":
                if (currentRightUses <= 0)
                {
                    if (printCardDrainage)
                        Debug.Log("ResourceManager.cs >> No Right card uses remaining");

                    PuzzleFeedback.Instance?.ShowMessage("No more Right card usages!");
                    noMoreCardUses?.Invoke();
                    return false;
                }

                currentRightUses--;
                UpdateRightText(currentRightUses);
                break;

            case "Forward":
                if (currentForwardUses <= 0)
                {
                    if (printCardDrainage)
                        Debug.Log("ResourceManager.cs >> No Forward card uses remaining");

                    PuzzleFeedback.Instance?.ShowMessage("No more Forward card usages!");
                    noMoreCardUses?.Invoke();
                    return false;
                }

                currentForwardUses--;
                UpdateUpText(currentForwardUses);
                break;

            case "Back":
                if (currentBackUses <= 0)
                {
                    if (printCardDrainage)
                        Debug.Log("ResourceManager.cs >> No Back card uses remaining");

                    PuzzleFeedback.Instance?.ShowMessage("No more Back card usages!");
                    noMoreCardUses?.Invoke();
                    return false;
                }

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