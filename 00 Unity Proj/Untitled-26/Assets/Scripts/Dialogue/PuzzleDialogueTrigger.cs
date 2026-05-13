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
        StartCoroutine(WaitForPuzzleTransition(enterNode));
    }

    public void OnPuzzleExit()
    {
        runner.StartDialogue(exitNode);
    }

    private IEnumerator WaitForPuzzleTransition(string node)
    {
        bool ready = false;

        if (GameStateManager.CurrentGameState == GameStateManager.GameState.Puzzle)
        {
            ready = true;
        }

        System.Action<GameStateManager.GameState> callback = (newState) =>
        {
            if (newState == GameStateManager.GameState.Puzzle)
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
