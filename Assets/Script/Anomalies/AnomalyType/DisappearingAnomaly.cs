using UnityEngine;

public class DisappearingAnomaly : Anomaly
{
    //Anomaly class for anomalies that make objects disappear
    private void Start()
    {
        originalMaterial = GetComponent<MeshRenderer>().material; //Save default object material
    }

    public override void TriggerAnomaly()
    {

        isActive = true;
        currentAnomalyPoint = anomalyPoint;
        Debug.Log("Trigger Disappearing Anomaly: " + this.name);

        gameObject.GetComponent<MeshRenderer>().enabled = false; //Make the object disappear      
    }

    public override void UndoAnomaly(Anomaly anomaly)
    {
        if(anomaly == this && isActive)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true; //Make anomaly appear back to normal
            currentAnomalyPoint = 0;
            isActive = false;
            CurrentCooldown = cooldown;
        }       
    }
}
