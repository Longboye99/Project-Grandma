using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Database;

public class AnomalyManager : MonoBehaviour
{
    [SerializeField] SpreadsheetContainer DataContainer;

    [Header("State")]
    public int anomalyPoint;
    public float currentTime;
    public float finishTime;
    public AreaEnum currentArea;

    [Header("Enemy Event")]
    [SerializeField] TestEnemy2 enemy;
    public List<LevelData> timedLevelUpdate;
    public List<LevelAnomalyData> timedAnomalyUpdate;
    int eventIndex = 0;
    float nextEventTime;

    int spawningTries = 4;

    Anomaly[] AllAnomalies;
    public Dictionary<AreaEnum, AreaAnomaly> dict = new Dictionary<AreaEnum, AreaAnomaly>();
    List<AreaEnum> areas = new List<AreaEnum>();
    [SerializeField] private List<Anomaly> ActiveAnomalies = new List<Anomaly>();

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += UndoAnomaly;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= UndoAnomaly;
    }

    private void Start()
    {
        AllAnomalies = FindObjectsByType<Anomaly>(FindObjectsSortMode.None);
        CreateAreaAnomalyDict();
        timedLevelUpdate = DataContainer.Content.levelConfigs;
        timedAnomalyUpdate = DataContainer.Content.AnomalyConfig;
        foreach (LevelAnomalyData data in timedAnomalyUpdate)
        {
            data.CreateList();
        }
    }

    public void CheckEnemyEvent(float currentTime)
    {  
        if(timedLevelUpdate[eventIndex] != null)
        {
            if (nextEventTime <= currentTime)
            {
                UpdateLevelData(timedLevelUpdate[eventIndex]);//Update enemy Ai 
                UpdateAnomalies(eventIndex);
                eventIndex++;
                nextEventTime = timedLevelUpdate[eventIndex].Time;
                Debug.Log("Change enemy AI at time: " + currentTime);
                Debug.Log("Next enemy AI Update at: " + nextEventTime);
            }
        }
    }

    private void UpdateLevelData(LevelData data)
    {
        enemy.difficultyLevel = data.Difficulty;
        enemy.cooldownDuration = data.ActiveInterval;
        enemy.lightAnomalyThreshold = data.LightAnomalyThreshold;
        enemy.heavyAnomalyThreashold = data.HeavyAnomalyThreshold;

        GameManager.instance.levelManager.incenseSpeed = data.IncenseDrainSpeed;
        GameManager.instance.levelManager.timeSpeed = data.TimeScale;
    }

    private void UpdateAnomalies(int index)
    {
        foreach (Anomaly anomaly in AllAnomalies)
        {
            var data = DataContainer.Content.AnomalyConfig.FirstOrDefault(d => d.AnomalyId == anomaly.id);
            if(data != null)
            {
                string newAnomalyState = data.activationTimes[index];
                anomaly.SetAnomalyEnabled(newAnomalyState);
            }
        }
    }

    public bool SpawnRandomLightAnomaly()//Randomly trigger a light anomaly
    {
        List<AreaEnum> avalableArea = ExcludeCurrentAreaList(currentArea);
        AreaAnomaly targetArea;
        int random;

        for (int i = 0; i < spawningTries; i++)
        {
            random = Random.Range(0, avalableArea.Count);
            targetArea = dict[avalableArea[random]];

            if (targetArea.lightAnomalies.Count > 0)
            {
                random = Random.Range(0, targetArea.lightAnomalies.Count);
                if (targetArea.lightAnomalies[random].SpawnAnomaly() == true)
                {
                    GameEventsManager.instance.anomalyEvents.TriggerHeavyAnomaly();
                    ActiveAnomalies.Add(targetArea.lightAnomalies[random]);
                    targetArea.lightAnomalies.RemoveAt(random);
                    return true;
                }
            }
        }

        targetArea = dict[currentArea];
        for (int i = 0; i < spawningTries; i++)
        {
            if (targetArea.lightAnomalies.Count > 0)
            {
                random = Random.Range(0, targetArea.lightAnomalies.Count);
                if (targetArea.lightAnomalies[random].SpawnAnomaly() == true)
                {
                    GameEventsManager.instance.anomalyEvents.TriggerHeavyAnomaly();
                    ActiveAnomalies.Add(targetArea.lightAnomalies[random]);
                    targetArea.lightAnomalies.RemoveAt(random);
                    return true;
                }
            }
        }
        

        Debug.Log("Failed to Spawn Light Anomaly");
        return false;
    }

    public bool SpawnRandomHeavyAnomaly()//Randomly trigger a heavy anomaly
    {
        List<AreaEnum> avalableArea = ExcludeCurrentAreaList(currentArea);
        AreaAnomaly targetArea;
        int random;

        for (int i = 0; i < spawningTries; i++)
        {
            random = Random.Range(0, avalableArea.Count);
            targetArea = dict[avalableArea[random]];

            if (targetArea.heavyAnomalies.Count > 0)
            {
                random = Random.Range(0, targetArea.heavyAnomalies.Count);
                if (targetArea.heavyAnomalies[random].SpawnAnomaly() == true)
                {
                    GameEventsManager.instance.anomalyEvents.TriggerHeavyAnomaly();
                    ActiveAnomalies.Add(targetArea.heavyAnomalies[random]);
                    targetArea.heavyAnomalies.RemoveAt(random);
                    return true;
                }
            }
        }

        targetArea = dict[currentArea];
        for (int i = 0; i < spawningTries; i++)
        {
            if (targetArea.heavyAnomalies.Count > 0)
            {
                random = Random.Range(0, targetArea.heavyAnomalies.Count);
                if (targetArea.heavyAnomalies[random].SpawnAnomaly() == true)
                {
                    GameEventsManager.instance.anomalyEvents.TriggerHeavyAnomaly();
                    ActiveAnomalies.Add(targetArea.heavyAnomalies[random]);
                    targetArea.heavyAnomalies.RemoveAt(random);
                    return true;
                }
            }
        }
        

        Debug.Log("Failed to Spawn Heavy Anomaly");
        return false;
    }

    public bool SpawnRandomAttackAnomaly()//Randomly trigger an attack anomaly
    {
        AreaAnomaly targetArea = dict[currentArea];

        for (int i = 0; i < spawningTries; i++)
        {
            int random = Random.Range(0, targetArea.attackAnomalies.Count);
            if (targetArea.attackAnomalies.Count > 0)
            {
                if (targetArea.attackAnomalies[random].SpawnAnomaly() == true)
                {
                    GameEventsManager.instance.anomalyEvents.TriggerAttackAnomaly();
                    ActiveAnomalies.Add(targetArea.attackAnomalies[random]);
                    targetArea.attackAnomalies.RemoveAt(random);
                    return true;
                }
            }
        }
        
        return false;
    }

    public int TallyAnomalyPoint() //Loop through all anomalies connected to this node and sum up the anomaly point
    {
        anomalyPoint = 0;
        foreach (Anomaly anomaly in ActiveAnomalies)
        {
            anomalyPoint += anomaly.anomalyPoint;
        }
        return anomalyPoint;
    }

    private void UndoAnomaly(Anomaly anomaly)
    {
        if (!ActiveAnomalies.Contains(anomaly))
        { return; }

        AddAnomalyToInactiveList(anomaly);

        ActiveAnomalies.Remove(anomaly);
    }

    public void UndoAllAnomaly()
    {
        foreach (Anomaly anomaly in ActiveAnomalies)
        {
            AddAnomalyToInactiveList(anomaly);
        }
        ActiveAnomalies.Clear();
    }

    private void CreateAreaAnomalyDict()
    {
        areas = System.Enum.GetValues(typeof(AreaEnum)).Cast<AreaEnum>().ToList();
        foreach (var area in areas)
        {
            AreaAnomaly anomalyContainer = new AreaAnomaly();
            anomalyContainer.areaEnum = area;
            dict.Add(area, anomalyContainer);
            Debug.Log(dict[area].areaEnum);
        }


        if (AllAnomalies != null)
        {
            foreach (Anomaly anomaly in AllAnomalies) //Assign all anomaly into a list by type
            {
                var data = DataContainer.Content.anomalies.First(d => d.Id == anomaly.id);
                if(data != null)
                {
                    anomaly.Initialize(data);
                }
                
                AddAnomalyToInactiveList(anomaly);
            }
        }
    }

    private void AddAnomalyToInactiveList(Anomaly anomaly)
    {
        if (anomaly.anomalyLevel == AnomalyEnum.LightAnomaly)
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

    private List<AreaEnum> ExcludeCurrentAreaList(AreaEnum area)
    {
        List<AreaEnum> excludeList = areas;
        excludeList.Remove(area);
        return excludeList;
    }
}
