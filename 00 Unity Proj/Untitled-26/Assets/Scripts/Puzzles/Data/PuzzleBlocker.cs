using UnityEngine;

public class PuzzleBlocker : MonoBehaviour
{

    public GameStateManager StateManager;

    private void Start()
    {

        

    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(StateManager.gameState == GameStateManager.GameState.Puzzle);

        if (StateManager != null && StateManager.gameState == GameStateManager.GameState.Puzzle)
        //if (GameStateManager.Instance != null && GameStateManager.Instance.gameState == GameStateManager.GameState.Puzzle)
        {
            GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            GetComponent<BoxCollider>().enabled = true;
        }

    }
}
