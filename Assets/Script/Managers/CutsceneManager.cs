using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] PlayerCutsceneController cutsceneController;
    [SerializeField] PointClickCameraController pointClickCameraController;
    [SerializeField] PlayableDirector cutscenePlayer;
    [SerializeField] PlayableAsset openingCutscene;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += FinishAnimationEvent;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= FinishAnimationEvent;

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
        cutsceneController.MoveAwayFromIncenseTrigger();
        Invoke("FlashlightDelay", 0.5f);
    }

    private void FlashlightDelay()
    {
        pointClickCameraController.EnableFlashlight(true);

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