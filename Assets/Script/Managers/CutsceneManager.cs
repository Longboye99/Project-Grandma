using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [Header("Player Components")]
    [SerializeField] PlayerCutsceneController cutsceneController;
    [SerializeField] PointClickCameraMovement pointClickCameraMovement;
    [SerializeField] PointClickCameraController pointClickCameraController;
    [SerializeField] PlayableDirector cutscenePlayer;
    [SerializeField] GameObject flashlightHand;

    [Header("Anomaly")]
    [SerializeField] Anomaly anomaly;

    [Header("Tutorial Text")]
    [SerializeField] GameObject tutorialText;


    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += FinishAnimationEvent;
        GameEventsManager.instance.playerEvents.onRefillIncense += OnRefilIncense;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= FinishAnimationEvent;
        GameEventsManager.instance.playerEvents.onRefillIncense -= OnRefilIncense;

    }

    private void Start()
    {
        Debug.Log("disable stuff");
        pointClickCameraController.EnableFlashlight(false);
        GameManager.instance.uiManager.FlashlightHand(false);
        GameManager.instance.playerManager.EnableInteract(false);
        GameEventsManager.instance.playerEvents.EnableMovement(false);
        GameManager.instance.levelManager.PauseTimer(true);

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
        Invoke("DisplayIncenseSubtitle", 1);
    }

    private void DisplayIncenseSubtitle()
    {
        GameManager.instance.uiManager.subtitleTextController.SetSubtitleText("ต้องจุดธูปใหม่แล้ว");
    }


    private void OnRefilIncense()
    {
        GameEventsManager.instance.playerEvents.EnableMovement(true);
        GameManager.instance.levelManager.PauseTimer(false);

        GameManager.instance.uiManager.subtitleTextController.DisableTitleText();
        Invoke("SpawnAnomaly", 1);
    }
    
    private void SpawnAnomaly()
    {
        anomaly.TriggerAnomaly();

    }

    private void DisplayAnomalySubtitle()
    {

    }
 

    //some anomaly spawning
}

//start intro animation
//Active UI
//Wait til recieve refill incense event
//Activate anomaly and stuff
//Wait til recieve fix anomaly
//Jumpscare n cutscene
//Force camera look
//Wait til recieve refill incense event
//tutorial Ui
//start level manager

/*1.เปิดมาหน้าธูปขึ้นuiไฮไลท์ธูปบอกให้กดเติม 
 * 2.ของตกเดินไปแก้ 
 * 3.ตอนกำลังใกล้จะแก้เสร็จมีเสียงจากขวามือ 
 * 4. ผีพุ่งใส่จากทางขวา 
 * 5.พุ่งเข้ามาเสร็จหายไป ขอบขึ้นออร่าแดงๆกระพริบหน่อยฟิวหัวใจเต้นแรง 
 * 5.ผีหายไปล็อคกล้องหันไปหาธูป 
 * 6.พอจุดอีกรอบเจอกระดาษข้อความบอกว่าให้แก้สิ่งผิดปกติ*/