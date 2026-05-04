using UnityEngine;

public class IceTreeManager : MonoBehaviour
{

    public float maxDefaultTrees;
    public float currentDefaultTrees;
    public int maxSmallTrees;
    public int currentSmallTrees;
    public int maxTallTrees;
    public int currentTallTrees;

    private void Start()
    {

        maxDefaultTrees = transform.childCount / 3;
        Mathf.RoundToInt(maxDefaultTrees + 0.5f);

        maxSmallTrees = transform.childCount / 3;
        maxTallTrees = transform.childCount / 3;

    }

    // Update is called once per frame
    void Update()
    {
        


    }
}
