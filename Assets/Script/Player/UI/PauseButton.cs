using UnityEngine;

public class PauseButton : MonoBehaviour
{
    public void Pause()
    {
        GameEventsManager.instance.inputEvents.Pause();
    }
}
