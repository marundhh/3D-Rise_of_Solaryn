using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;  

    [Header("BGM Clips")]
    public AudioClip mainBGM;

    [Header("SFX Clips")]
    public AudioClip clickSFX;
    public AudioClip winSFX;
    public AudioClip loseSFX;
    public AudioClip collectGemSFX;
    public AudioClip moveSFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region BGM
    public void PlayBGM(AudioClip clip = null)
    {
        if (clip == null)
            clip = mainBGM;

        if (bgmSource.clip != clip)
        {
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void ToggleBGM(bool isOn)
    {
        audioMixer.SetFloat("BGMVolume", isOn ? 0 : -80);  
    }
    #endregion

    #region SFX
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayClipSFX() => PlaySFX(clickSFX);
    public void PlayWinSFX() => PlaySFX(winSFX);
    public void PlayLoseSFX() => PlaySFX(loseSFX);
    public void PlayCollectGemSFX() => PlaySFX(collectGemSFX);
    public void PlayMoveSFX() => PlaySFX(moveSFX);

    public void ToggleSFX(bool isOn)
    {
        audioMixer.SetFloat("SFXVolume", isOn ? 0 : -80);
    }
    #endregion

    #region Volume Control
    public void SetBGMVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.001f, 1f)) * 20f;
        audioMixer.SetFloat("BGMVolume", dB);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.001f, 1f)) * 20f;
        audioMixer.SetFloat("SFXVolume", dB);
    }
    #endregion
}
