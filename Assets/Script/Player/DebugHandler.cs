using UnityEngine;

public class DebugHandler : MonoBehaviour
{
    [SerializeField] GameObject text;
    [SerializeField] Canvas debugCanvas;
    bool isActive;
    void Update()
    {
        HandleDebugToggles();
        ActivateLookedAnomaly();
        TriggerBypassAnomaly();
    }

    private void HandleDebugToggles()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            GameEventsManager.instance.debugEvents.PressHighlight();
            debugCanvas.gameObject.SetActive(!isActive);
            text.SetActive(!isActive);
            isActive = !isActive;
        }
    }

    public void HighlightAnomaly()
    {
        GameEventsManager.instance.debugEvents.PressHighlight();
    }

    public void ActivateLookedAnomaly()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            GameManager.instance.anomalyManager.TriggerAnomaly(GameManager.instance.playerManager.currentAnomaly);
        }
    }

    public void TriggerBypassAnomaly()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GameManager.instance.playerManager.currentAnomaly.TriggerAnomaly();
        }
    }

    public void UndoAllAnomaly()
    {
        GameManager.instance.anomalyManager.UndoAllAnomaly();
    }

    public void RefillIncense()
    {
        GameManager.instance.levelManager.RefillIncense();
    }

    public void ActivateAllAnomaly()
    {
        GameEventsManager.instance.debugEvents.ActivateAllAnomalies();
    }

    public void TriggerJumpscare()
    {
        GameManager.instance.jumpscareManager.EnableJumpscare();
    }

    public void SkipTime()
    {
        GameManager.instance.levelManager.currentTime += 60f;
    }

    public void ReduceIncense()
    {
        GameManager.instance.levelManager.incenseCurrentTime -= 10;
    }
}
