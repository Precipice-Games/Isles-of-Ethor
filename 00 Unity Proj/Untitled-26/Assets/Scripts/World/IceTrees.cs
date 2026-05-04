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

        if (treeNum == 0)
        {
            GetComponent<MeshRenderer>().material = defaultTree;
        }
        else if (treeNum == 1)
        {
            GetComponent<MeshRenderer>().material = tallTree;
        }
        else if (treeNum == 2)
        {
            GetComponent<MeshRenderer>().material = smallTree;
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(player.transform.position);
    }
}
