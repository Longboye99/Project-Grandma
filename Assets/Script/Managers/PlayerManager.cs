using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject playerCameraObject;
    Camera playerCamera;
    [SerializeField] PointClickCameraMovement cameraMovement;
    [SerializeField] float rayMaxDistance;
    private int layerMask = (1 << 6) | (1 << 7);

    public float interactProgression;
    public float maxProgression = 1;

    private bool isHoldingInteract = false;

    public Anomaly currentAnomaly;
    private bool isLookingAtIncense;

    public Interactable currentInteractable;


    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onStartInteract += StartInteract;
        GameEventsManager.instance.inputEvents.onCancelInteract += CancelInteract;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onStartInteract -= StartInteract;
        GameEventsManager.instance.inputEvents.onCancelInteract -= CancelInteract;
    }

    private void Start()
    {
        /*movementController = GameObject.FindAnyObjectByType<PlayerMovementController>();
        movementController.moveSpeed = baseMoveSpeed;*/
        GameManager.instance.uiManager.silderMaxValue = maxProgression;
        playerCamera = playerCameraObject.GetComponent<Camera>();
    }

    private void Update()
    {
        if (isHoldingInteract)
        {
            HandleInteractValue();
            GameManager.instance.uiManager.sliderValue = interactProgression;
        }
        else
        {
            CheckCameraRayCastForInteractable();
        }
        
    }

    private void StartInteract(InputEventContextEnum inputContext)
    {
        GameEventsManager.instance.anomalyEvents.StartHoldingAnomaly();
        interactProgression = 0; //Reset slider timer
        isHoldingInteract = true; //Keep track when mosue is already been held

        isLookingAtIncense = (inputContext == InputEventContextEnum.Incense);
        if(currentInteractable != null)
        {
            GameEventsManager.instance.levelEvents.TriggerInteractable(currentInteractable);
        }
        
        //movementController.moveSpeed = baseMoveSpeed / 2;
    }

    public void TeleportPlayerToRespawn()
    {
        cameraMovement.RespawnPlayer();
    }

    private void HandleInteractValue()
    {
        if (interactProgression >= maxProgression) //If timer is complete and is still active (this is to stop the slider from reactivating again without letting go of the mouse)
        {
            if (isLookingAtIncense)
            {
                GameEventsManager.instance.playerEvents.RefilIncense();
            }
            else if (currentAnomaly != null)
            {
                GameEventsManager.instance.anomalyEvents.UndoAnomaly(currentAnomaly);
            }
            else if (currentInteractable != null)
            {
                
            }
            
            GameEventsManager.instance.playerEvents.CompleteInteract();
            isHoldingInteract = false;
            isLookingAtIncense = false;
            //movementController.moveSpeed = baseMoveSpeed;
        }
        else
        {
            interactProgression += Time.deltaTime;
        }
    }

    private void CancelInteract(InputEventContextEnum inputContext)
    {
        isHoldingInteract = false;
        isLookingAtIncense = false;
        //movementController.moveSpeed = baseMoveSpeed;
        
    }

    private void CheckCameraRayCastForInteractable()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition/3);

        Debug.DrawRay(playerCameraObject.transform.position, ray.direction*50, Color.cyan);
        if (Physics.Raycast(ray, out RaycastHit hit, layerMask))
        {
            Debug.DrawRay(playerCameraObject.transform.position, hit.transform.position, Color.green);
            if (hit.collider.gameObject.GetComponent<Anomaly>())
            {
                currentAnomaly = hit.collider.gameObject.GetComponent<Anomaly>();
                GameEventsManager.instance.inputEvents.ChangeInputeventContext(InputEventContextEnum.Anomaly);

                currentInteractable = null;
            }
            else if (hit.collider.gameObject.GetComponent<Incense>())
            {
                GameEventsManager.instance.inputEvents.ChangeInputeventContext(InputEventContextEnum.Incense);

                currentAnomaly = null;
                currentInteractable = null;
            }
            else if (hit.collider.gameObject.GetComponent<Interactable>())
            {
                currentInteractable = hit.collider.gameObject.GetComponent<Interactable>();
                GameEventsManager.instance.inputEvents.ChangeInputeventContext(InputEventContextEnum.Interactable);

                currentAnomaly = null;
            }
            else
            {
                currentAnomaly = null;
                currentInteractable = null;
                GameEventsManager.instance.inputEvents.ChangeInputeventContext(InputEventContextEnum.Default);
            }

        }
        else
        {
            currentAnomaly = null;
            currentInteractable = null;
            GameEventsManager.instance.inputEvents.ChangeInputeventContext(InputEventContextEnum.Default);
        }

    }


    /*private void OnDrawGizmos() //Funny green line in inspect
    {
        Vector3 endPos = playerCamera.transform.position + playerCamera.transform.forward * rayMaxDistance;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCamera.transform.position, endPos);

    }*/
    
    /*public void DisablePlayerMovement()
    {
        movementController.moveSpeed = 0;
    }

    public void EnablePlayerMovement()
    {
        movementController.moveSpeed = baseMoveSpeed;
    }*/
}
