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

    bool isPaused;


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
        Cursor.visible = true;

        flashLightHandAnimator.SetTrigger("HandUp");
        StartCoroutine(TransitionStart());

        anomalySlider = anomalySliderObject.GetComponent<Slider>();
        anomalySliderObject.SetActive(false);
        anomalySlider.maxValue = GameManager.instance.playerManager.maxProgression;

    }

    private IEnumerator TransitionStart()
    {
        yield return new WaitForSeconds(0.8f);
        transitionOverlay.SetTrigger("TransitionIn");
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
            flashLightHandAnimator.SetTrigger("HandDown");
            lighterHandAnimator.SetTrigger("HandUp");
            handEnum = HandEnum.LighterHand;
        }
        else if (context == InputEventContextEnum.Interactable)
        {
            handEnum = HandEnum.Default;
        }
        else
        {
            flashLightHandAnimator.SetTrigger("HandDown");
            anomalyHandAnimator.SetTrigger("HandUp");
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
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
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
