using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiManager : MonoBehaviour
{
    [SerializeField] Canvas pausedCanvas;
    [SerializeField] Animator transitionOverlay;
    public GameObject anomalySliderObject;
    private Slider anomalySlider;

    public float sliderValue;
    public float silderMaxValue;

    public TextMeshProUGUI timeDisplay;
    public TextMeshProUGUI anomalyPointDisplay;

    int hour;
    int minute;
    float currentTime;
    float midnightTime;

    bool isPaused;


    private HandEnum handEnum;

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        anomalySlider = anomalySliderObject.GetComponent<Slider>();
        anomalySliderObject.SetActive(false);
        anomalySlider.maxValue = GameManager.instance.playerManager.maxProgression;

    }

    private void Update()
    {
        anomalySlider.value = GameManager.instance.playerManager.interactProgression;
        anomalyPointDisplay.text = GameManager.instance.anomalyManager.ActiveAnomalies.Count.ToString();
        DisplayTime();
    }

    private void ActivateInteractSlider(InputEventContextEnum context)
    {
        if (context == InputEventContextEnum.Incense)
        {
            lighterHandAnimator.SetTrigger("AnomalyHandUp");
            handEnum = HandEnum.LighterHand;
        }
        else
        {
            anomalyHandAnimator.SetTrigger("AnomalyHandUp");
            handEnum = HandEnum.AnomalyHand;
        }
        anomalySliderObject.SetActive(true);
    }

    private void CancelInteract(InputEventContextEnum context)
    {
        if (handEnum == HandEnum.LighterHand)
        {
            lighterHandAnimator.SetTrigger("AnomalyHandDown");
        }
        else
        {
            anomalyHandAnimator.SetTrigger("AnomalyHandDown");
        }
        anomalySliderObject.SetActive(false);
    }

    private void CompleteInteract()
    {
        anomalySliderObject.SetActive(false);
    }

    public void Pause()
    {
        if (isPaused)
        {
            pausedCanvas.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;
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

    public void TransitionIn()
    {
        transitionOverlay.SetTrigger("TransitionIn");
    }

    public void TransitionOut()
    {
        transitionOverlay.SetTrigger("TransitionOut");
    }
}

public enum HandEnum
{
    AnomalyHand,
    LighterHand
}
