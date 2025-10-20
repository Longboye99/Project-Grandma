using UnityEngine;

public class LookAnomaly : Anomaly
{
    [Header("Config")]
    [SerializeField] Anomaly anomaly;
    [SerializeField] float activationTime;
    [SerializeField] float lookScore;
    [SerializeField] float maxScore;

    GameObject playerCam;

    private void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (!isActive)
        {
            if (CheckPlayerIsLooking(true))
            {
                lookScore += Time.deltaTime;
            }
            else if (lookScore > maxScore)
            {
                TriggerAnomaly();
            }
            
        }
    }

    public override void TriggerAnomaly()
    {
        anomaly.TriggerAnomaly();
        isActive = true;
    }

    public override void UndoAnomaly(Anomaly targetAnomaly)
    {
        if(targetAnomaly == anomaly && isActive)
        {
            isActive = false;
            lookScore = 0;
        }
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
