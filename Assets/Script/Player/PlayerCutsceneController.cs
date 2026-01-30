using UnityEngine;
using System.Collections;

public class PlayerCutsceneController : MonoBehaviour
{
    [SerializeField] Transform IncenseCam;
    [SerializeField] float transitionInTime;
    [SerializeField] float transitionOutTime;
    [SerializeField] GameObject ghostHand;

    PointClickCameraMovement pointClickCameraMovement;
    PointClickCameraController pointClickCameraController;

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
        pointClickCameraController = GetComponent<PointClickCameraController>();
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
    public void TeleportToIncense()
    {
        transform.position = IncenseCam.position;
        transform.rotation = IncenseCam.rotation;
    }

    public void IncenseCutsceneSequence()
    {
        pointClickCameraController.EnableFlashlight(false);
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
    public void MoveAwayFromIncenseTrigger(float sec)
    {
        StartCoroutine(MoveAwayFromIncense(sec));
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
        pointClickCameraController.EnableFlashlight(true);
        GameManager.instance.levelManager.FinishRespawnCutscene(); 
    }

    private IEnumerator MoveAwayFromIncense(float sec)
    {
        Vector3 targetPos = pointClickCameraMovement.currentNode.CameraPos.transform.position;
        Quaternion targetRot = pointClickCameraMovement.currentNode.CameraPos.transform.rotation;

        Vector3 currentPos = transform.position;
        Quaternion currentRot = this.transform.rotation;

        float elapsedTime = 0;
        float waitTime = sec;

        while (elapsedTime < waitTime)
        {
            transform.position = Vector3.Lerp(currentPos, targetPos, (elapsedTime / waitTime));
            transform.rotation = Quaternion.Slerp(currentRot, targetRot, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        pointClickCameraMovement.SetCamPosition();
        pointClickCameraMovement.isTurning = false;
        pointClickCameraController.EnableFlashlight(true);
        GameManager.instance.levelManager.FinishRespawnCutscene();
    }
}
