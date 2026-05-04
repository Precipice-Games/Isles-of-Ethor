using System.Runtime.CompilerServices;
using UnityEngine;

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
    private GameObject player;

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
        transform.LookAt(player.transform.position);
    }
}
