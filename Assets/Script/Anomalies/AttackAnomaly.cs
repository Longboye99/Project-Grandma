using UnityEngine;

public class AttackAnomaly : Anomaly
{
    [SerializeField] private GameObject idleGhost;
    [SerializeField] private float despawnDelay;

    private GameObject spawnedGhost;
    private bool isInTransition;
    GameObject playerCam;

    private void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("Player");

    }

    private void FixedUpdate()
    {
        if (isActive)
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
}
