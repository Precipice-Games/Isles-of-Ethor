using System.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
    [Title("Post Processing Variables", "Variables related to the post processor.")]
    [PropertyTooltip("The volume component of the post processor.")]
    public Volume volume;
    public DepthOfField dofComponent;
    public Bloom bloomComponent;
    
    // Subscribe to events
    private void OnEnable()
    {
        ViewManager.toggleDepthOfField += ToggleDepthOfField;
        ViewManager.toggleBloom += ToggleBloom;
    }
    
    // Unsubscribe from events
    private void OnDisable()
    {
        ViewManager.toggleDepthOfField -= ToggleDepthOfField;
        ViewManager.toggleBloom -= ToggleBloom;
    }

    private void Awake()
    {
        // Get the depth of field override
        if (volume.profile.TryGet<DepthOfField>(out var dof))
        {
            dofComponent = dof;
        }
        
        // Get the bloom override
        if (volume.profile.TryGet<Bloom>(out var blm))
        {
            bloomComponent = blm;
        }
    }

    /// <summary>
    /// Toggles the Volume component of the PostProcessor to enable or
    /// disable all post-processing effects.
    /// </summary>
    private void TogglePostProcessor(bool active)
    {
        volume.enabled = active;
        Debug.Log($"PostProcessingManager.cs >> Toggled the volume on the post processor as {active}.");
    }

    /// <summary>
    /// Toggles the depth of field component on the post-processor.
    /// </summary>
    /// <param name="active"></param>
    private void ToggleDepthOfField(bool active)
    {
        dofComponent.active = active;
        Debug.Log($"PostProcessingManager.cs >> Toggled the depth of field on the post processor as {active}.");
    }
    
    /// <summary>
    /// Toggles the depth of field component on the post-processor.
    /// </summary>
    /// <param name="active"></param>
    private void ToggleBloom(bool active)
    {
        bloomComponent.active = active;
        Debug.Log($"PostProcessingManager.cs >> Toggled the bloom on the post processor as {active}.");
    }
}
