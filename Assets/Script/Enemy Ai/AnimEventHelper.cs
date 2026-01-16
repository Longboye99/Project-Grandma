using UnityEngine;

public class AnimEventHelper : MonoBehaviour
{
    [SerializeField] string eventName1;
    [SerializeField] string eventName2;
    [SerializeField] string eventName3;
    private void FinishAnimation1()
    {
        GameEventsManager.instance.anomalyEvents.FinishAnimationEvent(eventName1);
    }

    private void FinishAnimation2()
    {
        GameEventsManager.instance.anomalyEvents.FinishAnimationEvent(eventName2);
    }

    private void FinishAnimation3()
    {
        GameEventsManager.instance.anomalyEvents.FinishAnimationEvent(eventName3);
    }

}
