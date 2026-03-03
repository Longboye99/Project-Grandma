using System.Collections;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float duration;
    [SerializeField] AnimationCurve curve;
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

    private void OnFinishAnimationEvent(string eventName)
    {
        if(eventName == "ShakeScreen")
        {
            DoScreenShake();
        }
    }

    public void  DoScreenShake()
    {
        StartCoroutine(Shaking());
    }

    IEnumerator Shaking()
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime/duration) * curveOffset;
            transform.position = startingPosition + Random.insideUnitSphere * strength;
            yield return null;
        }
        transform.position = startingPosition;
    }
}
