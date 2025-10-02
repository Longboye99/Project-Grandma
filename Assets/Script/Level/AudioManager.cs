using System.Xml.Serialization;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onTriggerAttackAnomaly += DisableAmbience;
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += EnableAmbience;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onTriggerAttackAnomaly -= DisableAmbience;
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= EnableAmbience;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void DisableAmbience()
    {
        audioSource.volume = 0;
    }

    private void EnableAmbience(Anomaly anomaly)
    {
        if(anomaly.anomalyEnum == AnomalyEnum.AttackAnomaly)
        {
            Invoke("SetVolumnOne", 3);
            
        }

    }

    private void SetVolumnOne()
    {
        audioSource.volume = 1;
    }
    
}
