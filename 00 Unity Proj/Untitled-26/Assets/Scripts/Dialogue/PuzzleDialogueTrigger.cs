using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Yarn.Unity;

public class PuzzleDialogueTrigger : MonoBehaviour
{
    public DialogueRunner runner;
    [PropertyTooltip("Attach the DialogueSystem object here, which contains the InMemoryVariableStorage.")]
    public InMemoryVariableStorage variableStorage;

    public string enterNode;
    public string exitNode;

    [Tooltip("Time delay before dialogue starts after exiting the puzzle.")]
    public float exitDelay = .5f;

    public void OnPuzzleEnter()
    {
        StartCoroutine(WaitForPuzzleTransition(enterNode, GameStateManager.GameState.Puzzle));
    }

    public void OnPuzzleExit()
    {
        StartCoroutine(WaitForPuzzleTransition(exitNode, GameStateManager.GameState.Exploration));
    }

    private IEnumerator WaitForPuzzleTransition(string node, GameStateManager.GameState targetState)
    {
        bool ready = false;

        if (GameStateManager.CurrentGameState == targetState)
        {
            ready = true;
        }

        System.Action<GameStateManager.GameState> callback = (newState) =>
        {
            if (newState == targetState)
            {
                ready = true;
            }
        };

        GameStateManager.transitionedToNewState += callback;

        while (!ready) yield return null;

        GameStateManager.transitionedToNewState -= callback;

        if(targetState == GameStateManager.GameState.Exploration && exitDelay > 0f)
        {
            yield return new WaitForSeconds(exitDelay);
        }

        runner.StartDialogue(node);
    }
}
