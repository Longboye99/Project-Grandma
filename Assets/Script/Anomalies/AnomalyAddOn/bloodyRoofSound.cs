using UnityEngine;

public class bloodyRoofSound : MonoBehaviour
{
    Anomaly anomaly;
    bool isPlaying;
    AudioSource bloodAudio;


    private void OnEnable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly += UndoAnomaly;

    }

    private void OnDisable()
    {
        GameEventsManager.instance.anomalyEvents.onUndoAnomaly -= UndoAnomaly;

    }

    void Start()
    {
        anomaly = GetComponent<Anomaly>();
        bloodAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(anomaly.isActive && !isPlaying)
        {
            bloodAudio.Play();
            isPlaying = true;
        }
    }

    private void UndoAnomaly(Anomaly targetAnomaly)
    {
        if (anomaly == targetAnomaly && anomaly.isActive)
        {
            bloodAudio.Stop();
        }
    }
}
