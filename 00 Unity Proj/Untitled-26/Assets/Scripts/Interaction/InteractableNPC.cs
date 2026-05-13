using System;
using Unity.Cinemachine;
using UnityEngine;
using Yarn;
using Yarn.Unity;

#if UNITY_EDITOR
using Yarn.Unity.Editor;
#endif

public class InteractableNPC : MonoBehaviour, IInteractable
{

    //   ==== Interaction Variables ====
    bool interactedWith = false;
    DialogueRunner runner;
    public GameObject exclamationPoint;
    public GameObject crystal;

    [SerializeField]
    GameObject player;
    [SerializeField]
    CinemachineCamera playerCam;

    [SerializeField]
    bool destroyOnPickup;

    private void Awake()
    {
        if (playerCam == null)
        {
            playerCam = GetComponent<CinemachineCamera>();
        }
    }

    // TODO: Does this need to be inside of a FixedUpdate()? Perhaps we can
    //       optimize this? FixedUpdate() is really for physics calculations,
    //       and this is just for starting a dialogue. Maybe we can move this
    //       to a more optimized method, like Callbacks, UnitEvents, etc.?
    private void FixedUpdate()
    {
        //Vector3 targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(playerCam.transform.position, Vector3.up);
        transform.Rotate(0, 180, 0);
        
        
        if (interactedWith)
        {
            Debug.Log("Dialogue Starting");
            
            
            runner = FindFirstObjectByType<Yarn.Unity.DialogueRunner>();
            runner.StartDialogue(gameObject.name);
            if (destroyOnPickup){Destroy(gameObject);}
            interactedWith = false;

        }
    }

    public void Interaction()
    {
        interactedWith = true;
    }

    /// <summary>
    /// Toggles visibility of exclamation point above NPC's head.
    /// The exclamation point should be visible for [TalkToMe] NPCS.
    /// </summary>
    [YarnCommand("toggleExclamation")]
    public void ToggleExclamation()
    {
        if (exclamationPoint != null)
        {
            exclamationPoint.SetActive(!exclamationPoint.activeSelf);
        }
    }

    [YarnCommand("toggleCrystal")]
    public void ToggleCrystal()
    {
        if (crystal != null)
        {
            crystal.SetActive(!crystal.activeSelf);
        }
    }
}
