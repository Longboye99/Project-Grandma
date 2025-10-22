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

    [Header("Anomaly Spawning Chance")]
    [SerializeField] float AreaChance2;
    [SerializeField] float AreaChance3;
    [SerializeField] float AreaChance4;
    [SerializeField] float AreaChance5;

    [SerializeField] float AttackChance;
    [SerializeField] float HeavyChance0;
    [SerializeField] float HeavyChance1;
    [SerializeField] float HeavyChance2;
    [SerializeField] float LightDoubleChance;


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
        SpawnAnomaly();
    }

    private void CheckEnemyEvent()
    {  
        int currentLoop = GameManager.instance.levelManager.currentLoop;
        if(timedLevelUpdate[currentLoop] != null)
        {
            UpdateLevelData(timedLevelUpdate[currentLoop]);
            UpdateAnomaliesData(currentLoop);
            Debug.Log("Change enemy AI at time: " + currentLoop);
        }
    }

    private void UpdateLevelData(LevelData data)
    {
        AreaChance2 = data.AreaChance2;
        AreaChance3 = data.AreaChance3;
        AreaChance4 = data.AreaChance4;
        AreaChance5 = data.AreaChance5;

        AttackChance = data.AttackChance;
        HeavyChance0 = data.HeavyChance0;
        HeavyChance1 = data.HeavyChance1;
        HeavyChance2 = data.HeavyChance2;
        LightDoubleChance = data.LightDoubleChance;
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

    private void SpawnAnomaly()
    {
        //random area number
        int randomAreaCount = GetRandomValue(
            new RandomSelection(2,2,AreaChance2),
            new RandomSelection(3,3,AreaChance3),
            new RandomSelection(4,4,AreaChance4),
            new RandomSelection(5,5,AreaChance5)
            );

        List<AreaAnomaly> availableArea = RandomSpawnArea(randomAreaCount);

        //if attack anomaly spawn
        int rnd;
        int randomAttack = Random.Range(0, 100);
        if(randomAttack <= AttackChance * 100 )
        {
            //where attack anomaly
            rnd = Random.Range(0,availableArea.Count);
            AreaAnomaly targetArea = availableArea[rnd];
            availableArea.RemoveAt(rnd);

            //which anomaly
            rnd = Random.Range(0, targetArea.attackAnomalies.Count);
            targetArea.attackAnomalies[rnd].TriggerAnomaly();

            //active
            ActiveAnomalies.Add(targetArea.attackAnomalies[rnd]);
            targetArea.attackAnomalies.RemoveAt(rnd);
        }

        //if heavy anomaly spawn
        int randomHeavyAnomalyCount = GetRandomValue(
            new RandomSelection(0,0,HeavyChance0),
            new RandomSelection(1,1,HeavyChance1),
            new RandomSelection(2,2,HeavyChance2)
            );
        if(randomHeavyAnomalyCount > 0)
        {
            for (int i = 0; i < randomHeavyAnomalyCount; i++)
            {
                //where
                rnd = Random.Range(0, availableArea.Count);
                AreaAnomaly targetArea = availableArea[rnd];
                availableArea.RemoveAt(rnd);

                rnd = Random.Range(0, targetArea.heavyAnomalies.Count);
                targetArea.heavyAnomalies[rnd].TriggerAnomaly();

                ActiveAnomalies.Add(targetArea.heavyAnomalies[rnd]);
                targetArea.heavyAnomalies.RemoveAt(rnd);
            }
            
        }
        //spawn light anomaly in left over areas
        foreach (var area in availableArea)
        {
            if (availableArea.Count == 0) break;

            //random how many light anomaly the rest of the area has
            int randomDouble = Random.Range(0, 100);
            if(randomDouble <= AttackChance * 100)
            {
                //spawn twice
                for (int i = 0; i < 2; i++)
                {
                    rnd = Random.Range(0, area.lightAnomalies.Count);
                    area.lightAnomalies[rnd].TriggerAnomaly();

                    ActiveAnomalies.Add(area.lightAnomalies[rnd]);
                    area.lightAnomalies.RemoveAt(rnd);
                }
            }
            else
            {
                //spawn once
                rnd = Random.Range(0, area.lightAnomalies.Count);
                area.lightAnomalies[rnd].TriggerAnomaly();

                ActiveAnomalies.Add(area.lightAnomalies[rnd]);
                area.lightAnomalies.RemoveAt(rnd);
            }
            availableArea.Remove(area);
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
            /*//Filter out area with no anomaly enable(This shouldnt exist)
            if(area.Value.lightAnomalies.Count > 0 || area.Value.lightAnomalies.Count > 0 || area.Value.attackAnomalies.Count > 0)
            {
                availableArea.Add(area.Value);
                Debug.Log("Available Area :" + area.Value.areaEnum.ToString());

            }*/
            availableArea.Add(area.Value);
            Debug.Log("Available Area :" + area.Value.areaEnum.ToString());

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
            AreaAnomaly anomalyContainer = new AreaAnomaly();
            anomalyContainer.areaEnum = area;
            dict.Add(area, anomalyContainer);
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
