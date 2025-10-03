using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AnomalyManager : MonoBehaviour
{
    [Header("State")]
    public int anomalyPoint;
    public float currentTime;
    public float finishTime;
    public AreaEnum currentArea;

    [Header("Enemy Event")]
    public List<TimedEnemyBehavior> enemyEvents = new List<TimedEnemyBehavior>();
    int eventIndex = 0;
    float nextEventTime;

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
        CreateAreaAnomalyDict();
    }

    public void CheckEnemyEvent()
    {
        if(enemyEvents != null)
        {
            if (eventIndex < enemyEvents.Count)
            {
                if (nextEventTime <= currentTime)
                {
                    enemyEvents[eventIndex].UpdateEnemyAI(); //Update enemy Ai using method in EnemyEvent class
                    eventIndex++;
                    nextEventTime = enemyEvents[eventIndex].eventTime;
                    Debug.Log("Change enemy AI at time: " + currentTime);
                }
            }
        }
    }

    public bool SpawnRandomLightAnomaly()//Randomly trigger a light anomaly
    {
        List<AreaEnum> avalableArea = ExcludeCurrentAreaList(currentArea);
        int random = Random.Range(0, avalableArea.Count);
        AreaAnomaly targetArea = dict[avalableArea[random]];

        random = Random.Range(0, targetArea.lightAnomalies.Count);
        if(targetArea.lightAnomalies.Count > 0)
        {
            if (targetArea.lightAnomalies[random].SpawnAnomaly() == true)
            {
                GameEventsManager.instance.anomalyEvents.TriggerLightAnomaly();
                ActiveAnomalies.Add(targetArea.lightAnomalies[random]);
                targetArea.lightAnomalies.RemoveAt(random);
                return true;
            }
        }
        return false;
    }

    public bool SpawnRandomHeavyAnomaly()//Randomly trigger a heavy anomaly
    {
        List<AreaEnum> avalableArea = ExcludeCurrentAreaList(currentArea);
        int random = Random.Range(0, avalableArea.Count);
        AreaAnomaly targetArea = dict[avalableArea[random]];

        random = Random.Range(0, targetArea.heavyAnomalies.Count);
        if(targetArea.heavyAnomalies.Count > 0)
        {
            if (targetArea.heavyAnomalies[random].SpawnAnomaly() == true)
            {
                GameEventsManager.instance.anomalyEvents.TriggerHeavyAnomaly();
                ActiveAnomalies.Add(targetArea.heavyAnomalies[random]);
                targetArea.heavyAnomalies.RemoveAt(random);
                return true;
            }
        }
        return false;
    }

    public bool SpawnRandomAttackAnomaly()//Randomly trigger an attack anomaly
    {
        List<AreaEnum> avalableArea = ExcludeCurrentAreaList(currentArea);
        int random = Random.Range(0, avalableArea.Count);
        AreaAnomaly targetArea = dict[avalableArea[random]];

        random = Random.Range(0, targetArea.attackAnomalies.Count);
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
        return false;
    }

    public int TallyAnomalyPoint() //Loop through all anomalies connected to this node and sum up the anomaly point
    {
        anomalyPoint = 0;
        foreach (Anomaly anomaly in ActiveAnomalies)
        {
            anomalyPoint += anomaly.anomalyPointValue;
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

        Anomaly[] anomalies = FindObjectsByType<Anomaly>(FindObjectsSortMode.None);

        if (anomalies != null)
        {
            foreach (Anomaly anomaly in anomalies) //Assign all anomaly into a list by type
            {
                AddAnomalyToInactiveList(anomaly);
            }
        }
    }

    private void AddAnomalyToInactiveList(Anomaly anomaly)
    {
        if (anomaly.anomalyEnum == AnomalyEnum.LightAnomaly)
        {
            dict[anomaly.areaEnum].lightAnomalies.Add(anomaly);
            Debug.Log("Added light anomaly: " + anomaly.name + ", in area: " + anomaly.areaEnum);
        }
        else if (anomaly.anomalyEnum == AnomalyEnum.HeavyAnomaly)
        {
            dict[anomaly.areaEnum].heavyAnomalies.Add(anomaly);
            Debug.Log("Added heavy anomaly: " + anomaly.name + ", in area: " + anomaly.areaEnum);
        }
        else if (anomaly.anomalyEnum == AnomalyEnum.AttackAnomaly)
        {
            dict[anomaly.areaEnum].attackAnomalies.Add(anomaly);
            Debug.Log("Added light anomaly: " + anomaly.name + ", in area: " + anomaly.areaEnum);
        }
        else if (anomaly.anomalyEnum == AnomalyEnum.PartOfSequence)
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
