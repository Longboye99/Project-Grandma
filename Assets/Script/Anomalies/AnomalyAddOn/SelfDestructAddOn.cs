using UnityEngine;

public class SelfDestructAddOn : MonoBehaviour
{
    [SerializeField]  Anomaly anomaly;
    [SerializeField] string destructMessage;

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
        anomaly = GetComponent<Anomaly>();
    }

    private void FinishAnimationEvent(string eventName)
    {
        if(eventName == destructMessage && anomaly.isActive)
        {
            GameEventsManager.instance.anomalyEvents.UndoAnomaly(anomaly);
        }
    }
}
