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

        Debug.Log(StateManager.gameState == GameStateManager.GameState.Puzzle);

        if (StateManager != null && puzzleMode)
        //if (GameStateManager.Instance != null && GameStateManager.Instance.gameState == GameStateManager.GameState.Puzzle)
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
