using UnityEngine;
using System.Collections;

public class PlayerCutsceneController : MonoBehaviour
{
    [SerializeField] Transform IncenseCam;
    [SerializeField] float transitionInTime;
    [SerializeField] float transitionOutTime;
    [SerializeField] GameObject ghostHand;
    Animator ghostHandnimator;

    PointClickCameraMovement pointClickCameraMovement;

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
        pointClickCameraMovement = GetComponent<PointClickCameraMovement>();
    }

    private void FinishAnimationEvent(string eventName)
    {
        if(eventName == "SnapIncense")
        {
            GameEventsManager.instance.anomalyEvents.SnapIncense();
        }
        if(eventName == "FinishIncenseCutscene")
        {
            ghostHand.SetActive(false);
            StartCoroutine(MoveAwayFromIncense());
        }
    }

    public void IncenseCutsceneSequence()
    {
        StartCoroutine(MoveToIncense());
    }

    private IEnumerator MoveToIncense()
    {
        Vector3 targetPos = IncenseCam.position;
        Quaternion targetRot = IncenseCam.rotation;

        Vector3 currentPos = transform.position;
        Quaternion currentRot = this.transform.rotation;

        float elapsedTime = 0;
        float waitTime = transitionInTime;

        while (elapsedTime < waitTime)
        {
            transform.position = Vector3.Lerp(currentPos, targetPos, (elapsedTime / waitTime));
            transform.rotation = Quaternion.Slerp(currentRot, targetRot, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        ghostHand.SetActive(true);

    }

    private IEnumerator MoveAwayFromIncense()
    {
        Vector3 targetPos = pointClickCameraMovement.currentNode.CameraPos.transform.position;
        Quaternion targetRot = pointClickCameraMovement.currentNode.CameraPos.transform.rotation;

        Vector3 currentPos = transform.position;
        Quaternion currentRot = this.transform.rotation;

        float elapsedTime = 0;
        float waitTime = transitionOutTime;

        while (elapsedTime < waitTime)
        {
            transform.position = Vector3.Lerp(currentPos, targetPos, (elapsedTime / waitTime));
            transform.rotation = Quaternion.Slerp(currentRot, targetRot, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        pointClickCameraMovement.SetCamPosition();
        pointClickCameraMovement.isTurning = false;
        GameManager.instance.levelManager.FinishRespawnCutscene();
       
    }
}
