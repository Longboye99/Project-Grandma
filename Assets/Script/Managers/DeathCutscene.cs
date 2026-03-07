using UnityEngine;
using UnityEngine.Playables;

public class DeathCutscene : MonoBehaviour
{
    [SerializeField] PointClickCameraController pointClickCameraController;
    [SerializeField] PlayableDirector cutscenePlayer;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += FinishAnimationEvent;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= FinishAnimationEvent;

    }
    public void StartDeathCutscene()
    {
        pointClickCameraController.EnableFlashlight(false);
        pointClickCameraController.stopCameraMovement = true;
        GameManager.instance.uiManager.FlashlightHand(false);
        GameManager.instance.playerManager.EnableInteract(false);
        GameEventsManager.instance.playerEvents.EnableMovement(false);
        GameManager.instance.levelManager.PauseTimer(true);
        GameManager.instance.uiManager.EnableGameOverlay(false);

        PlayCutscene();
    }

    private void PlayCutscene()
    {
        cutscenePlayer.Play();

    }

    private void FinishAnimationEvent(string eventName)
    {
        if (eventName == "FinishDeathCutscene") 
        {
            GameManager.instance.levelManager.FinishDeathCutscene();
            GameManager.instance.uiManager.TransitionIn();
            Invoke("StopCutscene", 1);
        }
    }

    private void StopCutscene()
    {
        cutscenePlayer.Stop();

    }
}
