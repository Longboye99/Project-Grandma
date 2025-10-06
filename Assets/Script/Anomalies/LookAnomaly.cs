using UnityEngine;

public class LookAnomaly : Anomaly
{
    [Header("Config")]
    [SerializeField] Anomaly anomaly;
    [SerializeField] float activationTime;
    [SerializeField] float lookScore;
    [SerializeField] float maxScore;

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
}
