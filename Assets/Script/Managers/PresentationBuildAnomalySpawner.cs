using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PresentationBuildAnomalySpawner : MonoBehaviour
{
    [SerializeField] DataSwitchContainer dataSwitcher;
    [SerializeField] LocalSpreadsheetContainer DataContainer;

    [SerializeField] TestEnemy2 enemy;
    [SerializeField] List<Anomaly> anomalyList;
    [SerializeField] List<int> spawnTime;
    [SerializeField] float currentTime;
    [SerializeField] int spawnIndex = 0;


    private void Start()
    {
        bool presentationMode = false;
        int _currentLevel = PlayerPrefs.GetInt("currentLevel");
        DataContainer = dataSwitcher.levelsData[_currentLevel];
        presentationMode = DataContainer.presentationMode;

        if (presentationMode)
        {
            enemy.enable = false;
        }
        else
        {
            this.gameObject.SetActive(false);
        }
        
    }
    private void Update()
    {
        currentTime = GameManager.instance.levelManager.currentTime;
        if(spawnIndex < anomalyList.Count)
        {
            CheckTime();
        }
    }

    private void CheckTime()
    {
        if(currentTime >= spawnTime[spawnIndex])
        {
            GameManager.instance.anomalyManager.TriggerAnomaly(anomalyList[spawnIndex]);
            Debug.Log("Presentation spawned: " + anomalyList[spawnIndex].name);

            spawnIndex++;
        }
    }
}
