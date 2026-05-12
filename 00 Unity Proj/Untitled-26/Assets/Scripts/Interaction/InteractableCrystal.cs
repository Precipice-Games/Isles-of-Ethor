using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class InteractableCrystal : MonoBehaviour, IInteractable
{
    [Title("Interactable Crystal Variables", "Variables related to the interactable crystal.")]
    [PropertyTooltip("Please attach the YarnSpinner DialogueRunner component here.")]
    public DialogueRunner runner;
    [PropertyTooltip("Please attach the Player game object here.")]
    public GameObject player;
    [PropertyTooltip("Where the Player should be teleported to after collecting the crystal.")]
    public UnityEngine.Vector3 destination;
    
    [Title("CrystalCollected", "This event is fired when the crystal has been collected.")]
    public UnityEvent crystalCollected;
    private bool finalPuzzleCompleted;

    public bool revealed = false;
    public bool bobUp = false;

    public float maxHeight = 4.8f;
    public float minHeight = 3.2f;

    private void FixedUpdate()
    {

        // Debug.Log("Height: " + transform.position.y);

        if (transform.position.y >= maxHeight)
        {

            bobUp = false;

        }
        else if (transform.position.y <= minHeight)
        { 
        
            bobUp = true;
        
        }

        if (finalPuzzleCompleted)
        {

            if (revealed)
            {
                Debug.Log("Revealed");
                if (bobUp)
                {

                    if (transform.position.y <= maxHeight)
                    {

                        transform.position = new UnityEngine.Vector3(transform.position.x, transform.position.y + (0.5f * Time.deltaTime), transform.position.z);

                    }

                }
                if(!bobUp)
                {

                    if (transform.position.y >= minHeight)
                    {

                        transform.position = new UnityEngine.Vector3(transform.position.x, transform.position.y - (0.5f * Time.deltaTime), transform.position.z);

                    }

                }


            }
            else
            {
                // When the final puzzle is completed, ensure it rises up from the ground
                if (transform.position.y < minHeight)
                {
                    transform.position = new UnityEngine.Vector3(transform.position.x, transform.position.y + Time.deltaTime, transform.position.z);
                }
                else
                {
                    revealed = true;
                }
                /*else if (transform.position.y >= 3.2)
                {
                    revealed = true;
                    bobUp = true;
                }*/
            }
        }

        
        
    }

    /// <summary>
    /// Runs when interacting with a collectable crystal. This triggers
    /// crystalCollected and destroys the game object.
    /// </summary>
    public void Interaction()
    {
        crystalCollected.Invoke();
        Destroy(gameObject);
        Debug.Log("Player position BEFORE collecting crystal: " + player.transform.position);
        Player.Instance.TeleportPlayer(destination);
        Debug.Log("Player position AFTER collecting crystal: " + player.transform.position);
        runner.StartDialogue(gameObject.name);
    }
    
    /// <summary>
    /// This method is subscribed to the islandPuzzlesCompleted UnityEvent from
    /// IslandPuzzleManager. Once all puzzles are completed, this method will run
    /// and set finalPuzzleCompleted to true, which will cause the crystal to
    /// rise up from the ground in the FixedUpdate() method.
    /// </summary>
    public void FinalPuzzleCompleted()
    {
        finalPuzzleCompleted = true;
        Debug.Log($"InteractableCrystal.cs >> finalPuzzleCompleted = {finalPuzzleCompleted}. Now rising crystal from ground...");
    }
}
