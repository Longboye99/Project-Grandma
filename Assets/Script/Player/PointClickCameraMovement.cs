using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using Unity.Mathematics;
using System.Security.Cryptography;

public class PointClickCameraMovement : MonoBehaviour
{
    [Header("Camera Movement Setting")]
    public float camSmoothTime;
    public float camSpeed;
    public float turningOffset;
    public float movementCooldown = 0.6f;

    float currentRotationY;
    float targetRotationY = 0;

    public bool isTurning = false;
    bool enableMovement;
    public float lerpPercent;

    [SerializeField] GameObject flashLight;

    GameObject currentCam;
    public AreaNode currentNode;
    [SerializeField] AreaNode coffinNode;
    [SerializeField] Transform IncenseCam;

    SplineAnimate splineAnimate;
    SplineContainer currentSplineContainer;
    Animator cameraAnimator;
    HeadBop headBoper;
    [SerializeField] float transitionInTime;
    [SerializeField] float transitionOutTime;

    private void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();
        cameraAnimator = GetComponentInChildren<Animator>();
        headBoper = GetComponentInChildren<HeadBop>();
        currentCam = currentNode.CameraPos; //Set camera to default position
        
        SetCamPosition();

    }

    private void Update()
    {
        HandleKeyboardInput();
    }

    private void HandleKeyboardInput() //PLACE HOLDER#####################
    {
        if (Input.GetKeyDown(KeyCode.A) && !enableMovement) //When getting key input, set target for camera to turn to
        {
            if (currentNode.directionDict.ContainsKey(Direction.Left))
            {
                TurnCamera(currentNode.directionDict[Direction.Left]);
            }
        }
        if (Input.GetKeyDown(KeyCode.D) && !enableMovement)
        {
            if (currentNode.directionDict.ContainsKey(Direction.Right))
            {
                TurnCamera(currentNode.directionDict[Direction.Right]);
            }
        }
        if (Input.GetKeyDown(KeyCode.A) && !enableMovement)
        {
            if (currentNode.directionDict.ContainsKey(Direction.Forward))
            {
                TurnCamera(currentNode.directionDict[Direction.Forward]);
            }
        }
        if (Input.GetKeyDown(KeyCode.S) && !enableMovement)
        {
            if (currentNode.directionDict.ContainsKey(Direction.Backward))
            {
                TurnCamera(currentNode.directionDict[Direction.Backward]);
            }
        }
    }

    public void TurnCamera(AreaEnum nextArea)
    {
        if (!isTurning) //Don't turn if the player is already turning
        {
            isTurning = true; //Keep track of when player is turning
            //GameManager.instance.uiManager.TransitionOut();
            if (currentNode.pathDict[nextArea].direction == Direction.Left)
            {
                targetRotationY -= 90; //Set camera turn target to 90 degree to the left
            }
            else
            {
                targetRotationY += 90; //Set camera turn target to 90 degree to the right
            }
            
            currentSplineContainer = currentNode.pathDict[nextArea].splineContainer;
            splineAnimate.Duration = currentNode.pathDict[nextArea].duration;
            currentNode = currentNode.pathDict[nextArea].areaNode;
            

            StartCoroutine(TransitionIn());
        }
    }

    private IEnumerator TransitionIn()
    {
        cameraAnimator.SetTrigger("TurnDown");
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
            transform.position = Vector3.Lerp(currentPos, splinePosition, (elapsedTime/waitTime));
            transform.rotation = Quaternion.Slerp(currentRot, splineRot, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        PlaySplineAnimation();
    }

    private void PlaySplineAnimation()
    {
        splineAnimate.Container = currentSplineContainer;
        splineAnimate.Play();
        StartCoroutine(CheckSplineComplete());
    }

    private IEnumerator CheckSplineComplete()
    {
        while(splineAnimate.ElapsedTime < splineAnimate.Duration)
        {
            yield return null;
        }
        PlayTransitonOut();
        splineAnimate.ElapsedTime = 0;
    }

    private void PlayTransitonOut()
    {
        SwitchCamera();
        StartCoroutine(TransitionOut());
    }

    private IEnumerator TransitionOut()
    {
        cameraAnimator.SetTrigger("TurnUp");
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
        GameEventsManager.instance.playerEvents.MoveToArea(currentNode.area);
        yield return null;
    }


    private IEnumerator TurnCamera()
    {
        Quaternion currentPos = this.transform.rotation;
        Quaternion targetTransform = Quaternion.Euler(currentCam.transform.eulerAngles.x, targetRotationY, currentCam.transform.eulerAngles.z);
        float elapsedTime = 0;
        float waitTime = 0.7f;
        

        while (elapsedTime < waitTime)
        {
            transform.rotation = Quaternion.Slerp(currentPos, targetTransform, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
          // Yield here
            yield return null;
        }

        // Make sure we got there
        SwitchCamera();
        GameManager.instance.uiManager.TransitionIn();
        yield return null;
    }

    public void RespawnPlayer()
    {
        currentNode = coffinNode;
        currentCam = currentNode.CameraPos;
        SwitchCamera();
        SetCamPosition();
    }

    private void SwitchCamera() //Set target camera based on the index
    {
        currentCam = currentNode.CameraPos;
    }

    private void SetCamPosition() //Set the camera position and rotation to the target camera
    {
        this.transform.position = currentCam.transform.position;
        this.transform.rotation = currentCam.transform.rotation;
        flashLight.transform.rotation = currentCam.transform.rotation;
        currentRotationY = currentCam.transform.eulerAngles.y;
        targetRotationY = currentCam.transform.eulerAngles.y;

        GameManager.instance.anomalyManager.currentArea = currentNode.area;
        StartCoroutine(Movementcooldown());
    }

    private void IncenseCutscene()
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
        float waitTime = transitionOutTime;

        while (elapsedTime < waitTime)
        {
            transform.position = Vector3.Lerp(currentPos, targetPos, (elapsedTime / waitTime));
            transform.rotation = Quaternion.Slerp(currentRot, targetRot, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }

    private void DoIncenseSnap()
    {
        //animationStuff
    }

    private IEnumerator WaitForSeconds()
    {
        yield return new WaitForSeconds(2);
        MoveAway();
    }

    private void MoveAway()
    {
        StartCoroutine(MoveCamBack());  
    }

    private IEnumerator MoveCamBack()
    {
        cameraAnimator.SetTrigger("TurnUp");
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
    }

    private IEnumerator Movementcooldown()
    {
        yield return new WaitForSeconds(movementCooldown);
        isTurning = false;
    }
}
