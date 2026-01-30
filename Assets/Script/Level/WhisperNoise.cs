using System.Collections.Generic;
using UnityEngine;

public class WhisperNoise : MonoBehaviour
{
    AudioSource audioSource;
    int anomalyPoint;
    bool isPlaying;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        anomalyPoint = GameManager.instance.anomalyManager.TallyAnomalyPoint();

        if( anomalyPoint >= 50 && !isPlaying)
        {
            audioSource.Play();
            isPlaying = true;
        }
        else if ( anomalyPoint <  50 && isPlaying )
        {
            audioSource.Stop();
            isPlaying = false;
        }
    }

    public void SetAmbienceVolumn(float volumn)
    {
        audioSource.volume = volumn;
    }
}
