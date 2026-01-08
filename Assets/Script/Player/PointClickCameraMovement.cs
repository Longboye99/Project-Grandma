using UnityEngine;
using UnityEngine.Splines;

public class PointClickCameraMovement : MonoBehaviour
{
    [Header("Camera Movement Setting")]
    public float camSmoothTime;
    public float camSpeed;
    public float turningOffset;

    float currentRotationY;
    float targetRotationY = 0;

    bool isTurning = false;
    bool disableSideTurning;

    [SerializeField] GameObject flashLight;

    GameObject currentCam;
    [SerializeField] AreaNode currentNode;

    private void Start()
    {
        currentCam = currentNode.CameraPos; //Set camera to default position
        SetCamPosition();

    }

    private void Update()
    {
        HandleKeyboardInput();

        if (isTurning)
        {
            HandleCameraMovement(); //Do camera movement stuff
        }

    }

    private void HandleKeyboardInput() //PLACE HOLDER#####################
    {
        if (Input.GetKeyDown(KeyCode.A) && !disableSideTurning) //When getting key input, set target for camera to turn to
        {
            if (currentNode.directionDict.ContainsKey(Direction.Left))
            {
                TurnCamera(currentNode.directionDict[Direction.Left]);
            }
        }
        if (Input.GetKeyDown(KeyCode.D) && !disableSideTurning)
        {
            if (currentNode.directionDict.ContainsKey(Direction.Right))
            {
                TurnCamera(currentNode.directionDict[Direction.Right]);
            }
        }
        if (Input.GetKeyDown(KeyCode.A) && !disableSideTurning)
        {
            if (currentNode.directionDict.ContainsKey(Direction.Forward))
            {
                TurnCamera(currentNode.directionDict[Direction.Forward]);
            }
        }
        if (Input.GetKeyDown(KeyCode.S) && !disableSideTurning)
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
            GameManager.instance.uiManager.TransitionOut();
            if (currentNode.pathDict[nextArea].direction == Direction.Left)
            {
                targetRotationY -= 90; //Set camera turn target to 90 degree to the left
            }
            else
            {
                targetRotationY += 90; //Set camera turn target to 90 degree to the right
            }
            
            currentNode = currentNode.pathDict[nextArea].areaNode;
            isTurning = true; //Keep track of when player is turning
        }
    }

    private void HandleCameraMovement()
    {
        currentRotationY = Mathf.SmoothDamp(currentRotationY, targetRotationY, ref camSpeed, camSmoothTime); //Smooth camera turn (janky camera turn target rn FIX LATER)
        this.transform.rotation = Quaternion.Euler(currentCam.transform.eulerAngles.x, currentRotationY, currentCam.transform.eulerAngles.z);

        if (currentRotationY >= targetRotationY - 5 * turningOffset && currentRotationY <= targetRotationY + 5 * turningOffset && isTurning) //Teleport camera to target position when it has almost turned toward the target
        {
            SwitchCamera();
            GameManager.instance.uiManager.TransitionIn();
        }

        if (currentRotationY >= targetRotationY - turningOffset && currentRotationY <= targetRotationY + turningOffset && isTurning) //Enable turning again once player has turned toward the target
        {
            isTurning = false;
        }
    }

    private void SwitchCamera() //Set target camera based on the index
    {
        currentCam = currentNode.CameraPos;
        SetCamPosition();
    }

    private void SetCamPosition() //Set the camera position and rotation to the target camera
    {
        this.transform.position = currentCam.transform.position;
        this.transform.rotation = currentCam.transform.rotation;
        flashLight.transform.rotation = currentCam.transform.rotation;
        currentRotationY = currentCam.transform.eulerAngles.y;
        targetRotationY = currentCam.transform.eulerAngles.y;
    }
}
