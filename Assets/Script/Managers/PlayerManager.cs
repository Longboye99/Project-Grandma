using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] float rayMaxDistance;
    private int layerMask = (1 << 6);

    public float interactProgression;
    public float maxProgression = 2;

    private bool isHoldingFixAnomaly = false;

    public Anomaly currentAnomaly;
    public bool isLookingAtIncense;
    private bool isLighting;


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
        GameManager.instance.uiManager.silderMaxValue = maxProgression;
    }

    private void Update()
    {
        if (isHoldingFixAnomaly)
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
        isHoldingFixAnomaly = true; //Keep track when mosue is already been held
        isLighting = isLookingAtIncense;
    }

    private void UpdateInteractValue()
    {
        if (interactProgression >= maxProgression) //If timer is complete and is still active (this is to stop the slider from reactivating again without letting go of the mouse)
        {
            if (isLighting)
            {
                GameEventsManager.instance.playerEvents.RefilIncense();
            }
            else if (currentAnomaly != null)
            {
                GameEventsManager.instance.anomalyEvents.UndoAnomaly(currentAnomaly);
            }
            GameEventsManager.instance.playerEvents.CompleteInteract();
            isHoldingFixAnomaly = false;
            isLighting = false;
        }
        else
        {
            interactProgression += Time.deltaTime;
        }
    }

    private void CancelInteract(InputEventContextEnum inputContext)
    {
        isHoldingFixAnomaly = false;
        isLighting = false;
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
                isLookingAtIncense = true;
                GameEventsManager.instance.inputEvents.ChangeInputeventContext(InputEventContextEnum.Incense);
            }
        }
        else
        {
            currentAnomaly = null;
            isLookingAtIncense = false;
            GameEventsManager.instance.inputEvents.ChangeInputeventContext(InputEventContextEnum.Default);
        }
        
    }
  
    /*private void OnDrawGizmos() //Funny green line in inspect
    {
        Vector3 endPos = playerCamera.transform.position + playerCamera.transform.forward * rayMaxDistance;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCamera.transform.position, endPos);

    }   */
}
