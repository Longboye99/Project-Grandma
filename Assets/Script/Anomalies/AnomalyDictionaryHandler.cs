using Game.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnomalyDictionaryHandler : MonoBehaviour
{
    [SerializeField] DataSwitchContainer dataSwitcher;
    [SerializeField] LocalSpreadsheetContainer DataContainer;
    public int currentLevel;

    Anomaly[] AllAnomalies;
    public Dictionary<AreaEnum, AreaAnomaly> dict = new Dictionary<AreaEnum, AreaAnomaly>();
    List<AreaEnum> areas = new List<AreaEnum>();
    public List<Anomaly> ActiveAnomalies = new List<Anomaly>();

    [Header("Enemy Event")]
    public bool lockEnemyUpdate = false;
    [SerializeField] TestEnemy2 enemy;
    public List<LevelData> timedLevelUpdate;
    public List<LevelAnomalyData> timedAnomalyUpdate;
    int eventIndex = 0;
    float nextEventTime;

    private void Start()
    {
        DataContainer = dataSwitcher.currentData;

        AllAnomalies = FindObjectsByType<Anomaly>(FindObjectsSortMode.None);
        CreateAreaAnomalyDict();

        timedLevelUpdate = DataContainer.Content.levelConfigs;
        timedAnomalyUpdate = DataContainer.Content.AnomalyConfig;
        currentLevel = DataContainer.level;
        foreach (LevelAnomalyData data in timedAnomalyUpdate)
        {
            data.CreateList();
            //Debug.Log("Created Data list:" + data.AnomalyId);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            foreach (var areaAnomaly in dict)
            {
                foreach (var item in areaAnomaly.Value.lightAnomalies)
                {
                    Debug.Log("Area: " + areaAnomaly.Value.areaEnum + ", Light Anomaly: " + item.id);
                }
                foreach (var item in areaAnomaly.Value.heavyAnomalies)
                {
                    Debug.Log("Area: " + areaAnomaly.Value.areaEnum + ", Heavy Anomaly: " + item.id);
                }
                foreach (var item in areaAnomaly.Value.DisabledAnomalies)
                {
                    Debug.Log("Area: " + areaAnomaly.Value.areaEnum + ", Disabled Anomaly: " + item.id);
                }
            }
        }
    }
    public void CheckEnemyEvent(float currentTime) //Check when to update ai and level data
    {
        if(eventIndex < 5 && !lockEnemyUpdate)
        {
            if (timedLevelUpdate[eventIndex] != null)
            {
                if (nextEventTime <= currentTime)
                {
                    UpdateLevelData(timedLevelUpdate[eventIndex]);//Update enemy Ai 
                    UpdateAnomaliesData(eventIndex);
                    eventIndex++;
                    nextEventTime = timedLevelUpdate[eventIndex].Time;
                    Debug.Log("Change enemy AI at time: " + currentTime);
                    Debug.Log("Next enemy AI Update at: " + nextEventTime);
                }
            }
        }      
    }

    private void UpdateLevelData(LevelData data) //update ai and level data
    {
        enemy.difficultyLevel = data.Difficulty;
        enemy.cooldownDuration = data.ActiveInterval;
        enemy.lightAnomalyThreshold = data.LightAnomalyThreshold;
        enemy.heavyAnomalyThreashold = data.HeavyAnomalyThreshold;

        GameManager.instance.levelManager.incenseSpeed = data.IncenseDrainSpeed;
        GameManager.instance.levelManager.timeSpeed = data.TimeScale;
    }

    private void UpdateAnomaliesData(int index) //Update anomaly data inside container
    {
        foreach (Anomaly anomaly in AllAnomalies)
        {
            var data = DataContainer.Content.AnomalyConfig.FirstOrDefault(d => d.AnomalyId == anomaly.id);
            if (data != null)
            {
                string newAnomalyState = data.activationTimes[index];
                anomaly.SetAnomalyEnabled(newAnomalyState);
            }
        }

        ResetAvailableAnomalyLists();
    }

    private void CreateAreaAnomalyDict()
    {
        areas = System.Enum.GetValues(typeof(AreaEnum)).Cast<AreaEnum>().ToList();
        foreach (var area in areas)
        {
            if (area != AreaEnum.Default)
            {
                AreaAnomaly anomalyContainer = new AreaAnomaly();
                anomalyContainer.areaEnum = area;
                dict.Add(area, anomalyContainer);
            }
        }


        if (AllAnomalies != null)
        {
            foreach (Anomaly anomaly in AllAnomalies) //Assign all anomaly into a list by type
            {
                if (anomaly.area != AreaEnum.Default)
                {
                    var data = DataContainer.Content.anomalies.FirstOrDefault(d => d.Id == anomaly.id);
                    if (data != null)
                    {
                        anomaly.Initialize(data);
                    }
                    else
                    {
                        Debug.LogWarning("Couldn't find anomaly:" + anomaly.id, anomaly);
                    }

                    AddAnomalyToAvailableList(anomaly);
                }
            }
        }
    }

    public void AddAnomalyToAvailableList(Anomaly anomaly) //return anomaly to their container
    {
        if (anomaly.isEnabled == false)
        {
            dict[anomaly.area].DisabledAnomalies.Add(anomaly);
            Debug.Log("Added disabled anomaly: " + anomaly.name);

        }
        else if (anomaly.anomalyLevel == AnomalyEnum.LightAnomaly)
        {
            dict[anomaly.area].lightAnomalies.Add(anomaly);
            Debug.Log("Added light anomaly: " + anomaly.name + ", in area: " + anomaly.area);
        }
        else if (anomaly.anomalyLevel == AnomalyEnum.HeavyAnomaly)
        {
            dict[anomaly.area].heavyAnomalies.Add(anomaly);
            Debug.Log("Added heavy anomaly: " + anomaly.name + ", in area: " + anomaly.area);
        }
        else if (anomaly.anomalyLevel == AnomalyEnum.AttackAnomaly)
        {
            dict[anomaly.area].attackAnomalies.Add(anomaly);
            Debug.Log("Added light anomaly: " + anomaly.name + ", in area: " + anomaly.area);
        }
        else if (anomaly.anomalyLevel == AnomalyEnum.NotRandomSpawn)
        {

        }
        else
        {
            Debug.LogWarning("Anomaly Not Assigned Type: " + anomaly.name);
        }
    }

    public void EnableAllAnomaly()
    {
        foreach (Anomaly anomaly in AllAnomalies)
        {
            anomaly.SetAnomalyEnabled("TRUE");
        }

        ResetAvailableAnomalyLists();
    }

    private void ResetAvailableAnomalyLists() //move anomalies to correct list in container 
    {
        foreach (var areaAnomaly in dict)
        {
            List<Anomaly> removeLight = new List<Anomaly>();
            List<Anomaly> removeHeavy = new List<Anomaly>();
            List<Anomaly> removeDisabled = new List<Anomaly>();
            foreach (Anomaly anomaly in areaAnomaly.Value.lightAnomalies)
            {
                if (!anomaly.isEnabled)
                {
                    areaAnomaly.Value.DisabledAnomalies.Add(anomaly);
                    removeLight.Add(anomaly);
                }
            }

            foreach (Anomaly anomaly in areaAnomaly.Value.heavyAnomalies)
            {
                if (!anomaly.isEnabled)
                {
                    areaAnomaly.Value.DisabledAnomalies.Add(anomaly);
                    removeHeavy.Add(anomaly);
                }
            }

            foreach (Anomaly anomaly in areaAnomaly.Value.DisabledAnomalies)
            {
                if (anomaly.isEnabled)
                {
                    AddAnomalyToAvailableList(anomaly);
                    removeDisabled.Add(anomaly);
                }
            }

            foreach (var item in removeLight)
            {
                areaAnomaly.Value.lightAnomalies.Remove(item);
            }
            foreach (var item in removeHeavy)
            {
                areaAnomaly.Value.heavyAnomalies.Remove(item);
            }
            foreach (var item in removeDisabled)
            {
                areaAnomaly.Value.DisabledAnomalies.Remove(item);
            }
            Debug.Log("Reset area" + areaAnomaly.Value.areaEnum);
        }
    }
}
