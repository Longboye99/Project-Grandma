using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] PlayerMovementController movementController;
    [SerializeField] float rayMaxDistance;
    private int layerMask = (1 << 6) | (1 << 7);

    public float baseMoveSpeed;
    public float interactProgression;
    public float maxProgression = 1;

    private bool isHoldingInteract = false;

    public Anomaly currentAnomaly;
    public bool isLookingAtBed;
    public bool isLookingAtIncense;


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
        movementController = GameObject.FindAnyObjectByType<PlayerMovementController>();
        movementController.moveSpeed = baseMoveSpeed;
        GameManager.instance.uiManager.silderMaxValue = maxProgression;
    }

    private void Update()
    {
        if (isHoldingInteract)
        {
            UpdateInteractValue();
            GameManager.instance.uiManager.sliderValue = interactProgression;
        }
        
    }

    private void FixedUpdate()
    {
        CheckRayCastForInteractable();
    }

    private void StartInteract(InputEventContextEnum inputContext)
    {
        GameEventsManager.instance.anomalyEvents.StartHoldingAnomaly();
        interactProgression = 0; //Reset slider timer
        isHoldingInteract = true; //Keep track when mosue is already been held

        isLookingAtIncense = (inputContext == InputEventContextEnum.Incense);
        isLookingAtBed = (inputContext == InputEventContextEnum.Bed);
        movementController.moveSpeed = baseMoveSpeed / 2;
    }

    private void UpdateInteractValue()
    {
        if (interactProgression >= maxProgression) //If timer is complete and is still active (this is to stop the slider from reactivating again without letting go of the mouse)
        {
            if (isLookingAtIncense)
            {
                GameEventsManager.instance.playerEvents.RefilIncense();
            }
            else if (isLookingAtBed)
            {
                GameEventsManager.instance.playerEvents.ProgressLoop();
            }
            else if (currentAnomaly != null)
            {
                GameEventsManager.instance.anomalyEvents.UndoAnomaly(currentAnomaly);
            }
            
            GameEventsManager.instance.playerEvents.CompleteInteract();
            isHoldingInteract = false;
            isLookingAtIncense = false;
            isLookingAtBed = false;
            movementController.moveSpeed = baseMoveSpeed;
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
        isLookingAtBed = false;
        movementController.moveSpeed = baseMoveSpeed;
    }

    private void CheckRayCastForInteractable()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * rayMaxDistance, Color.cyan);
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, rayMaxDistance, layerMask))
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * hit.distance, Color.green);
            if (hit.collider.gameObject.GetComponent<Anomaly>())
            {
                currentAnomaly = hit.collider.gameObject.GetComponent<Anomaly>();
                GameEventsManager.instance.inputEvents.ChangeInputeventContext(InputEventContextEnum.Anomaly);
            }
            else if (hit.collider.gameObject.GetComponent<Incense>())
            {
                GameEventsManager.instance.inputEvents.ChangeInputeventContext(InputEventContextEnum.Incense);
            }
            else if (hit.collider.gameObject.GetComponent<Bed>())
            {
                GameEventsManager.instance.inputEvents.ChangeInputeventContext(InputEventContextEnum.Bed);
            }
            
        }
        else
        {
            currentAnomaly = null;
            GameEventsManager.instance.inputEvents.ChangeInputeventContext(InputEventContextEnum.Default);
        }
        
    }
  
    private void OnDrawGizmos() //Funny green line in inspect
    {
        Vector3 endPos = playerCamera.transform.position + playerCamera.transform.forward * rayMaxDistance;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCamera.transform.position, endPos);

    }
    
    public void DisablePlayerMovement()
    {
        movementController.moveSpeed = 0;
    }

    public void EnablePlayerMovement()
    {
        movementController.moveSpeed = baseMoveSpeed;
    }
}
