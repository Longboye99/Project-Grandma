using UnityEngine;

public class dollActivateSoundEffect : MonoBehaviour
{
    Anomaly anomaly;
    bool played;
    [SerializeField] AudioClip soundEffect;
    [SerializeField] float volumn;

    private void Start()
    {
        anomaly = GetComponent<Anomaly>();
    }

    private void Update()
    {
        if( !played && anomaly.isActive)
        {
            GameManager.instance.sfxManager.PlaySoundFXClip(soundEffect, transform, volumn);
            played = true;
        }
    }
}
