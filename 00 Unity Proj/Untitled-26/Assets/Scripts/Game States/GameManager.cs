using UnityCommunity.UnitySingleton;
using UnityEngine;
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
    private string nextScene;
    private string currentScene;

    private AsyncOperation asyncLoadOp;
    private AsyncOperation asyncUnloadOp;

    /// <summary>
    /// Called by SceneChanger.cs to prep the next scene to load. Once the
    /// loading screen sequence is complete, the LoadScene() method will
    /// automatically use this variable.
    /// </summary>
    /// <param name="destination"></param>
    public void IncomingScene(SceneDestination destination)
    {
        // Unload the current scene
        currentScene = SceneManager.GetActiveScene().name;
        asyncUnloadOp = SceneManager.UnloadSceneAsync(currentScene);
        
        if (asyncUnloadOp == null)
        {
            Debug.LogError($"GameManager.cs >> Failed to unload scene '{currentScene}'. Scene may not exist or is not loaded.");
            return;
        }
        
        asyncUnloadOp.allowSceneActivation = false;
        Debug.Log($"GameManager.cs >> asyncUnloadOp.allowSceneActivation = {asyncUnloadOp.allowSceneActivation}");
        
        // Determine the next scene to load
        nextScene = DetermineScene(destination);
        asyncLoadOp = SceneManager.LoadSceneAsync(nextScene);
        
        if (asyncLoadOp == null)
        {
            Debug.LogError($"GameManager.cs >> Failed to load scene '{nextScene}'. Scene may not exist in build settings.");
            return;
        }
        
        asyncLoadOp.allowSceneActivation = false;
        Debug.Log($"GameManager.cs >> asyncLoadOp.allowSceneActivation = {asyncLoadOp.allowSceneActivation}");
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
        if (asyncLoadOp == null || asyncUnloadOp == null)
        {
            Debug.LogError("GameManager.cs >> Cannot load scene. Async operations are null. Call IncomingScene() first.");
            return;
        }
        
        asyncLoadOp.allowSceneActivation = true;
        asyncUnloadOp.allowSceneActivation = true;
    }
}
