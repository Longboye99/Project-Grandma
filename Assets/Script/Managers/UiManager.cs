using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UiManager : MonoBehaviour
{
    [SerializeField] Canvas pausedCanvas;
    [SerializeField] Animator transitionOverlay;
    public GameObject anomalySliderObject;
    public GameObject mouseCursor;
    Animator mouseCursorAnimator;
    private Slider anomalySlider;

    [SerializeField] Canvas sliderCanvas;
    public float sliderValue;
    public float silderMaxValue;

    public TextMeshProUGUI timeDisplay;
    public TextMeshProUGUI anomalyPointDisplay;

    int hour;
    int minute;
    float currentTime;
    float midnightTime;

    public bool isPaused;


    private HandEnum handEnum;

    [SerializeField] Animator flashLightHandAnimator;
    [SerializeField] Animator anomalyHandAnimator;
    [SerializeField] Animator lighterHandAnimator;

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onStartInteract += ActivateInteractSlider;
        GameEventsManager.instance.inputEvents.onCancelInteract += CancelInteract;
        GameEventsManager.instance.inputEvents.onPause += Pause;
        GameEventsManager.instance.playerEvents.onCompleteInteract += CompleteInteract;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onStartInteract -= ActivateInteractSlider;
        GameEventsManager.instance.inputEvents.onCancelInteract -= CancelInteract;
        GameEventsManager.instance.inputEvents.onPause -= Pause;
        GameEventsManager.instance.playerEvents.onCompleteInteract -= CompleteInteract;

    }

    private void Start()
    {
        /*Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        mouseCursorAnimator = mouseCursor.GetComponent<Animator>();


        flashLightHandAnimator.SetTrigger("HandUp");

        anomalySlider = anomalySliderObject.GetComponent<Slider>();
        anomalySliderObject.SetActive(false);
        anomalySlider.maxValue = GameManager.instance.playerManager.maxProgression;

    }

    private void Update()
    {
        anomalySlider.value = GameManager.instance.playerManager.interactProgression;
        anomalyPointDisplay.text = GameManager.instance.anomalyManager.ActiveAnomalies.Count.ToString();
        DisplayTime();
        MoveCursor();

    }

    private void MoveCursor()
    {
        if (!GameManager.instance.playerManager.isHoldingInteract)
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)sliderCanvas.transform, mousePosition, sliderCanvas.worldCamera, out uiPosition); //Position magic to get canvas position of the mouse
            mouseCursor.transform.position = sliderCanvas.transform.TransformPoint(uiPosition);
        }
        
    }

    private void ActivateInteractSlider(InputEventContextEnum context)
    {
        if (context == InputEventContextEnum.Incense)
        {
            flashLightHandAnimator.SetTrigger("HandDown");
            lighterHandAnimator.SetTrigger("HandUp");

            mouseCursorAnimator.SetTrigger("Lighting");

            handEnum = HandEnum.LighterHand;
        }
        else if (context == InputEventContextEnum.Interactable)
        {
            handEnum = HandEnum.Default;

            mouseCursorAnimator.SetTrigger("Interact");
        }
        else
        {
            flashLightHandAnimator.SetTrigger("HandDown");
            anomalyHandAnimator.SetTrigger("HandUp");

            mouseCursorAnimator.SetTrigger("CheckAnomaly");


            handEnum = HandEnum.AnomalyHand;
        }
        anomalySliderObject.SetActive(true);
        Vector2 mousePosition = Input.mousePosition;
        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)sliderCanvas.transform, mousePosition, sliderCanvas.worldCamera, out uiPosition); //Position magic to get canvas position of the mouse
        anomalySlider.transform.position = sliderCanvas.transform.TransformPoint(uiPosition); //Teleport slider to the mouse position
    }

    private void CancelInteract(InputEventContextEnum context)
    {
        if (handEnum == HandEnum.LighterHand)
        {
            lighterHandAnimator.SetTrigger("HandDown");           
        }
        else if (handEnum == HandEnum.AnomalyHand)
        {
            anomalyHandAnimator.SetTrigger("HandDown");
        }
        anomalySliderObject.SetActive(false);
        flashLightHandAnimator.SetTrigger("HandUp");

        mouseCursorAnimator.SetTrigger("Default");

    }

    private void CompleteInteract()
    {
        anomalySliderObject.SetActive(false);

        //Coroutine cursor here
    }

    public void CheckAnomalyCursor(bool value)
    {
        if(value == true)
        {
            mouseCursorAnimator.SetTrigger("Correct");
        }
        else
        {
            mouseCursorAnimator.SetTrigger("Incorrect");
        }

        Invoke("SetCursorDefault", 1);
    }

    public void SetCursorDefault()
    {
        mouseCursorAnimator.SetTrigger("Default");
    }

    public void Pause()
    {
        if (isPaused)
        {
            pausedCanvas.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            isPaused = false;
            return;
        }
        pausedCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void DisplayTime()
    {
        currentTime = GameManager.instance.levelManager.currentTime;

        hour = (int)Math.Floor(currentTime / 60);
        minute = (int)Math.Floor(currentTime % 60 / 10);

        timeDisplay.text = "0" + hour.ToString() + " : " + minute.ToString() + "0";

    }

    public void FlashlightHand(bool isUp)
    {
        if (isUp)
        {
            flashLightHandAnimator.SetTrigger("HandUp");

        }
        else
        {
            flashLightHandAnimator.SetTrigger("HandDown");
        }
    }

    public void TransitionIn()
    {
        transitionOverlay.SetTrigger("TransitionIn");
    }

    public void TransitionOut()
    {
        transitionOverlay.SetTrigger("TransitionOut");
    }

    public void FadeIn()
    {
        transitionOverlay.SetTrigger("FadeIn");
    }

    public void FadeOut()
    {
        transitionOverlay.SetTrigger("FadeOut");
    }

    public void HandShakeStart()
    {
        flashLightHandAnimator.SetTrigger("HandShakeStart");
    }
    public void HandShakeEnd()
    {
        flashLightHandAnimator.SetTrigger("HandShakeEnd");
    }
}

public enum HandEnum
{
    Default,
    AnomalyHand,
    LighterHand
}
