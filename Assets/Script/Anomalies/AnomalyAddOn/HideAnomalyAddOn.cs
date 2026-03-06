using UnityEngine;

public class HideAnomalyAddOn : MonoBehaviour
{
    [SerializeField] Anomaly anomalyBase;
    [SerializeField] Animator animator;
    [SerializeField] bool isHiding;

    private void OnEnable()
    {
        GameEventsManager.instance.playerEvents.onTransitionToArea += TransitionToArea;
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += UndoAnomaly;

    }

    private void OnDisable()
    {
        GameEventsManager.instance.playerEvents.onTransitionToArea -= TransitionToArea;
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= UndoAnomaly;

    }

    private void TransitionToArea(AreaEnum area)
    {
        if(anomalyBase.isActive)
        {
            if (area == anomalyBase.area && !isHiding)
            {
                animator.SetTrigger("Hide");
                isHiding = true;
            }
        }
    }

    private void UndoAnomaly(Anomaly targetAnomaly)
    {
        Anomaly anomaly = GetComponent<Anomaly>();
        if (anomaly == targetAnomaly && anomaly.isActive)
        {
            animator.SetTrigger("Show");
            isHiding = false;
        }
    }
}
