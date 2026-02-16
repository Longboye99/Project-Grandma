using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] AudioMixer _masterMixer;

    [SerializeField] Slider _ambienceSlider;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] Slider _masterSlider;


    private void Start()
    {
        if (PlayerPrefs.HasKey("ambienceVolume"))
        {
            LoadAmbienceVolume();
        }
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadSfxVolume();
        }
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            LoadMasterVolume();
        }
    }

    public void SetAmbienceVolume()
    {
        float volume = _ambienceSlider.value;
        _masterMixer.SetFloat("ambienceVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("ambienceVolume", volume);
    }

    public void SetSfxVolume()
    {
        float volume = _sfxSlider.value;
        _masterMixer.SetFloat("ambienceVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
    public void SetMasterVolume()
    {
        float volume = _masterSlider.value;
        _masterMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }
    private void LoadAmbienceVolume()
    {
        _ambienceSlider.value = PlayerPrefs.GetFloat("ambienceVolume");
        SetAmbienceVolume();
    }

    private void LoadSfxVolume()
    {
        _sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        SetSfxVolume();
    }
    private void LoadMasterVolume()
    {
        _masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        SetMasterVolume();
    }
}
