using UnityEngine;

public class IncensePulsingWarner : MonoBehaviour
{
    [SerializeField] GameObject overlay;
    [SerializeField] Animator overlayAnimator;
    [SerializeField] float lightWarningThreshold;
    [SerializeField] float heavyWarningThreshold;
    float incenseCurrentTime;
    WarningState warningState;

    private void Update()
    {
        incenseCurrentTime = GameManager.instance.levelManager.incenseCurrentTime;

        if(incenseCurrentTime <= heavyWarningThreshold && warningState != WarningState.HeavyWarning)
        {
            warningState = WarningState.HeavyWarning;
            overlayAnimator.SetTrigger("PulseHeavy");
            Debug.Log("Set warning to heavy");
        }
        else if (incenseCurrentTime > heavyWarningThreshold && incenseCurrentTime <= lightWarningThreshold && warningState != WarningState.LightWarning)
        {
            warningState = WarningState.LightWarning;
            overlayAnimator.SetTrigger("PulseLight");
            Debug.Log("Set warning to light");

        }
        else if(incenseCurrentTime > lightWarningThreshold && warningState != WarningState.Default)
        {
            warningState  = WarningState.Default;
            overlayAnimator.SetTrigger("Default");
            Debug.Log("Set warning to default" +
                "");
        }
    }
}

public enum WarningState
{
    Default,
    LightWarning,
    HeavyWarning
}
