using UnityEngine;

public class DisappearingAnomaly : Anomaly
{
    //Anomaly class for anomalies that make objects disappear
    private void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("Player");
        originalMaterial = GetComponent<MeshRenderer>().material; //Save default object material
    }

    public override void TriggerAnomaly()
    {
        Debug.Log("Triggered Disappearing Anomaly: " + this.name);
        isActive = true;
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
