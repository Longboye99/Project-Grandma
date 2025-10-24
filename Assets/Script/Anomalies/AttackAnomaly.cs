using UnityEngine;

public class AttackAnomaly : Anomaly
{
    [SerializeField] private GameObject idleGhost;
    [SerializeField] private float despawnDelay;

    private ChaseJumpscareHandler spawnedGhost;
    private bool isInTransition;

    private void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("Player");

    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            if (CheckPlayerIsLooking(true) && GameManager.instance.anomalyManager.currentArea == AreaEnum.Coffin)
            {
                if (!isInTransition)
                {
                    isInTransition = true;
                    spawnedGhost = Instantiate(idleGhost, this.transform.position, Quaternion.identity).GetComponent<ChaseJumpscareHandler>();
                    spawnedGhost.StartJumpscare(3);
                }
            }
        }
        
    }

    public override void TriggerAnomaly()
    {
        Debug.Log("Triggered Attack Anomaly: " + this.name);

        isActive = true;
        
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
