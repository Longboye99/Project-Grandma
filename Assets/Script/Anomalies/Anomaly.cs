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
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= UndoAnomaly;
    }

    public abstract void TriggerAnomaly();

    public abstract void UndoAnomaly(Anomaly anomaly);

    
    

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
}
