using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    [Header("Volume Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Navigation Links")]
    public Button settingsBackButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Load's Saved Data
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 0.75f);

        // Apply's values to UI
        if (musicSlider) musicSlider.value = savedMusic;
        if (sfxSlider) sfxSlider.value = savedSFX;

        // The initial Mixer Update
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVol", volume);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.globalMusicVolume = volume;
            if (AudioManager.Instance.musicSource != null)
            {
                AudioManager.Instance.musicSource.volume = volume;
            }
        }

        if (audioMixer != null)
        {
            float vol = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
            audioMixer.SetFloat("MusicVol", vol);
        }
        else
        {
            Debug.LogWarning("SettingsMenuScript: AudioMixer is unassigned in the inspector slot!");
        }
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVol", volume);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.globalSfxVolume = volume;
        }

        if (audioMixer != null)
        {
            float vol = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
            audioMixer.SetFloat("SFXVol", vol);
        }
    }
}
