using UnityEngine;

public class DiscolorAnomaly : Anomaly
{
    public Material discolorMaterial;

    private void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("Player");
        originalMaterial = GetComponent<MeshRenderer>().material; //Save default object material
    }

    public override void TriggerAnomaly()
    {
        Debug.Log("Triggered Discolor Anomaly: " + this.name);
        isActive = true;
        currentAnomalyPoint = anomalyPoint;

        gameObject.GetComponent<MeshRenderer>().material = discolorMaterial; //Make the object appear
    }

    public override void UndoAnomaly(Anomaly anomaly)
    {
        if(anomaly == this && isActive)
        {
            gameObject.GetComponent<MeshRenderer>().material = originalMaterial; //Make anomaly dissapear
            currentAnomalyPoint = 0;
            isActive = false;
        }  
    }
}
