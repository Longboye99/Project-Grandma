using UnityEngine;
using UnityEngine.UI;

public class ChaseJumpscare : Jumpscare
{
    [SerializeField] GameObject playerCam;
    [SerializeField] float MoveSpeed = 10;
    float MinDist = 2.5f;

    [SerializeField] Animator ghostAnimator;
    [SerializeField] Animator UiAnimator;

    [SerializeField] AudioClip screamAudio;
    [SerializeField] AudioClip noticeAudio;


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
        GameObject uiGameObject = GameObject.FindGameObjectWithTag("Jumpscare");
        uiGameObject.GetComponent<Image>().enabled = true;
        UiAnimator = uiGameObject.GetComponent<Animator>();
    }

    public override void EnableJumpscare()
    {
        
    }

    public override void DisableJumpscare()
    {
        
    }

    

    void Update()
    {
        Vector3 camDirection = playerCam.transform.position;
        camDirection.y = transform.position.y;
        transform.LookAt(camDirection);

        if (isChasing)
        {
            ChasePlayer();
        }

    }


    private void BeginTurning()
    {
        ghostAnimator.SetTrigger("Turning");
        GameManager.instance.sfxManager.PlaySoundFXClip(noticeAudio, this.transform, 0.3f);
    }

    private void FinishAnimationEvent(string name)
    {
        if (name == "FinishTurning")
        {
            ghostAnimator.SetTrigger("Running");
            isChasing = true;
            
        }
        else if (name == "FinishJumpscare")
        {
            
        }
    }

    private void ChasePlayer()
    {
        if (Vector3.Distance(transform.position, playerCam.transform.position) >= MinDist)
        {
            transform.position += transform.forward * MoveSpeed * Time.deltaTime;
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                UiAnimator.gameObject.SetActive(true);
                UiAnimator.SetTrigger("TriggerJumpscare");
                GameManager.instance.sfxManager.PlaySoundFXClip(screamAudio, this.transform, 1);
            }
        }
    }
}
