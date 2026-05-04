using UnityEngine;

public class Deathbox : MonoBehaviour
{
    public Transform[] respawnLocations;

    [SerializeField] private int puzzlesComplete = 0;

    // Set respawn location to next respawn point, based on number of puzzles completed.
    public void OnPuzzleCompletion()
    {
        puzzlesComplete++;
    }

    // Reset respawn location to the beginning of the island when the crystal is collected,
    // as the player will be teleported back to the beginning of the island after collecting the crystal.
    public void OnCrystalCollect()
    {
        puzzlesComplete = 0;
    }

    public void OnDoorEntered(Vector3 respawnLocation)
    {
        puzzlesComplete = 1;
    }

    // Teleport player to respawn location when they enter the deathbox trigger.
    // The respawn location is determined by the number of puzzles completed, which is updated by the OnPuzzleCompletion().
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {

            Player.Instance.TeleportPlayer(respawnLocations[puzzlesComplete].position);

        }

    }

}
