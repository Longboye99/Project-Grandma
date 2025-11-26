using UnityEngine;

public class DisappearingAnomaly : Anomaly
{
    //Anomaly class for anomalies that make objects disappear

    public override void TriggerAnomaly()
    {
        Debug.Log("Triggered Disappearing Anomaly: " + this.name);
        isActive = true;
        CurrentCooldown = cooldown;
        currentAnomalyPoint = anomalyPoint;

        gameObject.GetComponent<MeshRenderer>().enabled = false; //Make the object disappear      
    }

    public override void UndoAnomaly(Anomaly anomaly)
    {
        if(anomaly == this && isActive)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true; //Make anomaly appear back to normal
            currentAnomalyPoint = 0;
            isActive = false;
        }       
    }
}
