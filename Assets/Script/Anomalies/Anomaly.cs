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
    public int cooldown = 10;

    [Header("Anomaly State")]
    public bool isEnabled = false;
    public bool isActive;
    public int currentAnomalyPoint;
    public float CurrentCooldown;
    
    protected GameObject playerCam;

    public void Initialize(AnomalyData data)
    {
        anomalyPoint = data.AnomalyPoint;
        cooldown = data.Cooldown;
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

        Debug.Log("Initialized Anomaly: " + id);
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
        playerCam = GameObject.FindGameObjectWithTag("Player");
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += UndoAnomaly;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= UndoAnomaly;
    }

    protected void Update()
    {
        if (CurrentCooldown > 0 && isEnabled) //Counting down the cooldown
        {
            CurrentCooldown -= Time.deltaTime;
        }
    }

    public bool SpawnAnomaly()
    {
        if (CurrentCooldown <= 0 && isEnabled && CheckPlayerIsLooking(false)) //Check if not in cooldown and not already actived
        {
            TriggerAnomaly();
            return true;
        }
        else
        {
            return false;
        }
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
                //Debug.DrawLine(playerCam.transform.position, hit.point, Color.yellow);
                isLooking = true;
            }
            else
            {
                
                isLooking = false;
            }
        }
        else
        {
            //Debug.DrawLine(playerCam.transform.position, transform.position, Color.red);

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
        if (anomalyLevel == AnomalyEnum.HeavyAnomaly)
        {
            SpawnAnomaly();
        }
    }

    public void ActivateHeavyAnomalies()
    {
        if (anomalyLevel == AnomalyEnum.HeavyAnomaly)
        {
            SpawnAnomaly();
        }
    }

    public void ActivateAllAnomalies()
    {
        SpawnAnomaly();
    }
}
