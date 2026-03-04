using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChaseJumpscare : Jumpscare
{
    [Header("Prefabs")]
    [SerializeField] GameObject ghostPrefab;
    [SerializeField] GameObject jumpscarePrefab;

    GameObject playerCam;
    [SerializeField] FlashlightOverlay blinkOverlay;
    [SerializeField] float MoveSpeed = 10;
    float MinDist = 2.5f;

    [SerializeField] Animator ghostAnimator;
    [SerializeField] Animator jumpscareAnimator;

    Transform jumpscareCanvas;

    [SerializeField] AudioClip screamAudio;
    [SerializeField] AudioClip noticeAudio;

    GameObject ghost;
    GameObject jumpscare;
    bool isActive;

    bool isChasing = false;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += FinishAnimationEvent;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= FinishAnimationEvent;
    }
    private void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("Player");
        jumpscareCanvas = GameObject.FindGameObjectWithTag("Jumpscare").transform;
    }

    public override void TriggerJumpscare()
    {
        blinkOverlay.Blink(null);
        ghost = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
        ghostAnimator = ghost.GetComponentInChildren<Animator>();
        isActive = true;
        Invoke("BeginTurning", 2);
    }
    private void BeginTurning()
    {
        ghostAnimator.SetTrigger("Turning");
        GameManager.instance.sfxManager.PlaySoundFXClip(noticeAudio, this.transform, 0.3f);
    }

    public override void DisableJumpscare()
    {
        GameEventsManager.instance.anomalyEvents.FinishJumpscare();
        isActive = false;
        Invoke("DestroyGhost", 2);
    }

    void Update()
    {
        if (isActive && ghost != null)
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

    private void FinishAnimationEvent(string name)
    {
        if (name == "FinishTurning" && isActive)
        {
            Invoke("Run", 1.5f);
            
        }
        else if (name == "FinishJumpscare" && isActive)
        {
            Invoke("DisableJumpscare", 3f);
        }
    }

    private void Run()
    {
        GameEventsManager.instance.anomalyEvents.FinishAnimationEvent("ShakeScreen");
        ghostAnimator.SetTrigger("Running");
        isChasing = true;
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
                isChasing = false;
                Destroy(ghost);
                ghostAnimator = null;

                jumpscare = Instantiate(jumpscarePrefab, jumpscareCanvas);
                jumpscareAnimator = jumpscare.GetComponent<Animator>();
                jumpscareAnimator.SetTrigger("TriggerJumpscare");
                GameManager.instance.uiManager.EnableGameOverlay(false);
                GameManager.instance.sfxManager.PlaySoundFXClip(screamAudio, this.transform, 0.6f);
            }
        }
    }

    private void DestroyGhost()
    {
        Destroy(jumpscare);
        jumpscareAnimator = null;
    }

}
