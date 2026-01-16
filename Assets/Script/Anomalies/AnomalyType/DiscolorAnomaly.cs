using UnityEngine;

public class DiscolorAnomaly : Anomaly
{
    public Material discolorMaterial;

    private void Start()
    {
        originalMaterial = GetComponent<MeshRenderer>().material; //Save default object material
    }

    public override void TriggerAnomaly()
    {
        isActive = true;
        currentAnomalyPoint = anomalyPoint;
        Debug.Log("Trigger Discolor Anomaly: " + this.name);

        gameObject.GetComponent<MeshRenderer>().material = discolorMaterial; //Make the object appear
    }

    public override void UndoAnomaly(Anomaly anomaly)
    {
        if(anomaly == this && isActive)
        {
            gameObject.GetComponent<MeshRenderer>().material = originalMaterial; //Make anomaly dissapear
            currentAnomalyPoint = 0;
            isActive = false;
            CurrentCooldown = cooldown;
        }  
    }
}
