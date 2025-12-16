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
    public List<AudioClip> BGMClips = new List<AudioClip>();
    [Header("Battle BGM Clips")]
    public List<AudioClip> BattleBGMClips = new List<AudioClip>();

    [Header("Fade Config")]
    [SerializeField] private float fadeOutSeconds = 2f;
    [SerializeField] private float fadeInSeconds = 1.5f;
    private Coroutine transitionCoroutine;

    [Header("Battle State")]
    [SerializeField] private float battleLingerSeconds = 0f;
    private Coroutine lingerCo;
    private int battleCount = 0;

    private bool isInBattle = false;

    private Coroutine playLoopCoroutine;

    private AudioSource footstepSource;

    [Header("SFX Clips")]
    public AudioClip clickSFX;
    public AudioClip winSFX;
    public AudioClip loseSFX;
    public AudioClip collectGemSFX;
    public AudioClip walkSFX;
    public AudioClip runSFX;
    public AudioClip rollSFX;

    private bool bgmEnabled = true;
    private bool sfxEnabled = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.loop = true;
        footstepSource.playOnAwake = false;

        if (audioMixer != null)
        {
            AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("SFX");
            if (groups.Length > 0)
                footstepSource.outputAudioMixerGroup = groups[0];
        }
    }

    private void Start()
    {
        StartNormalBGMLoop();
    }

    #region BGM Control
    public void StartNormalBGMLoop()
    {
        if (!bgmEnabled) return;
        TransitionToList(BGMClips, false);
    }

    public void StartBattleBGMLoop()
    {
        if (!bgmEnabled) return;
        TransitionToList(BattleBGMClips, true);
    }

    private void TransitionToList(List<AudioClip> targetList, bool isBattle)
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(DoTransition(targetList, isBattle));
    }

    private void StopCurrentLoop()
    {
        if (playLoopCoroutine != null)
            StopCoroutine(playLoopCoroutine);
    }

    private IEnumerator PlayBGMLoop(List<AudioClip> clipList, bool isBattle)
    {
        int index = 0;
        while (true)
        {
            if (clipList.Count == 0)
                yield break;

            var clip = clipList[index];
            bgmSource.clip = clip;
            bgmSource.Play();

            yield return new WaitForSeconds(clip.length);

            index = (index + 1) % clipList.Count;
        }
    }

    public void EnterBattle()
    {
        if (!bgmEnabled) return;
        battleCount = Mathf.Max(1, battleCount + 1);

        if (lingerCo != null) { StopCoroutine(lingerCo); lingerCo = null; }

        if (isInBattle) return;
        isInBattle = true;
        StartBattleBGMLoop();
    }

    public void ExitBattle()
    {
        battleCount = Mathf.Max(0, battleCount - 1);

        if (battleCount > 0) return;
        if (!isInBattle) return;

        if (battleLingerSeconds > 0f)
        {
            if (lingerCo != null) StopCoroutine(lingerCo);
            lingerCo = StartCoroutine(LingerThenNormal());
        }
        else
        {
            isInBattle = false;
            StartNormalBGMLoop();
        }
    }

    private IEnumerator LingerThenNormal()
    {
        yield return new WaitForSeconds(battleLingerSeconds);
        if (battleCount == 0 && isInBattle)
        {
            isInBattle = false;
            StartNormalBGMLoop();
        }
        lingerCo = null;
    }

    public void ToggleBGM(bool isOn)
    {
        bgmEnabled = isOn; // cập nhật flag

        audioMixer.SetFloat("BGMVolume", isOn ? 0 : -80);

        if (!isOn)
        {
            StopBGM(); // tắt nhạc ngay
        }
        else
        {
            StartNormalBGMLoop();
        }
    }

    public void StopBGM()
    {
        bgmSource?.Stop();
        StopCurrentLoop();
    }
    #endregion

    #region SFX
    public void PlaySFX(AudioClip clip)
    {
        if (!sfxEnabled || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayAttackSFX(AudioClip attackClip)
    {
        if (!sfxEnabled || attackClip == null) return;
        sfxSource.PlayOneShot(attackClip);
    }

    public void PlayClipSFX() => PlaySFX(clickSFX);
    public void PlayWinSFX() => PlaySFX(winSFX);
    public void PlayLoseSFX() => PlaySFX(loseSFX);
    public void PlayCollectGemSFX() => PlaySFX(collectGemSFX);
    public void PlayRollSFX() => PlaySFX(rollSFX);

    public void ToggleSFX(bool isOn)
    {
        sfxEnabled = isOn; // cập nhật flag

        audioMixer.SetFloat("SFXVolume", isOn ? 0 : -80);

        if (!isOn)
        {
            StopFootstepSFX(); // tắt bước chân ngay
        }
    }
    #endregion

    #region Volume Control
    public void SetBGMVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.001f, 1f)) * 20f;
        audioMixer.SetFloat("BGMVolume", dB);
        bgmEnabled = sliderValue > 0.001f;
    }

    public void SetSFXVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.001f, 1f)) * 20f;
        audioMixer.SetFloat("SFXVolume", dB);
        sfxEnabled = sliderValue > 0.001f;

        if (!sfxEnabled)
        {
            StopFootstepSFX();
        }
    }
    #endregion

    #region Footstep Control
    public void PlayWalkSFX()
    {
        if (!sfxEnabled || walkSFX == null) return;

        if (!footstepSource.isPlaying || footstepSource.clip != walkSFX)
        {
            footstepSource.clip = walkSFX;
            footstepSource.Play();
        }
    }

    public void PlayRunSFX()
    {
        if (!sfxEnabled || runSFX == null) return;

        if (!footstepSource.isPlaying || footstepSource.clip != runSFX)
        {
            footstepSource.clip = runSFX;
            footstepSource.Play();
        }
    }

    public void StopFootstepSFX()
    {
        if (footstepSource.isPlaying)
            footstepSource.Stop();
    }
    #endregion

    #region ===== Fade Helpers & Transition =====
    private IEnumerator DoTransition(List<AudioClip> targetList, bool toBattle)
    {
        // 1) Fade OUT
        if (audioMixer != null)
            yield return FadeMixerTo("BGMVolume", -80f, fadeOutSeconds);
        else
            yield return FadeSourceVolume(bgmSource, bgmSource.volume, 0f, fadeOutSeconds);

        // 2) Dừng vòng lặp cũ & stop
        StopCurrentLoop();
        bgmSource.Stop();

        // 3) Chạy list mới (volume/mixer đang thấp), rồi Fade IN
        if (bgmEnabled)
        {
            playLoopCoroutine = StartCoroutine(PlayBGMLoop(targetList, toBattle));
            yield return null;

            if (audioMixer != null)
                yield return FadeMixerTo("BGMVolume", 0f, fadeInSeconds);
            else
                yield return FadeSourceVolume(bgmSource, 0f, 1f, fadeInSeconds);
        }

        transitionCoroutine = null;
    }

    private IEnumerator FadeMixerTo(string param, float toDb, float duration)
    {
        if (duration <= 0f) { audioMixer.SetFloat(param, toDb); yield break; }

        audioMixer.GetFloat(param, out float fromDb);
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float v = Mathf.Lerp(fromDb, toDb, Mathf.Clamp01(t / duration));
            audioMixer.SetFloat(param, v);
            yield return null;
        }
        audioMixer.SetFloat(param, toDb);
    }

    private IEnumerator FadeSourceVolume(AudioSource src, float from, float to, float duration)
    {
        if (duration <= 0f) { src.volume = to; yield break; }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
            yield return null;
        }
        src.volume = to;
    }
    #endregion
}
