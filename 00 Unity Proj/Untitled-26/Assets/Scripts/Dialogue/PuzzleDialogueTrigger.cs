using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class PuzzleDialogueTrigger : MonoBehaviour
{
    public DialogueRunner runner;

    public string enterNode;
    public string exitNode;

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
        runner.StartDialogue(node);
    }
}
