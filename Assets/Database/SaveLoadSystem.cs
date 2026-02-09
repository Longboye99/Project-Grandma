using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    public void SaveLevelProgress(int value)
    {
        PlayerPrefs.SetInt("levelProgress", value);
    }

    public int GetLevelProgress()
    {
        return PlayerPrefs.GetInt("levelProgress");
    }
}
