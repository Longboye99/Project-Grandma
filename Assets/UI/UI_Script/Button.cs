using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    [SerializeField] int sceneIndex;

    public void LoadNewScene()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
