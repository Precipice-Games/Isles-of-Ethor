using UnityEngine;
using Yarn.Unity;

public class InvisWallTrigger : MonoBehaviour
{
    public DialogueRunner runner;
    public string nodeToRun;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            runner.StartDialogue(nodeToRun);
        }
    }

}
