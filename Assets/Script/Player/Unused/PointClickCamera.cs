using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointClickCamera : MonoBehaviour
{
    [Header("Camera Movement Setting")]
    public GameObject playerCameraObject;
    private Camera playerCamera;
    public float camSmoothTime;
    public float camSpeed;
    public float turningOffset;
    public float sensX;
    public float sensY;

    float currentRotationY;
    float targetRotationY = 0;

    float xRotation;
    float yRotation;

    bool isTurning = false;
    bool disableSideTurning;

    [SerializeField] GameObject flashLight;

    [Header("Cameras")]
    private GameObject currentCam;
    [SerializeField] GameObject kitchenCam;
    [SerializeField] GameObject coffinCam;
    [SerializeField] GameObject laundryCam;
    [SerializeField] GameObject bathroomCam;
    [SerializeField] GameObject ceilingCam;
    private int cameraIndex;


    private void Start()
    {
        playerCamera = playerCameraObject.GetComponent<Camera>();
        currentCam = kitchenCam; //Set camera to default position
        cameraIndex = 1;
        SetCamPosition();

    }


    private void Update()
    {
        HandleKeyboardInput();
        FlashLightMovement();

        if (isTurning)
        {
            HandleCameraMovement(); //Do camera movement stuff
        }
        /*else if(!disableSideTurning)
        {
            MouseCameraMovement();
        }*/
        
    }

    private void MouseCameraMovement()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation = currentRotationY + mouseX;
        xRotation = mouseY;

        

        this.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }
    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A) && !disableSideTurning) //When getting key input, set target for camera to turn to
        {
            TurnCameraLeft();
        }
        if (Input.GetKeyDown(KeyCode.D) && !disableSideTurning)
        {
            TurnCameraRight();
        }
        /*if (Input.GetKeyDown(KeyCode.W)) //When getting key input, set target for camera to turn to
        {
            TurnCameraUp();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            TurnCameraDown();
        }*/
    }

    private void FlashLightMovement()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition/3);
        RaycastHit hit;
        Vector3 dir;
        if(Physics.Raycast(ray, out hit))
        {
            dir = (hit.point - flashLight.transform.position).normalized;
            flashLight.transform.rotation = Quaternion.LookRotation(dir);
            return;
        }
        else
        {
            dir = (ray.GetPoint(10) - flashLight.transform.position).normalized;
            flashLight.transform.rotation = Quaternion.LookRotation(dir);
        }       
    }

    public void TurnCameraLeft()
    {
        if (!isTurning) //Don't turn if the player is already turning
        {
            GameManager.instance.uiManager.TransitionOut();
            targetRotationY -= 90; //Set camera turn target to 90 degree to the left
            isTurning = true; //Keep track of when player is turning

            cameraIndex--;//Switch target camera index to the one on its left
            if(cameraIndex < 1)
            {
                cameraIndex = 4; //loop value back incase of underflow
            }
        }
    }

    public void TurnCameraRight()
    {
        if (!isTurning) //Don't turn if the player is already turning
        {
            GameManager.instance.uiManager.TransitionOut();
            targetRotationY += 90; //Set camera turn target to 90 degree to the right
            isTurning = true; //Keep track of when player is turning

            cameraIndex++; //Switch target camera index to the one on its right
            if (cameraIndex > 4)
            {
                cameraIndex = 1; //loop value back incase of overflow
            }
        }
    }

    public void TurnCameraUp()
    {
        if(!isTurning)
        {
            disableSideTurning = true;
            CamLookup(true);
        }
    }

    public void TurnCameraDown()
    {
        if (!isTurning)
        {
            disableSideTurning = false;
            CamLookup(false);
        }
    }

    private void HandleCameraMovement()
    {
        currentRotationY = Mathf.SmoothDamp(currentRotationY, targetRotationY, ref camSpeed, camSmoothTime); //Smooth camera turn (janky camera turn target rn FIX LATER)
        this.transform.rotation = Quaternion.Euler(currentCam.transform.eulerAngles.x, currentRotationY, currentCam.transform.eulerAngles.z);

        if (currentRotationY >= targetRotationY - 5*turningOffset && currentRotationY <= targetRotationY + 5*turningOffset && isTurning) //Teleport camera to target position when it has almost turned toward the target
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
        if(cameraIndex == 1)
        {
            currentCam = kitchenCam;
        }
        else if(cameraIndex == 2)
        {
            currentCam = coffinCam;
        }
        else if (cameraIndex == 3)
        {
            currentCam = laundryCam;
        }
        else if (cameraIndex == 4)
        {
            currentCam = bathroomCam;
        }
        SetCamPosition();
    }

    private void SetCamPosition() //Set the camera position and rotation to the target camera
    {
        this.transform.position = currentCam.transform.position;
        this.transform.rotation = currentCam.transform.rotation;
        flashLight.transform.rotation = currentCam.transform.rotation;
        currentRotationY = currentCam.transform.eulerAngles.y ;
        targetRotationY = currentCam.transform.eulerAngles.y;
    }

    private void CamLookup(bool dir)
    {
        if (dir)
        {
            playerCameraObject.transform.position = ceilingCam.transform.position;
            playerCameraObject.transform.rotation = ceilingCam.transform.rotation;
        }
        else
        {
            playerCameraObject.transform.position = currentCam.transform.position;
            playerCameraObject.transform.rotation = currentCam.transform.rotation;
        }
    }
}