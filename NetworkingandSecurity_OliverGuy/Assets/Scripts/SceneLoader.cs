using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName; //The script will load the given scene by name

    public void LoadScene() //Via Inspector (Button)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitApplication() //Via Inspector (Button)
    {
        Application.Quit();
    }
}
