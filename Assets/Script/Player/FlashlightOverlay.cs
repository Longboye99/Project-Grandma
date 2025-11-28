using System.Collections;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class FlashlightOverlay : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Animator animator;
    [SerializeField] Animation lightFlickering;
    [SerializeField] Animation heavyFlickering;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += Blink;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= Blink;


    }


    private void TriggerLightAnomaly()
    {
        animator.SetTrigger("LightFlickering");
        StartCoroutine(WaitForFlickering());
    }

    IEnumerator WaitForFlickering()
    {
        yield return new WaitForSeconds(0.13f);
        animator.SetTrigger("Default");
    }

    public void TriggerHeavyAnomaly()
    {
        animator.SetTrigger("HeavyFlickering");
        StartCoroutine(WaitForFlickering());
    }

    private void Blink(Anomaly anomaly)
    {
        animator.SetTrigger("LightDown");
        StartCoroutine(WaitForFlickering());

    }
}
