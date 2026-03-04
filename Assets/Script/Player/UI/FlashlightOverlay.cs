using System.Collections;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class FlashlightOverlay : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Animator animator;
    [SerializeField] Animation lightFlickering;
    [SerializeField] Animation heavyFlickering;
    [SerializeField] AudioClip flashlightBuzz;
    [SerializeField] float volumn;

    bool _isFlickering;

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
        animator.ResetTrigger("LightDown");
        animator.ResetTrigger("Default");
        ;
        GameManager.instance.sfxManager.PlaySoundFXClip(flashlightBuzz, GameObject.FindGameObjectWithTag("PlayerCollider").transform, volumn);
        animator.SetTrigger("LightDown");
        yield return new WaitForSeconds(0.13f);
        animator.SetTrigger("Default");
        _isFlickering = false;
    }

    public void TriggerHeavyAnomaly()
    {
        animator.SetTrigger("HeavyFlickering");
        StartCoroutine(WaitForFlickering());
    }

    public void Blink(Anomaly anomaly)
    {
        if(!_isFlickering)
        {
            _isFlickering = true;
            StartCoroutine(WaitForFlickering());
        }
        
    }
}
