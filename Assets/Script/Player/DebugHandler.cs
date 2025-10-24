using UnityEngine;

public class DebugHandler : MonoBehaviour
{
    void Update()
    {
        HandleDebugToggles();
        ActivateLookedAnomaly();
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
}
