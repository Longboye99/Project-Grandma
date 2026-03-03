using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class TutorialCutsceneSequence : MonoBehaviour
{
    [SerializeField] DataSwitchContainer dataSwitcher;
    [SerializeField] LocalSpreadsheetContainer DataContainer;
    

    [Header("Player Components")]
    [SerializeField] PlayerCutsceneController cutsceneController;
    [SerializeField] PointClickCameraMovement pointClickCameraMovement;
    [SerializeField] PointClickCameraController pointClickCameraController;
    [SerializeField] PlayableDirector cutscenePlayer;
    [SerializeField] GameObject flashlightHand;

    [Header("Anomaly")] 
    [SerializeField] Anomaly tutorialAnomaly;

    bool firstTime = true;
    bool hasTurned;
    [SerializeField] bool skipCutscene;
    [SerializeField] AudioClip howlingNoise;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += FinishAnimationEvent;
        GameEventsManager.instance.playerEvents.onRefillIncense += OnRefilIncense;
        GameEventsManager.instance.playerEvents.onMoveToArea += DetectMovingToArea;
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += DetectUndoAnomaly;
        GameEventsManager.instance.playerEvents.onStartTurning += StartTurning;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= FinishAnimationEvent;
        GameEventsManager.instance.playerEvents.onRefillIncense -= OnRefilIncense;
        GameEventsManager.instance.playerEvents.onMoveToArea -= DetectMovingToArea;
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= DetectUndoAnomaly;
        GameEventsManager.instance.playerEvents.onStartTurning -= StartTurning;

    }

    private void Start()
    {
        DataContainer = dataSwitcher.currentData;
        skipCutscene = DataContainer.skipCutscene;

        if(!skipCutscene)
        {
            Debug.Log("disable stuff");
            pointClickCameraController.EnableFlashlight(false);
            GameManager.instance.uiManager.FlashlightHand(false);
            GameManager.instance.playerManager.EnableInteract(false);
            GameEventsManager.instance.playerEvents.EnableMovement(false);
            GameManager.instance.levelManager.PauseTimer(true);
            GameManager.instance.uiManager.EnableGameOverlay(false);

            cutsceneController.TeleportToIncense();
            DoStartingCutscene();
        }
        else
        {
            GameManager.instance.levelManager.RefillIncense();
            pointClickCameraMovement.SetCamPosition();
            this.gameObject.SetActive(false);
        }
    }

    private void FinishAnimationEvent(string eventName)
    {
        if (eventName == "FinishStartingCutscene")
        {
            FinishStartingCutscene();
            
        }
    }

    private void DoStartingCutscene()
    {
        Debug.Log("cutscene");
        cutscenePlayer.Play();
    }

    private void FinishStartingCutscene()
    {
        pointClickCameraMovement.SetCamPosition();
        pointClickCameraController.EnableFlashlight(true);
        GameManager.instance.uiManager.FlashlightHand(true);
        GameManager.instance.playerManager.EnableInteract(true);
        GameManager.instance.uiManager.EnableGameOverlay(true);

        Invoke("DisplayIncenseSubtitle", 1);
    }

    private void DisplayIncenseSubtitle()
    {
        GameManager.instance.uiManager.subtitleTextController.SetSubtitleText("ต้องจุดธูปใหม่แล้ว");
        GameManager.instance.uiManager.floatingTextController.EnableTutorialText(TutorialText.IncenseTutorial);
    }


    private void OnRefilIncense()
    {
        if (!skipCutscene)
        {
            GameManager.instance.uiManager.subtitleTextController.DisableTitleText();
            GameManager.instance.uiManager.floatingTextController.DisableTutorialText(); ;

            Invoke("SpawnAnomaly", 1);
        }
    }
    
    private void SpawnAnomaly()
    {
        GameManager.instance.anomalyManager.TriggerAnomaly(tutorialAnomaly);
        Invoke("DoAnomalyDialogue", 5f);
    }

    private void DoAnomalyDialogue()
    {
        GameManager.instance.uiManager.subtitleTextController.SetSubtitleText("มันมาอีกแล้วรึเปล่า... ต้องไปเช็ค", 7);
        Invoke("DisplayMovementTutorial", 5);
    }

    private void DisplayMovementTutorial()
    {
        GameEventsManager.instance.playerEvents.EnableMovement(true);
        GameManager.instance.levelManager.PauseTimer(false);
        GameManager.instance.uiManager.floatingTextController.EnableTutorialText(TutorialText.MovementTutorial, 5);
    }

    private void StartTurning()
    {
        if (!hasTurned)
        {
            hasTurned = true;
            GameManager.instance.uiManager.floatingTextController.RemoveTutorialText("FinishTextFading");

        }
    }

    private void DetectMovingToArea(AreaEnum area)
    {
        if(area == AreaEnum.Kitchen && firstTime && !skipCutscene)
        {
            firstTime = false;
            GameEventsManager.instance.playerEvents.EnableMovement(false);
            GameManager.instance.playerManager.EnableInteract(false);


            Invoke("DisplayAnomalySubtitle", 1);
            Invoke("DisplayTutorial", 5);
        }
    }

    private void DisplayAnomalySubtitle()
    {
        GameManager.instance.uiManager.subtitleTextController.SetSubtitleText("(หายใจเข้าลึกๆ.. แค่ปัดเป่าผีร้ายเราทำได้...)", 6);
    }

    private void DisplayTutorial()
    {
        GameManager.instance.uiManager.floatingTextController.EnableTutorialText(TutorialText.AnomalyTutorial);
        GameManager.instance.playerManager.EnableInteract(true);

    }

    private void DetectUndoAnomaly(Anomaly anomaly)
    {
        if(anomaly == tutorialAnomaly && !skipCutscene)
        {
            GameManager.instance.uiManager.floatingTextController.DisableTutorialText();
            Invoke("DisplayUndoAnomalySubtitle", 2);
        }
    }

    private void DisplayUndoAnomalySubtitle()
    {
        GameManager.instance.uiManager.subtitleTextController.SetSubtitleText("ค่ำคืนนี้ยังอีกยาวนานสินะ...", 7);
        Invoke("FinishSequence", 2.5f);
    }

    private void FinishSequence()
    {   
        GameEventsManager.instance.playerEvents.EnableMovement(true);
        this.gameObject.SetActive(false);
        GameManager.instance.sfxManager.PlaySoundFXClip(howlingNoise, pointClickCameraMovement.transform, 0.1f);
    }


    //Detect when move to area ==
    //Disable movement ==
    //Display Subtitle Dialogue ==
    //Display Tutorial text==
    //Wait for fix anomaly events==
    //More dialogue text
    //Finish Sequence

}
