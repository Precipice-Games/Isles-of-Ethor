using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player != null && GameStateManager.CurrentGameState != GameStateManager.GameState.Puzzle)
            transform.LookAt(player.transform.position, Vector3.up);
            transform.Rotate(0, 180, 0);
    }
}
