using UnityEngine;

public class PuzzleBlocker : MonoBehaviour
{

    public GameStateManager StateManager;
    public bool puzzleMode = false;

    private void OnEnable()
    {

        GameStateManager.transitionedToNewState += CheckPuzzleState;

    }

    private void OnDisable()
    {

        GameStateManager.transitionedToNewState -= CheckPuzzleState;

    }

    // Update is called once per frame
    void Update()
    {
        if (StateManager != null && puzzleMode)
        {
            GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            GetComponent<BoxCollider>().enabled = true;
        }

    }

    void CheckPuzzleState(GameStateManager.GameState state)
    {

        if (state == GameStateManager.GameState.Puzzle)
        {
            puzzleMode = true;
        }

    }

}
