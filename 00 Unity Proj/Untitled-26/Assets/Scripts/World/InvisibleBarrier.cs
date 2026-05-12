using UnityEngine;

public class InvisibleBarrier : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    
    private void Awake()
    {
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }
}
