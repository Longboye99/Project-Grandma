using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject playerCameraObject;
    Camera playerCamera;
    [SerializeField] PointClickCameraMovement cameraMovement;
    [SerializeField] float rayMaxDistance;
    [SerializeField] AudioClip lighterAudioOpen;
    [SerializeField] AudioClip lighterAudioClose;
    [SerializeField] AudioClip flashlightBuzz;
    [SerializeField] float volumn;
    private int layerMask = (1 << 6) | (1 << 7);

    public float interactProgression;
    public float maxProgression = 1;
    public bool isHoldingInteract = false;
    public bool completedInteract;
    public bool enableInteract = true;

    public Anomaly currentAnomaly;
    public bool isLookingAtIncense;

    [Header("Interact Control")]
    public Interactable currentInteractable;
    [SerializeField] float minimumHeldDuration;
    [SerializeField] bool doingHeldCountdown;
    float pressedTime;
    InputEventContextEnum inputEventContext;

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onStartInteract += StartInteract;
        GameEventsManager.instance.inputEvents.onCancelInteract += CancelInteract;
        GameEventsManager.instance.playerEvents.onEnableInteract += EnableInteract;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onStartInteract -= StartInteract;
        GameEventsManager.instance.inputEvents.onCancelInteract -= CancelInteract;
        GameEventsManager.instance.playerEvents.onEnableInteract -= EnableInteract;

    }

    private void Start()
    {
        GameManager.instance.uiManager.silderMaxValue = maxProgression;
        playerCamera = playerCameraObject.GetComponent<Camera>();
    }

    private void Update()
    {
        if (enableInteract)
        {
            if (doingHeldCountdown)// start countdown before holding
            {
                CheckCameraRayCastForInteractable();

                if (Time.timeSinceLevelLoad - pressedTime > minimumHeldDuration) //if hold long enough, start holding
                {
                    GameEventsManager.instance.anomalyEvents.StartHoldingAnomaly();
                    doingHeldCountdown = false;
                    isHoldingInteract = true;
                    GameManager.instance.uiManager.ActivateInteractSlider(inputEventContext);
                    if (isLookingAtIncense)
                    {
                        GameManager.instance.sfxManager.PlaySoundFXClip(lighterAudioOpen, playerCameraObject.transform, 0.03f);

                    }
                }
            }
            else if (isHoldingInteract)
            {
                HandleInteractValue();
                GameManager.instance.uiManager.sliderValue = interactProgression;
            }
            else
            {
                RaycastHit hit = CheckCameraRayCastForInteractable();

                if (hit.collider != null && hit.collider.gameObject.GetComponent<Incense>())
                {
                    GameManager.instance.uiManager.IncenseMouseHover(true);
                }
                else if (!isHoldingInteract)
                {
                    GameManager.instance.uiManager.IncenseMouseHover(false);

                }
            }
        }
        
        
    }

    private void StartInteract(InputEventContextEnum inputContext)
    {
        if (!GameManager.instance.uiManager.isPaused)
        {
            inputEventContext = inputContext;

            pressedTime = Time.timeSinceLevelLoad;
            doingHeldCountdown = true;

            isHoldingInteract = false;
            completedInteract = false;

            isLookingAtIncense = (inputContext == InputEventContextEnum.Incense);
            interactProgression = 0;
        }
    }

    private void PerformClick()
    {
        if (currentInteractable != null)
        {
            GameEventsManager.instance.levelEvents.TriggerInteractable(currentInteractable, cameraMovement.currentNode.area, InteractMode.Click);
        }
    }

    private void CancelInteract(InputEventContextEnum inputContext)
    {
        if (!completedInteract)
        {
            GameManager.instance.uiManager.CancelInteract(inputContext);
        }
        
        if (!GameManager.instance.uiManager.isPaused)
        {
            doingHeldCountdown = false;

            if (isHoldingInteract == false && !completedInteract) //Do click if not hold for long enough
            {
                PerformClick();
                Debug.Log("Clicking");
            }
            else if (completedInteract)
            {

            }
            else
            {
                isHoldingInteract = false; //cancel normally if already holding
                isLookingAtIncense = false;
            }
        }
        
    }

    private void HandleInteractValue()
    {
        if (interactProgression >= maxProgression) //If timer is complete and is still active (this is to stop the slider from reactivating again without letting go of the mouse)
        {
            Debug.Log("Holding Complete");
            if (isLookingAtIncense)
            {
                GameEventsManager.instance.playerEvents.RefilIncense();
                GameManager.instance.sfxManager.PlaySoundFXClip(lighterAudioClose, playerCameraObject.transform, 0.03f);
                GameManager.instance.uiManager.CheckAnomalyCursor(true);

            }
            else if (currentAnomaly != null && currentAnomaly.isActive)
            {
                GameEventsManager.instance.anomalyEvents.UndoAnomaly(currentAnomaly);
                GameManager.instance.uiManager.CheckAnomalyCursor(true);
                GameManager.instance.sfxManager.PlaySoundFXClip(flashlightBuzz, playerCameraObject.transform, volumn);
            }
            else if (currentInteractable != null)
            {
                GameEventsManager.instance.levelEvents.TriggerInteractable(currentInteractable, cameraMovement.currentNode.area, InteractMode.Hold);

            }
            else
            {
                GameManager.instance.uiManager.CheckAnomalyCursor(false);
            }
            
            GameEventsManager.instance.playerEvents.CompleteInteract();

            completedInteract = true;
            isHoldingInteract = false;
            isLookingAtIncense = false;
            
            CancelInteract(InputEventContextEnum.Default);
        }
        else
        {
            interactProgression += Time.deltaTime;
        }
    }

    public void EnableInteract(bool value)
    {
        if(value == true)
        {
            enableInteract = true;
        }
        else
        {
            enableInteract = false;
            CancelInteract(InputEventContextEnum.Default);
        }

        Debug.Log("Interact Enable Mode: " + value);
    }

    
    private RaycastHit CheckCameraRayCastForInteractable()
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

        return hit;
    }

    public void TeleportPlayerToRespawn()
    {
        cameraMovement.RespawnPlayer();
    }

}
