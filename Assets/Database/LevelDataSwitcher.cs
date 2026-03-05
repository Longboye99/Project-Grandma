using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelDataSwitcher : MonoBehaviour
{
    [SerializeField]DataSwitchContainer _switchContainer;
    [SerializeField] int _dataIndex;

    public void SwitchContainer()
    {
        List<LocalSpreadsheetContainer> levelsData = _switchContainer.levelsData;
        _switchContainer.currentData = levelsData[_dataIndex];
    }

    public void SwitchScene()
    {
        SwitchContainer();
        if (_switchContainer.currentData.skipCutscene || _switchContainer.currentData.cutsceneLevel == null)
        {
            LoadGameLevel();
        }
        else
        {
            SceneManager.LoadScene(_switchContainer.currentData.cutsceneLevel);
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
