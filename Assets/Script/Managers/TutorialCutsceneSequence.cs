using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class TutorialCutsceneSequence : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] PlayerCutsceneController cutsceneController;
    [SerializeField] PointClickCameraMovement pointClickCameraMovement;
    [SerializeField] PointClickCameraController pointClickCameraController;
    [SerializeField] PlayableDirector cutscenePlayer;
    [SerializeField] GameObject flashlightHand;

    [Header("Anomaly")]
    [SerializeField] Anomaly tutorialAnomaly;

    bool firstTime = true;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += FinishAnimationEvent;
        GameEventsManager.instance.playerEvents.onRefillIncense += OnRefilIncense;
        GameEventsManager.instance.playerEvents.onMoveToArea += DetectMovingToArea;
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += DetectUndoAnomaly;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= FinishAnimationEvent;
        GameEventsManager.instance.playerEvents.onRefillIncense -= OnRefilIncense;
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= DetectUndoAnomaly;

    }

    private void Start()
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
        pointClickCameraMovement.isTurning = false;
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
        GameEventsManager.instance.playerEvents.EnableMovement(true);
        GameManager.instance.levelManager.PauseTimer(false);

        GameManager.instance.uiManager.subtitleTextController.DisableTitleText();
        GameManager.instance.uiManager.floatingTextController.DisableTutorialText(); ;

        Invoke("SpawnAnomaly", 1);
    }
    
    private void SpawnAnomaly()
    {
        tutorialAnomaly.TriggerAnomaly();
        Invoke("DoAnomalyDialogue", 5f);
    }

    private void DoAnomalyDialogue()
    {
        GameManager.instance.uiManager.subtitleTextController.SetSubtitleText("มันมาอีกแล้วรึเปล่า... ต้องไปเช็ค", 7);
        Invoke("DisplayMovementTutorial", 5);
    }

    private void DisplayMovementTutorial()
    {
        GameManager.instance.uiManager.floatingTextController.EnableTutorialText(TutorialText.MovementTutorial, 5);
    }

    private void DetectMovingToArea(AreaEnum area)
    {
        if(area == AreaEnum.Kitchen && firstTime)
        {
            firstTime = false;
            GameEventsManager.instance.playerEvents.EnableMovement(false);
            pointClickCameraMovement.isTurning = true;

            GameManager.instance.uiManager.subtitleTextController.SetSubtitleText("มันไม่เคยอยู่ตรงนี้หนิ", 7);
            Invoke("DisplayTutorial", 5);
        }
    }

    private void DisplayTutorial()
    {
        GameManager.instance.uiManager.floatingTextController.EnableTutorialText(TutorialText.AnomalyTutorial);
    }

    private void DetectUndoAnomaly(Anomaly anomaly)
    {
        if(anomaly == tutorialAnomaly)
        {
            GameManager.instance.uiManager.floatingTextController.DisableTutorialText();
            Invoke("DisplayAnomalySubtitle", 2);
        }
    }

    private void DisplayAnomalySubtitle()
    {
        GameManager.instance.uiManager.subtitleTextController.SetSubtitleText("ค่ำคืนนี้ยังอีกยาวนานสินะ...", 7);
        GameEventsManager.instance.playerEvents.EnableMovement(true);
        pointClickCameraMovement.isTurning = false;
    }


    //Detect when move to area ==
    //Disable movement ==
    //Display Subtitle Dialogue ==
    //Display Tutorial text==
    //Wait for fix anomaly events==
    //More dialogue text
    //Finish Sequence

}
