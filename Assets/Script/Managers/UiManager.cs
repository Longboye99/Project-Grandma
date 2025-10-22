using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiManager : MonoBehaviour
{
    public GameObject anomalySliderObject;
    private Slider anomalySlider;

    public float sliderValue;
    public float silderMaxValue;

    public TextMeshProUGUI timeDisplay;
    public TextMeshProUGUI anomalyPointDisplay;

    float currentLoop;


    private HandEnum handEnum;

    [SerializeField] Animator anomalyHandAnimator;
    [SerializeField] Animator lighterHandAnimator;

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onStartInteract += ActivateInteractSlider;
        GameEventsManager.instance.inputEvents.onCancelInteract += CancelInteract;
        GameEventsManager.instance.playerEvents.onCompleteInteract += CompleteInteract;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onStartInteract -= ActivateInteractSlider;
        GameEventsManager.instance.inputEvents.onCancelInteract -= CancelInteract;
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
        //anomalyPointDisplay.text = GameManager.instance.anomalyManager.TallyAnomalyPoint().ToString();
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

    private void DisplayTime()
    {
        currentLoop = GameManager.instance.levelManager.currentLoop;
        timeDisplay.text = "0" + currentLoop.ToString() + ":"+"00";
    }
}

public enum HandEnum
{
    AnomalyHand,
    LighterHand
}
