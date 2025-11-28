using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class SfxManager : MonoBehaviour 
{
    [SerializeField] AudioSource soundFXObject;

    public void PlaySoundFXClip(AudioClip audioCip, Transform spawnTransform, float volumn)
    {
        AudioSource audioSource = AudioSource.Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioCip;
        audioSource.Play();
        float clipLength = audioSource.clip.length; 
        GameObject.Destroy(audioSource.gameObject, clipLength );
    }
}
