using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelDataSwitcher : MonoBehaviour
{
    [SerializeField]DataSwitchContainer _container;
    [SerializeField] int _dataIndex;

    public void SwitchContainer()
    {
        List<LocalSpreadsheetContainer> levelsData = _container.levelsData;
        _container.currentData = levelsData[_dataIndex];
    }

    public void SwitchScene()
    {
        SwitchContainer();
        SceneManager.LoadScene(1);
        Time.timeScale = 1.0f;
    }
}
