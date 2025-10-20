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
    public AreaEnum currentArea;
    public int spawningAreaCount;
    

    [Header("Enemy Event")]
    public List<LevelData> timedLevelUpdate;
    public List<LevelAnomalyData> timedAnomalyUpdate;


    Anomaly[] AllAnomalies;
    public Dictionary<AreaEnum, AreaAnomaly> dict = new Dictionary<AreaEnum, AreaAnomaly>();
    List<AreaEnum> areas = new List<AreaEnum>();
    public List<Anomaly> ActiveAnomalies = new List<Anomaly>();

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

    public void SpawnNextLoopAnomaly()
    {
        CheckEnemyEvent();
        ResetAvailableAnomalyLists();
        List<AreaAnomaly> availableArea = RandomSpawnArea();
        //Spawn anomalies stuff 
    }

    private void CheckEnemyEvent()
    {  
        int currentLoop = GameManager.instance.levelManager.currentLoop;
        if(timedLevelUpdate[currentLoop] != null)
        {
            UpdateAnomaliesData(currentLoop);
            Debug.Log("Change enemy AI at time: " + currentLoop);
        }
    }

    private void UpdateAnomaliesData(int index)
    {
        foreach (Anomaly anomaly in AllAnomalies)
        {
            var data = DataContainer.Content.AnomalyConfig.FirstOrDefault(d => d.AnomalyId == anomaly.id);
            if(data != null)
            {
                string newAnomalyState = data.activationTimes[index];
                anomaly.SetAnomalyEnabled(newAnomalyState);
                Debug.Log("Update Anomaly Data of Loop: " + index + " | " + anomaly.name + " : " +  newAnomalyState);
            }
        }
    }
    /*
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

    /*
    public int TallyAnomalyPoint() //Loop through all anomalies connected to this node and sum up the anomaly point
    {
        anomalyPoint = 0;
        foreach (Anomaly anomaly in ActiveAnomalies)
        {
            anomalyPoint += anomaly.anomalyPoint;
        }
        return anomalyPoint;
    }*/

    private void UndoAnomaly(Anomaly anomaly)
    {
        if (!ActiveAnomalies.Contains(anomaly))
        { return; }

        AddAnomalyToAvailableList(anomaly);

        ActiveAnomalies.Remove(anomaly);
    }

    public void UndoAllAnomaly()
    {
        foreach (Anomaly anomaly in ActiveAnomalies)
        {
            AddAnomalyToAvailableList(anomaly);
        }
        ActiveAnomalies.Clear();
    }

    private void AddAnomalyToAvailableList(Anomaly anomaly)
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

    private void ResetAvailableAnomalyLists()
    {
        UndoAllAnomaly();

        foreach (var areaAnomaly in dict)
        {
            foreach (Anomaly anomaly in areaAnomaly.Value.lightAnomalies)
            {
                if (!anomaly.enabled)
                {
                    areaAnomaly.Value.DisabledAnomalies.Add(anomaly);
                    areaAnomaly.Value.lightAnomalies.Remove(anomaly);
                }
                
            }

            foreach (Anomaly anomaly in areaAnomaly.Value.heavyAnomalies)
            {
                if (!anomaly.enabled)
                {
                    areaAnomaly.Value.DisabledAnomalies.Add(anomaly);
                    areaAnomaly.Value.heavyAnomalies.Remove(anomaly);
                }

            }

            foreach (Anomaly anomaly in areaAnomaly.Value.attackAnomalies)
            {
                if (anomaly.enabled)
                {
                    areaAnomaly.Value.DisabledAnomalies.Add(anomaly);
                    areaAnomaly.Value.attackAnomalies.Remove(anomaly);
                }

            }

            foreach (Anomaly anomaly in areaAnomaly.Value.DisabledAnomalies)
            {
                if (!anomaly.enabled)
                {
                    AddAnomalyToAvailableList(anomaly);
                    areaAnomaly.Value.DisabledAnomalies.Remove(anomaly);
                }

            }
        }
    }

    private List<AreaAnomaly> RandomSpawnArea()
    {
        List<AreaAnomaly> availableArea = new List<AreaAnomaly>();
        foreach (var area in dict)
        {
            //Filter out area with no anomaly enable
            if(area.Value.lightAnomalies.Count > 0 || area.Value.lightAnomalies.Count > 0 || area.Value.attackAnomalies.Count > 0)
            {
                availableArea.Add(area.Value);
            }
        }

        //Remove area til we have the right amount
        int areaToRemove = availableArea.Count - spawningAreaCount;
        for (int i = 0; i < areaToRemove; i++)
        {
            if(availableArea.Count <= spawningAreaCount)
            {
                break;
            }
            int rd = Random.Range(0, availableArea.Count);
            availableArea.RemoveAt(rd);
        }

        return availableArea;
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
                if (data != null)
                {
                    anomaly.Initialize(data);
                }

                AddAnomalyToAvailableList(anomaly);
            }
        }
    }

}
