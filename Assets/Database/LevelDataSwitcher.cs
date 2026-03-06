using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelDataSwitcher : MonoBehaviour
{
    [SerializeField]DataSwitchContainer _switchContainer;
    [SerializeField] int _dataIndex;

    public void SwitchContainer()
    {
        PlayerPrefs.SetInt("currentLevel", _dataIndex);
    }

    public void SwitchScene()
    {
        SwitchContainer();
        if (_switchContainer.levelsData[_dataIndex].skipCutscene || _switchContainer.levelsData[_dataIndex].cutsceneLevel == null)
        {
            LoadGameLevel();
        }
        else
        {
            SceneManager.LoadScene(_switchContainer.levelsData[_dataIndex].cutsceneLevel);
            Time.timeScale = 1.0f;
        }
    }

    public void LoadGameLevel()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1.0f;
    }

    public void LoadNextLevel()
    {
        _dataIndex = GameManager.instance.anomalyManager.dictionary.currentLevel + 1;
        SwitchScene();
    }
}
