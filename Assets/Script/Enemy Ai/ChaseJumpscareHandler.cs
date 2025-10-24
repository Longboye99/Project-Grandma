using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChaseJumpscareHandler : MonoBehaviour
{
    [SerializeField] GameObject playerCam;
    [SerializeField] float MoveSpeed = 10;
    float MinDist = 3;

    [SerializeField] Animator ghostAnimator;
    [SerializeField] Animator UiAnimator;

    [SerializeField] AudioClip screamAudio;
    [SerializeField] AudioClip noticeAudio;


    bool isChasing = false;
    bool jumpscare = false;
    bool isInTransition = false;

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

        //Invoke("StartJumpscare", 5);

    }

    void Update()
    {
        Vector3 camPosition = playerCam.transform.position;
        camPosition.y = transform.position.y;
        transform.LookAt(camPosition);

        if (isChasing)
        {
            ChasePlayer();
        }

        if (jumpscare)
        {
            if (!CheckPlayerIsNotLooking())
            {
                if (!isInTransition)
                {
                    isInTransition = true;
                    Invoke("BeginTurning", 0.1f);
                }
            }
        }

    }

    public void StartJumpscare(int sec)
    {
        Invoke("DoJumpscare", sec);
        GameManager.instance.sfxManager.PlaySoundFXClip(noticeAudio, this.transform, 0.8f);
    }

    private void DoJumpscare()
    {
        jumpscare = true;
    }

    private void BeginTurning()
    {
        ghostAnimator.SetTrigger("Turning");
    }

    private void FinishAnimationEvent(string name)
    {
        if(name == "FinishTurning")
        {
            ghostAnimator.SetTrigger("Running");
            isChasing = true;
            GameManager.instance.sfxManager.PlaySoundFXClip(screamAudio, this.transform, 1);
        }
        else if(name == "FinishJumpscare")
        {
            GameManager.instance.levelManager.ProgessLoop();

            Invoke("ExitJumpscare", 1.5f);
        }
    }

    private void ExitJumpscare()
    {
        UiAnimator.gameObject.SetActive(false);
        UiAnimator.SetTrigger("ExitJumpscare");
        Destroy(this.gameObject);
    }

    private void ChasePlayer()
    {
        if (Vector3.Distance(transform.position, playerCam.transform.position) >= MinDist)
        {
            transform.position += transform.forward * MoveSpeed * Time.deltaTime;
        }
        else
        {
            if(isChasing)
            {
                isChasing = false;
                UiAnimator.gameObject.SetActive(true);
                UiAnimator.SetTrigger("TriggerJumpscare");
            }
        }
    }

    public bool CheckPlayerIsNotLooking() //Calculate if the player is looking at the node or not
    {
        Vector3 dir = Vector3.Normalize(this.transform.position - playerCam.transform.position);
        float dot = Vector3.Dot(dir, playerCam.transform.forward);
        float dist = Vector3.Distance(transform.position, playerCam.transform.position);

        if (dot >= 0.5)
        {
            /*if (Physics.Raycast(playerCam.transform.position, transform.position - playerCam.transform.position, out RaycastHit hit, dist, (1 << 7)))
            {
                Debug.DrawLine(playerCam.transform.position, hit.point, Color.yellow);
                return true;
            }
            else
            {

                return false;
            }*/
            return false;
        }
        else
        {
            Debug.DrawLine(playerCam.transform.position, transform.position, Color.red);

            return true;
        }
    }
}