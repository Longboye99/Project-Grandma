using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] bool holdTrigger = false;
    [SerializeField] bool Trigger = false;
    public float duration;
    public bool holdShaking = false;
    [SerializeField] AnimationCurve normalCurve;
    [SerializeField] AnimationCurve easeInCurve;
    [SerializeField] AnimationCurve easeOutCurve;
    [SerializeField] float curveOffset;
    Vector3 startingPosition;

    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent += OnFinishAnimationEvent;
    }
    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onFinishAnimationEvent -= OnFinishAnimationEvent;
    }

    private void Start()
    {
        startingPosition = transform.position;

    }

    private void Update()
    {
        if(holdTrigger)
        {
            holdTrigger = false;
            if (holdShaking)
            {
                StopLongScrenShake();
            }
            else
            {
                StartLongShake();
            }
        }
        if (Trigger)
        {
            Trigger = false;
            DoScreenShake(1.5f);
        }
    }

    private void OnFinishAnimationEvent(string eventName)
    {
        if(eventName == "ShakeScreen")
        {
            DoScreenShake(2);
        }
    }

    public void  DoScreenShake(float second)
    {
        duration = second;
        StartCoroutine(ScreenShakeCurve(1, normalCurve, 1));
    }

    IEnumerator ScreenShakeCurve(float _shakeDuration, AnimationCurve _curve, float multiplier)
    {
        float elapsedTime = 0;
        duration = _shakeDuration;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = _curve.Evaluate(elapsedTime / duration) * curveOffset * multiplier;
            transform.position = startingPosition + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.position = startingPosition;
    }

    IEnumerator LongShaking()
    {
        while (holdShaking)
        {
            transform.position = startingPosition + Random.insideUnitSphere * curveOffset * 0.1f;
            yield return null;
        }

        yield return ScreenShakeCurve(1f, easeOutCurve, 1/8);
    }

    public void StartLongShake()
    {
        holdShaking = true;
        StartCoroutine(LongShakeSequence());
    }

    public void StopLongScrenShake()
    {
        holdShaking = false;
    }

    IEnumerator LongShakeSequence()
    {
        yield return ScreenShakeCurve(1f, easeInCurve, 1/8);
        DoLongShake();
    }

    private void DoLongShake()
    {
        StartCoroutine(LongShaking());
    }


}
