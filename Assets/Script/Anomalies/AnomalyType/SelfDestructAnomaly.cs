using UnityEngine;

public class SelfDestructAnomaly : Anomaly
{
    [SerializeField] private GameObject idleGhost;
    [SerializeField] private float despawnDelay;
    [SerializeField] bool isInArea;
    [SerializeField] AreaEnum playerArea;

    private bool isInTransition;

    private void FixedUpdate()
    {
        playerArea = GameManager.instance.anomalyManager.currentArea;
        if (playerArea == area)
        {
            isInArea = true;
        }
        else
        {
            isInArea = false;
        }

        if (isActive && isInArea)
        {
            if (CheckPlayerIsLooking(true))
            {
                if (!isInTransition)
                {
                    isInTransition = true;
                    Invoke("DespawnGhost", despawnDelay);
                }
            }
        }

    }

    public override void TriggerAnomaly()
    {
        isActive = true;
        currentAnomalyPoint = anomalyPoint;
        idleGhost.SetActive(true);
        Debug.Log("Trigger Self Destruct Anomaly: " + this.name);
    }

    private void DespawnGhost()
    {
        GameEventsManager.instance.anomalyEvents.UndoAnomaly(this);
    }

    public override void UndoAnomaly(Anomaly anomaly)
    {
        if (anomaly == this && isActive)
        {
            isActive = false;
            isInTransition = false;      
            idleGhost.SetActive(false);
            CurrentCooldown = cooldown;
            currentAnomalyPoint = 0;
        }
    }
}