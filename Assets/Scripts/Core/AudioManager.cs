using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private const float SilentDb = -90.0f; // 無音にするためのデシベル値

    [Header("Library")]
    [SerializeField] private AudioCueLibrary cueLibrary;

    [Header("Sources")] // 音源データの入れ物
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    [Header("Mixer Optional")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup seGroup;
    [SerializeField] private string masterVolumeParameter = "MasterVolume";
    [SerializeField] private string bgmVolumeParameter = "BgmVolume";
    [SerializeField] private string seVolumeParameter = "SeVolume";

    private readonly Dictionary<SeId, float> lastSeTimes = new Dictionary<SeId, float>(); // SEが最後に再生された時刻を記録する辞書
    private Coroutine bgmFadeCoroutine;
    private BgmId? currentBgmId;

    private float currentBgmCueVolume = 1f;
    private float bgmUserVolume = 0.7f;
    private float seUserVolume = 0.9f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // シーンが切り替わっても、このGameObjectを破棄しない
        PrepareSources();              // 
    }

    private void Start()
    {
        SetMasterVolume(1f);

        if (SettingsManager.Instance != null)
        {
            SetBgmVolume(SettingsManager.Instance.BgmVolume);
            SetSeVolume(SettingsManager.Instance.SeVolume);
        }
        else
        {
            SetBgmVolume(bgmUserVolume);
            SetSeVolume(seUserVolume);
        }
    }

    // BGMを再生
    public void PlayBgm(BgmId id, bool restartIfSame = false)
    {
        if (cueLibrary == null) return;

        AudioCueLibrary.BgmCue cue = cueLibrary.GetBgm(id);
        if (cue == null || cue.clip == null) return;

        // 同じBGMがすでに再生中でも最初から再生し直すか選択
        if (!restartIfSame && currentBgmId == id && bgmSource.isPlaying) return;

        if (bgmFadeCoroutine != null) // 別のBGMが鳴っていたらフェードアウト
        {
            StopCoroutine(bgmFadeCoroutine);
        }

        currentBgmId = id;
        bgmFadeCoroutine = StartCoroutine(FadeToBgm(cue));
    }

    public void StopBgm(float fadeSeconds = 0.25f)
    {
        if (bgmFadeCoroutine != null)
        {
            StopCoroutine(bgmFadeCoroutine);
        }

        bgmFadeCoroutine = StartCoroutine(FadeOutBgm(fadeSeconds));
    }

    public void PlaySe(SeId id)
    {
        if (cueLibrary == null) return;

        AudioCueLibrary.SeCue cue = cueLibrary.GetSe(id);
        if (cue == null || cue.clip == null) return;
        if (IsSeCoolingDown(id, cue.minInterval)) return;

        lastSeTimes[id] = Time.unscaledTime;
        seSource.pitch = cue.pitch <= 0f ? 1f : cue.pitch;

        seSource.PlayOneShot(cue.clip, cue.volume * seUserVolume);
    }

    public void SetMasterVolume(float value)
    {
        SetMixerVolume(masterVolumeParameter, value);
    }

    public void SetBgmVolume(float value)
    {
        bgmUserVolume = Mathf.Clamp01(value);
        bgmSource.volume = currentBgmCueVolume * bgmUserVolume;
    }

    public void SetSeVolume(float value)
    {
        seUserVolume = Mathf.Clamp01(value);
        seSource.volume = 1f;
    }

    // 音源の準備
    private void PrepareSources()
    {
        if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
        if (seSource == null) seSource = gameObject.AddComponent<AudioSource>();

        bgmSource.playOnAwake = false;              // シーン開始時に自動で音を鳴らすか
        bgmSource.loop = true;                      // 音を繰り返し再生するか
        bgmSource.spatialBlend = 0f;                // 2D音声か3D音声
        bgmSource.outputAudioMixerGroup = bgmGroup; // AudioSourceの音をどのAudioMixerGroupに送るか

        seSource.playOnAwake = false;
        seSource.loop = false;
        seSource.spatialBlend = 0f;
        seSource.outputAudioMixerGroup = seGroup;
    }

    // BGMの切り替え
    private IEnumerator FadeToBgm(AudioCueLibrary.BgmCue cue)
    {
        float fadeSeconds = Mathf.Max(0f, cue.fadeSeconds);

        currentBgmCueVolume = cue.volume;
        float targetVolume = currentBgmCueVolume * bgmUserVolume;

        if (bgmSource.isPlaying && fadeSeconds > 0f)
        {
            yield return FadeVolume(bgmSource.volume, 0f, fadeSeconds);
        }

        bgmSource.clip = cue.clip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        if (fadeSeconds > 0f)
        {
            yield return FadeVolume(0f, targetVolume, fadeSeconds);
        }
        else
        {
            bgmSource.volume = targetVolume;
        }

        bgmFadeCoroutine = null;
    }

    // 現在流れているBGMを徐々に小さくして、完全に停止する処理
    private IEnumerator FadeOutBgm(float fadeSeconds)
    {
        yield return FadeVolume(bgmSource.volume, 0f, Mathf.Max(0f, fadeSeconds));

        bgmSource.Stop();
        bgmSource.clip = null;
        currentBgmId = null;
        bgmFadeCoroutine = null;
    }

    // BGMの音量を、指定した時間をかけて変化させる
    private IEnumerator FadeVolume(float from, float to, float seconds)
    {
        if (seconds <= 0f)
        {
            bgmSource.volume = to;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < seconds)
        {
            elapsed += Time.unscaledDeltaTime;
            bgmSource.volume = Mathf.Lerp(from, to, elapsed / seconds);
            yield return null;
        }

        bgmSource.volume = to;
    }

    private bool IsSeCoolingDown(SeId id, float minInterval)
    {
        if (minInterval <= 0f) return false;
        if (!lastSeTimes.TryGetValue(id, out float lastTime)) return false;

        return Time.unscaledTime - lastTime < minInterval;
    }

    private bool SetMixerVolume(string parameterName, float linearValue)
    {
        if (audioMixer == null || string.IsNullOrWhiteSpace(parameterName)) return false;

        float clamped = Mathf.Clamp(linearValue, 0.0001f, 1f);
        float db = linearValue <= 0f ? SilentDb : Mathf.Log10(clamped) * 20f;
        return audioMixer.SetFloat(parameterName, db);
    }

}
