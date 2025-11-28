using UnityEngine;

public class TurningAnomaly : Anomaly
{
    Vector3 camPosition;
    [SerializeField] int turningOffset;
    [SerializeField] AreaEnum playerArea;
    [SerializeField] bool isInArea;

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
            if (CheckPlayerIsLooking(false))
            {
                camPosition = playerCam.transform.position;
                camPosition.y = transform.position.y;
                transform.LookAt(camPosition);
                transform.eulerAngles = transform.eulerAngles + new Vector3(0, turningOffset, 0);
            }
            //turning
        }
    }

    public override void TriggerAnomaly()
    {
        isActive = true;
        currentAnomalyPoint = anomalyPoint;
        CurrentCooldown = cooldown;
        Debug.Log("Trigger Turning Anomaly: " + this.name);
    }

    public override void UndoAnomaly(Anomaly targetAnomaly)
    {
        if (targetAnomaly == this && isActive)
        {
            isActive = false;
            currentAnomalyPoint = 0;
            CurrentCooldown = 0;
        }
    }
}
