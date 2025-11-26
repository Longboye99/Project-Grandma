using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnomalySequence : Anomaly
{
    [Header("Sequence Config")]
    [SerializeField] private List<Anomaly> anomalyList;
    [SerializeField] float[] delayTime;

    public int index = 0;

    public override void TriggerAnomaly()
    {
        anomalyList[index].TriggerAnomaly();
        Debug.Log("Trigger Anomaly Sequence: " + this.name + index);
        isActive = true;
        CurrentCooldown = cooldown;
        currentAnomalyPoint = anomalyPoint;
    }

    public override void UndoAnomaly(Anomaly anomaly)
    {
        if (index >= anomalyList.Count )
        {
            return;
        }
        if (anomaly == anomalyList[index] && isActive)
        {               
            currentAnomalyPoint = 0;
            isActive = false;
            index++;
            if( index >= anomalyList.Count)
            {
                index = 0;
            }
            else
            {
                Invoke("ContinueAnomaly", delayTime[index]);
            }            
        }       
    }

    private void ContinueAnomaly()
    {
        if (index <= anomalyList.Count)
        {
            anomalyList[index].TriggerAnomaly();
            isActive = true;
            currentAnomalyPoint = anomalyPoint;
        }
    }
}