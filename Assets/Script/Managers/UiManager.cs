using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UiManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] Canvas overlayCanvas;

    [SerializeField] Volume postProcessVolume;
    UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments;
    

    [Header("Hand UI")]
    [SerializeField] Animator flashLightHandAnimator;
    [SerializeField] Animator anomalyHandAnimator;
    [SerializeField] Animator lighterHandAnimator;

    [Header("Cursor and Slider")]
    public GameObject anomalySliderObject;
    public GameObject mouseCursor;
    public Animator mouseCursorAnimator;
    private Slider anomalySlider;
    public float sliderValue;
    public float silderMaxValue;

    [SerializeField] Animator transitionOverlay;
    public SubtitleTextController subtitleTextController;
    public FloatingTextController floatingTextController;

    [Header("Timer")]
    public TextMeshProUGUI timeDisplay;
    int hour;
    int minute;
    float currentTime;
    float midnightTime;
    public bool isPaused;

    private HandEnum handEnum;

    

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onCancelInteract += CancelInteract;
        GameEventsManager.instance.inputEvents.onPause += Pause;
        GameEventsManager.instance.playerEvents.onCompleteInteract += CompleteInteract;
    }

    private void OnDisable()
    {
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

        postProcessVolume.profile.TryGet<UnityEngine.Rendering.Universal.ColorAdjustments>(out colorAdjustments);
        SetPostProcessingLayerIsEnabled(false);

        //flashLightHandAnimator.SetTrigger("HandUp");

        anomalySlider = anomalySliderObject.GetComponent<Slider>();
        anomalySliderObject.SetActive(false);
        anomalySlider.maxValue = GameManager.instance.playerManager.maxProgression;

    }

    private void Update()
    {
        anomalySlider.value = GameManager.instance.playerManager.interactProgression;
        
        DisplayTime();
        MoveCursor();

        
    }

    public void IncenseMouseHover(bool value)
    {
        if(value == true)
        {
            if (!mouseCursorAnimator.GetCurrentAnimatorStateInfo(0).IsName("cursorLightIncense")
            && GameManager.instance.playerManager.enableInteract)
            {
                mouseCursorAnimator.SetTrigger("Lighting");
            }
        }
        else
        {
            if (!mouseCursorAnimator.GetCurrentAnimatorStateInfo(0).IsName("cursorWrongAnomaly")
                && !mouseCursorAnimator.GetCurrentAnimatorStateInfo(0).IsName("cursorCorrectAnomaly"))
            {
                SetCursorDefault();
            }
        }
    }

    private void MoveCursor()
    {
        if (!GameManager.instance.playerManager.isHoldingInteract)
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 uiPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)overlayCanvas.transform, mousePosition, overlayCanvas.worldCamera, out uiPosition); //Position magic to get canvas position of the mouse
            mouseCursor.transform.position = overlayCanvas.transform.TransformPoint(uiPosition);
        }
        
    }

    public void ActivateInteractSlider(InputEventContextEnum context)
    {
        if (context == InputEventContextEnum.Incense)
        {
            flashLightHandAnimator.ResetTrigger("HandUp");
            flashLightHandAnimator.SetTrigger("HandDown");

            lighterHandAnimator.ResetTrigger("HandDown");
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
            flashLightHandAnimator.ResetTrigger("HandUp");
            flashLightHandAnimator.SetTrigger("HandDown");

            anomalyHandAnimator.ResetTrigger("HandDown");
            anomalyHandAnimator.SetTrigger("HandUp");

            mouseCursorAnimator.SetTrigger("CheckAnomaly");


            handEnum = HandEnum.AnomalyHand;
        }
        anomalySliderObject.SetActive(true);
        Vector2 mousePosition = Input.mousePosition;
        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)overlayCanvas.transform, mousePosition, overlayCanvas.worldCamera, out uiPosition); //Position magic to get canvas position of the mouse
        anomalySlider.transform.position = overlayCanvas.transform.TransformPoint(uiPosition); //Teleport slider to the mouse position
    }

    public void CancelInteract(InputEventContextEnum context)
    {
        if (handEnum == HandEnum.LighterHand)
        {
            lighterHandAnimator.ResetTrigger("HandUp");           
            lighterHandAnimator.SetTrigger("HandDown");           
        }
        else if (handEnum == HandEnum.AnomalyHand)
        {
            anomalyHandAnimator.ResetTrigger("HandUp");
            anomalyHandAnimator.SetTrigger("HandDown");
        }
        anomalySliderObject.SetActive(false);

        flashLightHandAnimator.ResetTrigger("HandDown");
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
            SetPostProcessingLayerIsEnabled(false);
            //pausedCanvas.gameObject.SetActive(false);
            SceneManager.UnloadSceneAsync("[DevTest]PauseMenu");
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            isPaused = false;
            return;
        }
        SetPostProcessingLayerIsEnabled(true);
        //pausedCanvas.gameObject.SetActive(true);
            SceneManager.LoadSceneAsync("[DevTest]PauseMenu", LoadSceneMode.Additive);

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

    private void SetPostProcessingLayerIsEnabled(bool _value)
    {
        if (colorAdjustments == null) return;
        colorAdjustments.active = _value;
    }

    public void FlashlightHand(bool isUp)
    {
        if (isUp)
        {
            flashLightHandAnimator.ResetTrigger("HandDown");
            flashLightHandAnimator.SetTrigger("HandUp");

        }
        else
        {
            flashLightHandAnimator.ResetTrigger("HandUp");
            flashLightHandAnimator.SetTrigger("HandDown");
        }
        Debug.Log("Flashlight Hand Enable Mode: " + isUp);
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

    public void EnableGameOverlay(bool value)
    {
        if(value)
        {
            overlayCanvas.gameObject.SetActive(true);
        }
        else
        {
            overlayCanvas.gameObject.SetActive(false);
        }
    }
}

public enum HandEnum
{
    Default,
    AnomalyHand,
    LighterHand
}
