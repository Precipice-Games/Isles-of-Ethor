using System;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class IceTrees : MonoBehaviour
{
    [SerializeField]
    private Material defaultTree;

    [SerializeField]
    private Material tallTree;

    [SerializeField]
    private Material smallTree;

    private int treeNum = 0;

    [SerializeField]
    private CinemachineCamera playerCam;

    private void Awake()
    {
        if (playerCam == null)
        {
            playerCam = GetComponent<CinemachineCamera>();
        }
    }

    void Start()
    {
        
        treeNum = Random.Range(0, 3);

        if (treeNum == 0 && transform.GetComponentInParent<IceTreeManager>().currentDefaultTrees < transform.GetComponentInParent<IceTreeManager>().maxDefaultTrees)
        {
            GetComponent<MeshRenderer>().material = defaultTree;
            transform.GetComponentInParent<IceTreeManager>().currentDefaultTrees += 1;
        }
        else if (treeNum == 1 && transform.GetComponentInParent<IceTreeManager>().currentTallTrees < transform.GetComponentInParent<IceTreeManager>().maxTallTrees)
        {
            GetComponent<MeshRenderer>().material = tallTree;
            transform.GetComponentInParent<IceTreeManager>().currentTallTrees += 1;
        }
        else if (treeNum == 2 && transform.GetComponentInParent<IceTreeManager>().currentSmallTrees < transform.GetComponentInParent<IceTreeManager>().maxSmallTrees)
        {
            GetComponent<MeshRenderer>().material = smallTree;
            transform.GetComponentInParent<IceTreeManager>().currentSmallTrees += 1;
        }
        else
        {
            GetComponent<MeshRenderer>().material = defaultTree;
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameStateManager.CurrentGameState != GameStateManager.GameState.Puzzle)
        {
            transform.LookAt(playerCam.transform.position, Vector3.up); 
            transform.Rotate(0, 180, 0);
        }
    }
}
