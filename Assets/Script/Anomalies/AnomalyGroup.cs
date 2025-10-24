using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnomalyGroup : Anomaly
{
    [Header("Sequence Config")]
    [SerializeField] private List<Anomaly> anomalyList;

    public override void TriggerAnomaly()
    {
        foreach (Anomaly anomaly in anomalyList)
        {
            anomaly.TriggerAnomaly();
        }
        isActive = true;
        currentAnomalyPoint = anomalyPoint;
    }

    public override void UndoAnomaly(Anomaly incomingAnomaly)
    {
        if (anomalyList.Contains(incomingAnomaly))
        {
            foreach (Anomaly anomaly in anomalyList)
            {
                if(anomaly.isActive)
                {
                    GameEventsManager.instance.anomalyEvents.UndoAnomaly(anomaly);
                }
            }
        }      
    }
}