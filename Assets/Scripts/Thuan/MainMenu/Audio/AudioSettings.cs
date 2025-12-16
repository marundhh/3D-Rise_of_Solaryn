using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("Sliders")]
    public Slider bgmSlider;   // Âm lượng Game (BGM)
    public Slider sfxSlider;   // Âm lượng SFX (đánh, chạy, nhảy...)

    [Header("Toggles")]
    public Toggle bgmToggle;   // Bật/tắt BGM
    public Toggle sfxToggle;   // Bật/tắt SFX

    private void Start()
    {
        // --- BGM
        float bgmVol = PlayerPrefs.GetFloat("BGMVolume", 0.75f);
        bgmSlider.value = bgmVol;
        AudioManager.Instance.SetBGMVolume(bgmVol);

        bool bgmOn = PlayerPrefs.GetInt("BGMOn", 1) == 1;
        bgmToggle.isOn = bgmOn;
        AudioManager.Instance.ToggleBGM(bgmOn);
        bgmSlider.interactable = bgmOn;   //disable slider nếu đang tắt

        // --- SFX
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        sfxSlider.value = sfxVol;
        AudioManager.Instance.SetSFXVolume(sfxVol);

        bool sfxOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;
        sfxToggle.isOn = sfxOn;
        AudioManager.Instance.ToggleSFX(sfxOn);
        sfxSlider.interactable = sfxOn;   // disable slider nếu đang tắt

        // --- Nối sự kiện
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        bgmToggle.onValueChanged.AddListener(OnBGMToggled);
        sfxToggle.onValueChanged.AddListener(OnSFXToggled);
    }

    private void OnBGMVolumeChanged(float value)
    {
        AudioManager.Instance.SetBGMVolume(value);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void OnBGMToggled(bool isOn)
    {
        AudioManager.Instance.ToggleBGM(isOn);
        bgmSlider.interactable = isOn;   //disable/enable slider
        PlayerPrefs.SetInt("BGMOn", isOn ? 1 : 0);
    }

    private void OnSFXToggled(bool isOn)
    {
        AudioManager.Instance.ToggleSFX(isOn);
        sfxSlider.interactable = isOn;   //disable/enable slider
        PlayerPrefs.SetInt("SFXOn", isOn ? 1 : 0);
    }
}
