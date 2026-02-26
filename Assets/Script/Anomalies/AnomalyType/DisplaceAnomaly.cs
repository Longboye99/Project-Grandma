using UnityEngine;

public class DisplaceAnomaly : Anomaly
{
    [SerializeField] Transform newTransform;
    Vector3 originalPosition;
    Quaternion originalRotation;
    private void Start()
    { 
        originalPosition = this.gameObject.transform.position;
        originalRotation = this.gameObject.transform.rotation;
        originalMaterial = GetComponent<MeshRenderer>().material; //Save default object material
    }

    public override void TriggerAnomaly()
    {
        isActive = true;
        currentAnomalyPoint = anomalyPoint;
        Debug.Log("Trigger Displace Anomaly: " + this.name);

        gameObject.transform.position = newTransform.position;
        gameObject.transform.rotation = newTransform.rotation;
    }

    public override void UndoAnomaly(Anomaly anomaly)
    {
        if (anomaly == this && isActive)
        {
            this.gameObject.transform.position = originalPosition;
            this.gameObject.transform.rotation = originalRotation;

            currentAnomalyPoint = 0;
            isActive = false;
            CurrentCooldown = cooldown;
        }
    }
}
