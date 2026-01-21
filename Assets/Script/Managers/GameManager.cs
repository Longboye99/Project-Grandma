using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance{ get; private set; }

    public LevelManager levelManager;
    public AnomalyManager anomalyManager;
    public PlayerManager playerManager;
    public UiManager uiManager;
    public SfxManager sfxManager;
    public JumpscareManager jumpscareManager;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Game Manager in the scene.");
        }
        instance = this;
    }
}
