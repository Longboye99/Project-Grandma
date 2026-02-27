using UnityEngine;

public class AppearingAnomaly : Anomaly
{
    [SerializeField]MeshGroup meshGroup;
    [SerializeField] bool useMeshGroup = false;
    private void Start()
    {
        if (GetComponent<MeshRenderer>())
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else if(GetComponent<MeshGroup>())
        {
            Debug.Log("Found MeshGroup", this);
            useMeshGroup = true;
            meshGroup = GetComponent<MeshGroup>();
            meshGroup.DisableAllMesh();  
        }
        else
        {
            Debug.LogWarning("Can't find mesh for appearing anomaly: ", this);
        }

        gameObject.GetComponent<Collider>().enabled = false;
        if(GetComponent<MeshRenderer>())
        {
            originalMaterial = GetComponent<MeshRenderer>().material; //Save default object material

        }
    }

    public override void TriggerAnomaly()
    {
        isActive = true;
        currentAnomalyPoint = anomalyPoint;
        Debug.Log("Trigger Appearing Anomaly:" + this.name);

        if (useMeshGroup)
        {
            meshGroup.EnableAllMesh();
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true; //Make the object appear
        }
        gameObject.GetComponent<Collider>().enabled = true; //Make the object appear
    }

    public override void UndoAnomaly(Anomaly anomaly)
    {
        if (anomaly == this && isActive)
        {
            if (useMeshGroup)
            {
                meshGroup.DisableAllMesh();
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().enabled = false; //Make anomaly dissapear
            }
            
            gameObject.GetComponent<Collider>().enabled = false;

            currentAnomalyPoint = 0;
            isActive = false;
            CurrentCooldown = cooldown;
        }
    }
}
