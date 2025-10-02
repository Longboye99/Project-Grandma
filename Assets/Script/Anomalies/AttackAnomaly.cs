using UnityEngine;

public class AttackAnomaly : Anomaly
{
    [SerializeField] private GameObject idleGhost;
    [SerializeField] private float despawnDelay;

    private GameObject spawnedGhost;
    private bool isInTransition;

    private void Start()
    {

    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            if (!CheckPlayerIsNotLooking())
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
        Debug.Log("Triggered Attack Anomaly: " + this.name);

        GameEventsManager.instance.anomalyEvents.TriggerAttackAnomaly();
        isActive = true;
        spawnedGhost = Instantiate(idleGhost, this.transform.position, Quaternion.identity);
        
    }

    private void DespawnGhost()
    {
        UndoAnomaly(this);
    }

    public override void UndoAnomaly(Anomaly anomaly)
    {
        if (anomaly == this && isActive)
        {
            isActive = false;
            isInTransition = false;
            Destroy(spawnedGhost);
            GameEventsManager.instance.anomalyEvents.UndoAnomaly(this);
        }
    }

    public float CheckPlayerDist()
    {
        return Vector3.Distance(this.transform.position, playerCam.transform.position);
    }
}
