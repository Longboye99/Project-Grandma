using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Database;

public class AnomalyManager : MonoBehaviour
{
    public AnomalyDictionaryHandler dictionary;
    bool hasJumpscared;
    [SerializeField] AttackAnomaly jumpscare;
    [SerializeField] FlashlightOverlay flashlightOverlay;
    int spawningTries = 4;
    public bool isHaywire = false;

    [Header("State")]
    public int anomalyPoint;
    public AreaEnum currentArea;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += UndoAnomaly;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= UndoAnomaly;
    }

    public int TallyAnomalyPoint() //Loop through all anomalies connected to this node and sum up the anomaly point
    {
        anomalyPoint = 0;
        foreach (Anomaly anomaly in dictionary.ActiveAnomalies)
        {
            anomalyPoint += anomaly.anomalyPoint;
        }
        return anomalyPoint;
    }

    public int TallyAreaAnomalyPoint()
    {
        int localAnomalyPoint = 0;
        foreach (Anomaly anomaly in dictionary.ActiveAnomalies)
        {
            if(anomaly.area == currentArea)
            {
                localAnomalyPoint += anomaly.anomalyPoint;
            }
        }
        return localAnomalyPoint;
    }

    public bool SpawnRandomLightAnomaly()//Randomly trigger a light anomaly
    {
        AreaAnomaly targetArea;
        int random;

        for (int i = 0; i < spawningTries; i++)
        {
            targetArea = RandomLightSpawnArea();
            if(targetArea == null)
            {

            }
            else if (targetArea.lightAnomalies.Count > 0)
            {
                random = Random.Range(0, targetArea.lightAnomalies.Count);
                if (targetArea.lightAnomalies[random].SpawnAnomaly() == true)
                {
                    GameEventsManager.instance.anomalyEvents.TriggerLightAnomaly();

                    if(targetArea.lightAnomalies[random].area == currentArea)
                    {
                        flashlightOverlay.Blink(null);
                    }

                    dictionary.ActiveAnomalies.Add(targetArea.lightAnomalies[random]);
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
            targetArea = RandomHeavySpawnArea();
            if( targetArea == null)
            {

            }
            else if (targetArea.heavyAnomalies.Count > 0)
            {
                random = Random.Range(0, targetArea.heavyAnomalies.Count);
                if (targetArea.heavyAnomalies[random].SpawnAnomaly() == true)
                {
                    GameEventsManager.instance.anomalyEvents.TriggerHeavyAnomaly();
                    if (targetArea.heavyAnomalies[random].area == currentArea)
                    {
                        flashlightOverlay.Blink(null);
                    }
                    dictionary.ActiveAnomalies.Add(targetArea.heavyAnomalies[random]);
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
            dictionary.ActiveAnomalies.Add(anomaly);
        }         
    }

    private void UndoAnomaly(Anomaly anomaly)
    {
        if (!dictionary.ActiveAnomalies.Contains(anomaly))
        { return; }

        dictionary.AddAnomalyToAvailableList(anomaly);

        dictionary.ActiveAnomalies.Remove(anomaly);
    }

    public void UndoAllAnomaly()
    {
        foreach (Anomaly anomaly in dictionary.ActiveAnomalies)
        {
            anomaly.UndoAnomaly(anomaly);
            dictionary.AddAnomalyToAvailableList(anomaly);
        }
        dictionary.ActiveAnomalies.Clear();
    }

    private AreaAnomaly RandomHeavySpawnArea()
    {
        List<AreaAnomaly> availableArea = new List<AreaAnomaly>();
        foreach (var area in dictionary.dict)
        {
            //Filter out area with no anomaly enable(This shouldnt exist)
            if(area.Value.heavyAnomalies.Count > 0 && area.Value.areaEnum != AreaEnum.Default)
            {
                availableArea.Add(area.Value);
                Debug.Log("Available Area :" + area.Value.areaEnum.ToString());
            }
        }

        if(availableArea.Contains(dictionary.dict[currentArea]) && !isHaywire) //if enemy not in heavy phase, dont spawn anomaly in front of player
        {
            availableArea.Remove(dictionary.dict[currentArea]);
        }

        foreach (var area in availableArea)
        {
            Debug.Log("Heavy Spawnable Area :" + area.areaEnum.ToString());
        }

        if( availableArea.Count == 0)
        {
            Debug.Log("No available Area with Heavy Anomaly");
            return null;
        }
        
        int random = Random.Range(0, availableArea.Count);
        Debug.Log("Selected Heavy Area: " + availableArea[random].areaEnum.ToString());

        return availableArea[random];
    }

    private AreaAnomaly RandomLightSpawnArea()
    {
        List<AreaAnomaly> availableArea = new List<AreaAnomaly>();
        foreach (var area in dictionary.dict)
        {
            //Filter out area with no anomaly enable(This shouldnt exist)
            if (area.Value.lightAnomalies.Count > 0 && area.Value.areaEnum != AreaEnum.Default)
            {
                availableArea.Add(area.Value);
                Debug.Log("Available Area :" + area.Value.areaEnum.ToString());
            }
        }

        if (availableArea.Contains(dictionary.dict[currentArea]) && !isHaywire) //if enemy not in heavy phase, dont spawn anomaly in front of player
        {
            availableArea.Remove(dictionary.dict[currentArea]);
        }

        foreach (var area in availableArea)
        {
            Debug.Log("Light Spawnable Area :" + area.areaEnum.ToString());
        }

        if (availableArea.Count == 0)
        {
            Debug.Log("No available Area with Light Anomaly");
            return null;
        }

        int random = Random.Range(0, availableArea.Count);
        Debug.Log("Selected Light Area: " + availableArea[random].areaEnum.ToString());
        return availableArea[random];
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
