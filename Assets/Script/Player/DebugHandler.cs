using TMPro;
using UnityEngine;

public class DebugHandler : MonoBehaviour
{
    [SerializeField] TestEnemy2 enemy;
    
    [SerializeField] Canvas debugCanvas;

    [SerializeField] TextMeshProUGUI anomalyCountDisplay;
    [SerializeField] TextMeshProUGUI anomalyPointDisplay;

    [SerializeField] TextMeshProUGUI difficultyDisplay;
    [SerializeField] TextMeshProUGUI cooldownDisplay;
    [SerializeField] TextMeshProUGUI graceDisplay;

    [SerializeField] TextMeshProUGUI incenseSpeedDisplay;
    [SerializeField] TextMeshProUGUI TimeSpeedDisplay;

    bool isActive = false;

    void Update()
    {
        HandleDebugToggles();
        ActivateLookedAnomaly();
        if(isActive)
        {
            anomalyCountDisplay.text = GameManager.instance.anomalyManager.dictionary.ActiveAnomalies.Count.ToString();
            anomalyPointDisplay.text = GameManager.instance.anomalyManager.TallyAnomalyPoint().ToString();

            difficultyDisplay.text = enemy.difficultyLevel.ToString();
            cooldownDisplay.text = (Mathf.Floor(enemy.currentCooldown * 100)/100).ToString();
            graceDisplay.text = (Mathf.Floor(enemy.graceDuration * 100) / 100).ToString();

            incenseSpeedDisplay.text = GameManager.instance.levelManager.incenseSpeed.ToString();
            TimeSpeedDisplay.text = GameManager.instance.levelManager.timeSpeed.ToString();
        }
        
    }

    private void HandleDebugToggles()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            //GameEventsManager.instance.debugEvents.PressHighlight();
            debugCanvas.gameObject.SetActive(!isActive);
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
        GameManager.instance.levelManager.currentTime += 20f;
    }

    public void ReduceIncense()
    {
        GameManager.instance.levelManager.incenseCurrentTime -= 10;
    }
}
