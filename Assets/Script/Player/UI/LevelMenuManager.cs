using UnityEngine;
using UnityEngine.UI;


public class LevelMenuManager : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Button[] levelButtons;
    SaveLoadSystem saveLoadSystem;

    private void Awake()
    {
        saveLoadSystem = GetComponent<SaveLoadSystem>();
        int unlockedLevel = saveLoadSystem.GetLevelProgress();
        Debug.Log("Unlocked level index: " + unlockedLevel);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = false;
        }

        for (int i = 0;i < unlockedLevel + 1; i++)
        {
            levelButtons[i].interactable = true;

        }
    }
}
