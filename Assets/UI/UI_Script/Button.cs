using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    [SerializeField] string sceneName;

    public void LoadNewScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void PlayFadeOut()
    {

    }    
}
