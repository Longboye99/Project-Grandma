using UnityEngine;

public class textOverlayToggle : MonoBehaviour
{
    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onPause += ToggleText;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onPause -= ToggleText;
    }

    private void ToggleText()
    {
        
    }
}
