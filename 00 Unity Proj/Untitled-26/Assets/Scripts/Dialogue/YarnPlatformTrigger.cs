using UnityEngine;
using Yarn.Unity;

public class YarnPlatformTrigger : MonoBehaviour
{
    public MovingPlatform[] platforms;

    [YarnCommand("activatePlatforms")]
    public void ActivatePlatforms()
    {
        // Activate all platforms
        foreach (var platform in platforms)
        {
            platform.ActivatePlatform();
        }
    }
}