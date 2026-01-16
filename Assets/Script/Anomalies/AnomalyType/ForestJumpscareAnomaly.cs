using UnityEngine;

public class ForestJumpscareAnomaly : Anomaly
{
    [SerializeField] Animator ghostAnimator;
    [SerializeField] GameObject ghost;
    [SerializeField] FlashlightOverlay flashlightOverlay;

    [SerializeField] float MoveSpeed = 10;
    [SerializeField] float MinDist = 10f;
    bool isChasing;
    bool isDoingJumpscare;

    private void OnEnable()
    {
        playerCam = GameObject.FindGameObjectWithTag("Player");
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += UndoAnomaly;
        GameEventsManager.instance.debugEvents.onPressHighlight += PressHighlight;
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += FinishAnimationEvent;
        GameEventsManager.instance.inputEvents.onStartInteract += TriggerJumpscare;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= UndoAnomaly;
        GameEventsManager.instance.debugEvents.onPressHighlight -= PressHighlight;
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= FinishAnimationEvent;
    }

    private void Start()
    {
        ghost.SetActive(false);

        gameObject.GetComponent<MeshRenderer>().enabled = false;

        originalMaterial = GetComponent<MeshRenderer>().material; //Save default object material
    }

    public override void TriggerAnomaly()
    {
        isActive = true;
        currentAnomalyPoint = anomalyPoint;
        gameObject.GetComponent<Collider>().enabled = true;

        Debug.Log("Trigger Appearing Anomaly:" + this.name);

        ghost.SetActive(true);
    }

    public override void UndoAnomaly(Anomaly anomaly)
    {
        if (anomaly == this && isActive)
        {
            //TriggerJumpscare();
            
        }
    }

    public void TriggerJumpscare(InputEventContextEnum context)
    {
        if(GameManager.instance.playerManager.currentAnomaly == this)
        {
            currentAnomalyPoint = 0;
            isActive = false;
            isDoingJumpscare = true;
            ghostAnimator = ghost.GetComponentInChildren<Animator>();
            Invoke("BeginTurning", 0.3f);
        }
        
    }
    private void BeginTurning()
    {
        ghostAnimator.SetTrigger("Turning");
    }

    private void FinishAnimationEvent(string name)
    {
        if (name == "FinishTurning" && isDoingJumpscare)
        {
            Invoke("Running", 1);

        }
    }

    private void Running()
    {
        ghostAnimator.SetTrigger("Running");
        isChasing = true;
    }

    void FixedUpdate()
    {
        if (isDoingJumpscare && ghost != null)
        {
            Vector3 camDirection = playerCam.transform.position;
            camDirection.y = transform.position.y;
            ghost.transform.LookAt(camDirection);

            if (isChasing)
            {
                ChasePlayer();
            }
        }

    }

    private void ChasePlayer()
    {
        if (Vector3.Distance(ghost.transform.position, playerCam.transform.position) >= MinDist)
        {
            ghost.transform.position += ghost.transform.forward * MoveSpeed * Time.deltaTime;
        }
        else
        {
            if (isChasing && Vector3.Distance(ghost.transform.position, playerCam.transform.position) < MinDist)
            {
                flashlightOverlay.Blink(this);
                isChasing = false;

                Destroy(ghost);
                ghostAnimator = null;
                isDoingJumpscare = false;
                
                CurrentCooldown = cooldown;
            }
        }
    }


}
