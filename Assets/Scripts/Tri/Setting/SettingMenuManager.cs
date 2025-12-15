using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingMenuManager : MonoBehaviour
{
    public static bool isVibrate;

    public TMP_Dropdown graphicsDropdown;
    public Slider masterVol, musicVol, SFXVol;
    public AudioMixer MainAudio;
    public Toggle vibrateToggle;
    public void ChangeGraPhicsQuality()
    {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
    }

    public void ChangeMasterVolume()
    {
        MainAudio.SetFloat("MasterVol", masterVol.value);
    }
    public void ChangeMusicVolume()
    {
        MainAudio.SetFloat("MusicVol", musicVol.value);
    }
    public void ChangeSFXVolume()
    {
        MainAudio.SetFloat("SFXVol", SFXVol.value);
    }

    public void ChangeVibrate()
    {
        isVibrate = vibrateToggle.isOn;
    }

    public void OpenLink (string link)
    {
        Application.OpenURL(link);
    }
   
    void Start()
    {
        isVibrate = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
