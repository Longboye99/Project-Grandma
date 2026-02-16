using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] AudioMixer _masterMixer;
    private void Start()
    {
        if (PlayerPrefs.HasKey("ambienceVolume"))
        {
            float volume = PlayerPrefs.GetFloat("ambienceVolume");
            _masterMixer.SetFloat("ambienceVolume", Mathf.Log10(volume) * 20);
        }
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            float volume = PlayerPrefs.GetFloat("sfxVolume");
            _masterMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
        }
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            float volume = PlayerPrefs.GetFloat("masterVolume");
            _masterMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
        }
    }

}
