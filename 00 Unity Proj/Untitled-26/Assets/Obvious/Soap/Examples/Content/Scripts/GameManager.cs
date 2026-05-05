using UnityEngine;
using UnityEngine.SceneManagement;

namespace Obvious.Soap.Example
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private KeyCode _reloadSceneKey = KeyCode.R;
        [SerializeField] private KeyCode _deletePlayerPrefs = KeyCode.P;
        
        void Update()
        {
            if (Input.GetKeyDown(_reloadSceneKey))
                ReloadScene();
            
            if (Input.GetKeyDown(_deletePlayerPrefs))
                PlayerPrefs.DeleteAll();

            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }
        
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}