using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnomalyGroup : Anomaly
{
    [Header("Sequence Config")]
    [SerializeField] private List<Anomaly> anomalyList;

    public override void TriggerAnomaly()
    {
        Debug.Log("Trigger Anomaly Group: " + this.name);
        foreach (Anomaly anomaly in anomalyList)
        {
            anomaly.TriggerAnomaly();
        }
        isActive = true;
        currentAnomalyPoint = anomalyPoint;
    }

    public override void UndoAnomaly(Anomaly incomingAnomaly)
    {
        if (anomalyList.Contains(incomingAnomaly) && isActive)
        {
            isActive = false;
            currentAnomalyPoint = 0;
            CurrentCooldown = cooldown;
            foreach (Anomaly anomaly in anomalyList)
            {
                if(anomaly.isActive)
                {
                    GameEventsManager.instance.anomalyEvents.UndoAnomaly(anomaly);
                }
            }
            
            GameEventsManager.instance.anomalyEvents.UndoAnomaly(this);
        }
    }
}