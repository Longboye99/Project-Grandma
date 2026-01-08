using UnityEngine;
using UnityEngine.SceneManagement;

public class CutScene : MonoBehaviour
{
    [SerializeField] string sceneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadScene(sceneName);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
