using System.Xml.Serialization;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    int anomalyCount; 
    AreaEnum area;
    List<Anomaly> activeAnomaly = new List<Anomaly>();

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        anomalyCount = 0;
        area = GameManager.instance.anomalyManager.currentArea;
        activeAnomaly = GameManager.instance.anomalyManager.dictionary.ActiveAnomalies;
        foreach (var item in activeAnomaly)
        {
            if(item.area == area)
            {
                anomalyCount++;
            }
        }

        SetAmbienceVolumn(1 - (0.3f * anomalyCount));  
    }

    public void SetAmbienceVolumn(float volumn)
    {
        audioSource.volume = volumn;
    }
    
}
