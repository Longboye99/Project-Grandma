using UnityEngine;
using System.Collections.Generic;

public class HeadBop : MonoBehaviour
{
    [Range(0.001f, 0.01f)]
    public float amount;
    float originalAmount;

    [Range(1f,30f)]
    public float frequency;
    float originalFrequency;

    [Range(10f, 100f)]
    public float smooth;

    [SerializeField] PlayerMovementController m_Controller;
    Vector3 startPos;
    bool isBobbing;

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onStartInteract += StartInteract;
        GameEventsManager.instance.inputEvents.onCancelInteract += CancelInteract;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onStartInteract -= StartInteract;
        GameEventsManager.instance.inputEvents.onCancelInteract -= CancelInteract;
    }

    private void Start()
    {
        originalAmount = amount;
        originalFrequency = frequency;
    }

    private void StartInteract(InputEventContextEnum contextEnum)
    {
        amount = amount / 2;
        frequency = frequency / 2; 
    }

    private void CancelInteract(InputEventContextEnum contextEnum)
    {
        amount = originalAmount;
        frequency = originalFrequency;
    }

    private void Update()
    {
        CheckForTrigger();
        StopHeadBob();
    }

    private void CheckForTrigger()
    {
        if(m_Controller.curSpeed > 0)
        {
            StartHeadbob();
        }
    }

    private Vector3 StartHeadbob()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.y, Mathf.Cos(Time.time * frequency) * amount * 1.6f, smooth * Time.deltaTime);
        transform.localPosition += pos;
        return pos;
    }

    private void StopHeadBob()
    {
        if (transform.localPosition == startPos) return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, 1* Time.deltaTime);
    }

}
