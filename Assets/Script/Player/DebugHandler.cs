using UnityEngine;

public class DebugHandler : MonoBehaviour
{
    [SerializeField] GameObject text;
    bool isActive;
    void Update()
    {
        HandleDebugToggles();
        ActivateLookedAnomaly();
        UndoAllAnomaly();
        RefillIncense();
    }

    private void HandleDebugToggles()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            GameEventsManager.instance.debugEvents.PressHighlight();
        }
    }

    private void ActivateLookedAnomaly()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            GameManager.instance.playerManager.currentAnomaly.TriggerAnomaly();
        }
    }

    private void UndoAllAnomaly()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            GameManager.instance.anomalyManager.UndoAllAnomaly();
        }
    }

    private void RefillIncense()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.instance.levelManager.RefillIncense();
        }
    }

    private void AnomalyCount()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            text.SetActive(!isActive);
        }
    }
}
