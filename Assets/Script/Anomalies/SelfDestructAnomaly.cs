using UnityEngine;

public class SelfDestructAnomaly : Anomaly
{
    [SerializeField] private GameObject idleGhost;
    [SerializeField] private float despawnDelay;
    [SerializeField] bool isInArea;
    [SerializeField] AreaEnum playerArea;

    private bool isInTransition;

    private void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("Player");
        playerArea = GameManager.instance.anomalyManager.currentArea;

    }

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
        idleGhost.SetActive(true);
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
        }
    }
}