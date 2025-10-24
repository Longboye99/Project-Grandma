using UnityEngine;

public class DisplaceAnomaly : Anomaly
{
    [SerializeField] Transform newTransform;
    Vector3 originalPosition;
    Quaternion originalRotation;
    private void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("Player");
        originalMaterial = GetComponent<MeshRenderer>().material; //Save default object material
        originalPosition = this.gameObject.transform.position;
        originalRotation = this.gameObject.transform.rotation;
    }

    public override void TriggerAnomaly()
    {
        Debug.Log("Triggered Displace Anomaly: " + this.name);
        isActive = true;
        currentAnomalyPoint = anomalyPoint;

        gameObject.transform.position = newTransform.position;
        gameObject.transform.rotation = newTransform.rotation;
    }

    public override void UndoAnomaly(Anomaly anomaly)
    {
        if (anomaly == this && isActive)
        {
            Debug.Log("Undid Displace Anomaly: " + this.name, this.gameObject);
            this.gameObject.transform.position = originalPosition;
            this.gameObject.transform.rotation = originalRotation;

            currentAnomalyPoint = 0;
            isActive = false;
        }
    }
}
