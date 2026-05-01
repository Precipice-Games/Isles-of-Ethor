using UnityCommunity.UnitySingleton;
using UnityEngine.SceneManagement;

// This is the GameManager. It is a singleton that is primarily used for handling scene changes.

public class GameManager : MonoSingleton<GameManager>
{
    public enum SceneDestination
    {
        MainMenu,
        MotherIsland,
        IceIsland,
        OasisIsland,
        FlowerIsland
    }
    
    // Placeholder string
    private string scene;

    /// <summary>
    /// Called by SceneChanger.cs to prep the next scene to load. Once the
    /// loading screen sequence is complete, the LoadScene() method will
    /// automatically use this variable.
    /// </summary>
    /// <param name="destination"></param>
    public void IncomingScene(SceneDestination destination)
    {
        scene = DetermineScene(destination);
    }
    
    /// <summary>
    /// Called by IncomingScene(). Returns a string that is the name of
    /// the next scene to load, based on the value of nextDestination.
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    private string DetermineScene(SceneDestination destination)
    {
        switch (destination)
        {
            case SceneDestination.MainMenu:
                return "MainMenu";
            case SceneDestination.MotherIsland:
                return "Mother_Island";
            case SceneDestination.IceIsland:
                return "Ice_Island";
            case SceneDestination.OasisIsland:
                return "Oasis_Island";
            case SceneDestination.FlowerIsland:
                return "Flower_Island";
            default:
                return "MainMenu";
        }
    }
    
    /// <summary>
    /// This method is what's actually called to load the next scene. It is
    /// triggered by a UnityEvent from the ViewManager once the loading screen
    /// sequence is complete.
    /// </summary>
    public void LoadScene()
    {
        SceneManager.LoadScene(scene);
    }
}
