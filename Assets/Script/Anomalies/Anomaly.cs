using Game.Database;
using TMPro;
using UnityEngine;

public abstract class Anomaly: MonoBehaviour
{
    [Header("Anomaly Config")]
    public string id;
    public AnomalyEnum anomalyLevel;
    public AreaEnum area;
    public int anomalyPoint;

    [Header("Anomaly State")]
    public bool isEnabled = false;
    public bool isActive;
    public int currentAnomalyPoint;

    [Header("Debug")]
    protected Material originalMaterial;
    protected Material currentMaterial;
    protected bool currentMeshActive;
    public Material highlightMaterial;
    protected bool isHighlighted = false;

    protected GameObject playerCam;

    public void Initialize(AnomalyData data)
    {
        anomalyPoint = data.AnomalyPoint;
        switch (data.Level)
        {
            case "Level 1":
                anomalyLevel = AnomalyEnum.LightAnomaly;
                break;
            case "Level 2":
                anomalyLevel = AnomalyEnum.HeavyAnomaly;
                break;
            default:
                break;
        }

        //Debug.Log("Initialized Anomaly: " + id);
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    public void SetAnomalyEnabled(string text)
    {
        if (text == "TRUE")
        {
            isEnabled = true;
        }
        else
        {
            isEnabled = false;
        }
    }

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += UndoAnomaly;
        GameEventsManager.instance.debugEvents.onPressHighlight += PressHighlight;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= UndoAnomaly;
        GameEventsManager.instance.debugEvents.onPressHighlight -= PressHighlight;
    }

    public abstract void TriggerAnomaly();

    public abstract void UndoAnomaly(Anomaly anomaly);

    public bool CheckPlayerIsLooking(bool checkisLooking) //Calculate if the player is looking at the node or not
    {
        bool isLooking;

        Vector3 dir = Vector3.Normalize(this.transform.position - playerCam.transform.position);
        float dot = Vector3.Dot(dir, playerCam.transform.forward);
        float dist = Vector3.Distance(transform.position, playerCam.transform.position);

        if (dot >= 0.5)
        {
            if (Physics.Raycast(playerCam.transform.position, transform.position - playerCam.transform.position, out RaycastHit hit, dist, (1 << 7)))
            {
                Debug.DrawLine(playerCam.transform.position, hit.point, Color.yellow);
                isLooking = true;
            }
            else
            {

                isLooking = false;
            }
        }
        else
        {
            Debug.DrawLine(playerCam.transform.position, transform.position, Color.red);

            isLooking = true;
        }

        if (checkisLooking)
        {
            return !isLooking;
        }
        else
        {
            return isLooking;
        }
    }


    //---------------------Debug functions ------------------------------

    public void ActivateLightAnomalies()
    {
        if (anomalyLevel == AnomalyEnum.LightAnomaly)
        {
            TriggerAnomaly();
        }
    }

    public void ActivateHeavyAnomalies()
    {
        if (anomalyLevel == AnomalyEnum.HeavyAnomaly)
        {
            TriggerAnomaly();
        }
    }

    public void ActivateAllAnomalies()
    {
        TriggerAnomaly();
    }

    public void PressHighlight()
    {
        if (!isHighlighted && isActive)
        {
            isHighlighted = true;
            currentMeshActive = GetComponent<MeshRenderer>().enabled;
            if (!currentMeshActive)
            {
                gameObject.GetComponent<MeshRenderer>().enabled = true;
            }

            currentMaterial = GetComponent<MeshRenderer>().material;
            gameObject.GetComponent<MeshRenderer>().material = highlightMaterial;
        }
        else if (isHighlighted)
        {
            isHighlighted = false;
            gameObject.GetComponent<MeshRenderer>().material = currentMaterial;
            gameObject.GetComponent<MeshRenderer>().enabled = currentMeshActive;
            Debug.Log(this.gameObject.name + "mesh state was : " + currentMeshActive);
        }
    }
}
