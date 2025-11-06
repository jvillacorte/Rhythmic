using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider masterSlider;

    void Start()
    {
        // Load saved values
        if (PlayerPrefs.HasKey("musicVolume")) musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        if (PlayerPrefs.HasKey("SFXVolume")) SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        if (PlayerPrefs.HasKey("masterVolume")) masterSlider.value = PlayerPrefs.GetFloat("masterVolume");

        ApplyVolumes();

        // Add listeners so sliders update mixer immediately
        masterSlider.onValueChanged.AddListener((v) => SetMasterVolume());
        musicSlider.onValueChanged.AddListener((v) => SetMusicVolume());
        SFXSlider.onValueChanged.AddListener((v) => SetSFXVolume());
    }

    void ApplyVolumes()
    {
        SetMasterVolume();
        SetMusicVolume();
        SetSFXVolume();
    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        myMixer.SetFloat("master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}
