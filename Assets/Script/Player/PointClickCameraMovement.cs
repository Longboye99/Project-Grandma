using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using Unity.Mathematics;
using UnityEngine.UI;
using System.Security.Cryptography;

public class PointClickCameraMovement : MonoBehaviour
{
    [Header("Camera Movement Setting")]
    public float camSmoothTime;
    public float camSpeed;
    public float turningOffset;
    public float movementCooldown = 0.6f;
    public bool turnDownCam;

    public bool isTurning = false;
    public bool isWalking = false;
    [SerializeField] bool enableMovement = true;

    [SerializeField] GameObject flashLight;
    [SerializeField] GameObject buttonCanvas;

    [SerializeField] GameObject leftButton;
    [SerializeField] GameObject rightButton;
    [SerializeField] GameObject forwardButton;
    [SerializeField] GameObject backwardButton;

    GameObject currentCam;
    public AreaNode currentNode;
    [SerializeField] AreaNode startingNode;

    SplineAnimate splineAnimate;
    SplineContainer currentSplineContainer;
    Animator cameraAnimator;
    HeadBop headBoper;
    [SerializeField] float transitionInTime;
    [SerializeField] float transitionOutTime;

    private void OnEnable()
    {
        GameEventsManager.instance.playerEvents.onEnableMovement += EnableMovement;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.playerEvents.onEnableMovement -= EnableMovement;
    }

    private void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();
        cameraAnimator = GetComponentInChildren<Animator>();
        headBoper = GetComponentInChildren<HeadBop>();

        currentNode = startingNode;
        currentCam = currentNode.CameraPos; //Set camera to default position
        GameManager.instance.anomalyManager.currentArea = currentNode.area;
        SwitchButtonDirection();

    }

    private void Update()
    {
        HandleKeyboardInput();
    }

    private void HandleKeyboardInput() //PLACE HOLDER#####################
    {
        if (Input.GetKeyDown(KeyCode.A)) //When getting key input, set target for camera to turn to
        {
            TurnLeft();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            TurnRight();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            TurnForward();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            TurnBackward();
        }
    }

    public void TurnRight()
    {
        if (currentNode.directionDict.ContainsKey(Direction.Right) && enableMovement)
        {
            TurnCamera(currentNode.directionDict[Direction.Right]);
        }
    }

    public void TurnLeft()
    {
        if (currentNode.directionDict.ContainsKey(Direction.Left) && enableMovement)
        {
            TurnCamera(currentNode.directionDict[Direction.Left]);
        }
    }

    public void TurnForward()
    {
        if (currentNode.directionDict.ContainsKey(Direction.Forward) && !enableMovement)
        {
            TurnCamera(currentNode.directionDict[Direction.Forward]);
        }
    }

    public void TurnBackward()
    {
        if (currentNode.directionDict.ContainsKey(Direction.Backward) && enableMovement)
        {
            TurnCamera(currentNode.directionDict[Direction.Backward]);
        }
    }

    private void EnableMovement(bool enable)
    {
        enableMovement = enable;
        if (enableMovement == true)
        {
            buttonCanvas.SetActive(true);

        }
        else
        {
            buttonCanvas.SetActive(false);
        }
    }

    public void TurnCamera(AreaEnum nextArea)
    {
        if (!isTurning) //Don't turn if the player is already turning
        {
            buttonCanvas.SetActive(false);
            isTurning = true; //Keep track of when player is turning
            GameManager.instance.playerManager.EnableInteract(false);
            GameEventsManager.instance.playerEvents.Startturning();

            currentSplineContainer = currentNode.pathDict[nextArea].splineContainer;
            splineAnimate.Duration = currentNode.pathDict[nextArea].duration;
            currentNode = currentNode.pathDict[nextArea].areaNode;


            StartCoroutine(TransitionIn());
        }
    }

    private IEnumerator TransitionIn()
    {
        if (turnDownCam)
        {
            cameraAnimator.SetTrigger("TurnDown");

        }
        
        GameManager.instance.uiManager.FadeOut();
        GameManager.instance.uiManager.HandShakeStart();
        headBoper.isBobbing = true;

        Spline spline = currentSplineContainer.Spline;
        float3 pos = spline.ToArray()[0].Position;

        Vector3 splinePosition = currentSplineContainer.transform.TransformPoint(pos);
        Quaternion splineRot = spline.ToArray()[0].Rotation;

        Vector3 currentPos = transform.position;
        Quaternion currentRot = this.transform.rotation;

        float elapsedTime = 0;
        float waitTime = transitionInTime;

        while (elapsedTime < waitTime)
        {
            transform.position = Vector3.Lerp(currentPos, splinePosition, (elapsedTime / waitTime));
            transform.rotation = Quaternion.Slerp(currentRot, splineRot, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        PlaySplineAnimation();
    }

    private void PlaySplineAnimation()
    {
        splineAnimate.Container = currentSplineContainer;
        splineAnimate.ElapsedTime = 0;

        splineAnimate.Play();
        isWalking = true;
        StartCoroutine(CheckSplineComplete());
    }

    private IEnumerator CheckSplineComplete()
    {
        while (splineAnimate.ElapsedTime < splineAnimate.Duration)
        {
            yield return null;
        }
        PlayTransitonOut();
        
    }

    private void PlayTransitonOut()
    {
        isWalking = false;
        SwitchCamera();
        StartCoroutine(TransitionOut());
    }

    private IEnumerator TransitionOut()
    {
        if (turnDownCam)
        {
            cameraAnimator.SetTrigger("TurnUp");

        }
        GameManager.instance.uiManager.FadeIn();
        GameManager.instance.uiManager.HandShakeEnd();
        headBoper.isBobbing = false;

        Vector3 targetPos = currentCam.transform.position;
        Quaternion targetRot = currentCam.transform.rotation;

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

        SetCamPosition();
        StartCoroutine(Movementcooldown());
        yield return null;
    }

    public void RespawnPlayer()
    {
        currentNode = startingNode;
        currentCam = currentNode.CameraPos;
        SwitchCamera();
        SetCamPosition();
    }

    private void SwitchCamera() //Set target camera based on the index
    {
        currentCam = currentNode.CameraPos;
    }

    public void SetCamPosition() //Set the camera position and rotation to the target camera
    {
        this.transform.position = currentCam.transform.position;
        this.transform.rotation = currentCam.transform.rotation;
        flashLight.transform.rotation = currentCam.transform.rotation;

        GameManager.instance.anomalyManager.currentArea = currentNode.area;
        GameEventsManager.instance.playerEvents.TransitionToArea(currentNode.area);
    }

    public IEnumerator Movementcooldown()
    {
        yield return new WaitForSeconds(movementCooldown);
        isTurning = false;
        GameManager.instance.playerManager.EnableInteract(true);

        buttonCanvas.SetActive(true);
        SwitchButtonDirection();
        GameEventsManager.instance.playerEvents.MoveToArea(currentNode.area);
    }

    private void SwitchButtonDirection()
    {
        rightButton.SetActive(false);
        leftButton.SetActive(false);
        backwardButton.SetActive(false);

        if (currentNode.directionDict.ContainsKey(Direction.Left))
        {
            leftButton.SetActive(true);
        }
        if (currentNode.directionDict.ContainsKey(Direction.Right))
        {
            rightButton.SetActive(true);
        }
        if (currentNode.directionDict.ContainsKey(Direction.Backward))
        {
            backwardButton.SetActive(true);
        }
    }
}
