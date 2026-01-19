using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Database;

public class AnomalyManager : MonoBehaviour
{
    [SerializeField] LocalSpreadsheetContainer DataContainer;

    [Header("State")]
    public int anomalyPoint;
    public float currentTime;
    public float finishTime;
    public AreaEnum currentArea;

    bool hasJumpscared;
    [SerializeField] AttackAnomaly jumpscare;

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
            //Debug.Log("Created Data list:" + data.AnomalyId);
        }
    }

    public void CheckEnemyEvent(float currentTime)
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

    public int TallyAnomalyPoint() //Loop through all anomalies connected to this node and sum up the anomaly point
    {
        anomalyPoint = 0;
        foreach (Anomaly anomaly in ActiveAnomalies)
        {
            anomalyPoint += anomaly.anomalyPoint;
        }
        return anomalyPoint;
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

    private void UpdateAnomaliesData(int index)
    {
        foreach (Anomaly anomaly in AllAnomalies)
        {
            var data = DataContainer.Content.AnomalyConfig.FirstOrDefault(d => d.AnomalyId == anomaly.id);
            if(data != null)
            {
                string newAnomalyState = data.activationTimes[index];
                anomaly.SetAnomalyEnabled(newAnomalyState);
                //Debug.Log("Update Anomaly Data of Loop: " + index + " | " + anomaly.name + " : " +  newAnomalyState);
            }
        }
    }

    public bool SpawnRandomLightAnomaly()//Randomly trigger a light anomaly
    {
        AreaAnomaly targetArea;
        int random;

        for (int i = 0; i < spawningTries; i++)
        {
            targetArea = RandomSpawnArea(1)[0];

            if (targetArea.lightAnomalies.Count > 0)
            {
                random = Random.Range(0, targetArea.lightAnomalies.Count);
                if (targetArea.lightAnomalies[random].SpawnAnomaly() == true)
                {
                    GameEventsManager.instance.anomalyEvents.TriggerLightAnomaly();
                    ActiveAnomalies.Add(targetArea.lightAnomalies[random]);
                    targetArea.lightAnomalies.RemoveAt(random);
                    return true;
                }
            }
        }

        Debug.Log("Failed to Spawn Light Anomaly");
        return false;
    }

    public bool SpawnRandomHeavyAnomaly()//Randomly trigger a Heavy anomaly
    {
        AreaAnomaly targetArea;
        int random;

        for (int i = 0; i < spawningTries; i++)
        {
            targetArea = RandomSpawnArea(1)[0];

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

    public void TriggerAnomaly(Anomaly anomaly)
    {       
        if(anomaly.isActive == false)
        {
            anomaly.TriggerAnomaly();
            ActiveAnomalies.Add(anomaly);
        }         
    }

    
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
            anomaly.UndoAnomaly(anomaly);
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

    private List<AreaAnomaly> RandomSpawnArea(int spawningAreaCount)
    {
        List<AreaAnomaly> availableArea = new List<AreaAnomaly>();
        foreach (var area in dict)
        {
            //Filter out area with no anomaly enable(This shouldnt exist)
            if(area.Value.lightAnomalies.Count > 0 || area.Value.lightAnomalies.Count > 0 || area.Value.attackAnomalies.Count > 0 || area.Value.areaEnum != AreaEnum.Default)
            {
                availableArea.Add(area.Value);
                //Debug.Log("Available Area :" + area.Value.areaEnum.ToString());

            }
            /*availableArea.Add(area.Value);
            Debug.Log("Available Area :" + area.Value.areaEnum.ToString());*/

        }

        if(availableArea.Contains(dict[currentArea])) //if enemy not in heavy phase, dont spawn anomaly in front of player
        {
            availableArea.Remove(dict[currentArea]);
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

        foreach (var area in availableArea)
        {
            Debug.Log("Spawnable Area :" + area.areaEnum.ToString());
        }

        return availableArea;
    }

    private void CreateAreaAnomalyDict()
    {
        areas = System.Enum.GetValues(typeof(AreaEnum)).Cast<AreaEnum>().ToList();
        foreach (var area in areas)
        {
            if(area != AreaEnum.Default)
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
                if(anomaly.area != AreaEnum.Default)
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

    struct RandomSelection
    {
        private int minValue;
        private int maxValue;
        public float probability;

        public RandomSelection(int minValue, int maxValue, float probability)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.probability = probability;
        }

        public int GetValue() { return Random.Range(minValue, maxValue + 1); }
    }

    int GetRandomValue(params RandomSelection[] selections)
    {
        float rand = Random.value;
        float currentProb = 0;
        foreach (var selection in selections)
        {
            currentProb += selection.probability;
            if (rand <= currentProb)
                return selection.GetValue();
        }

        //will happen if the input's probabilities sums to less than 1
        //throw error here if that's appropriate
        return -1;
    }

}
