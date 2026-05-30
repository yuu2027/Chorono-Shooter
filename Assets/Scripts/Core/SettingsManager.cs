using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    private const string BgmVolumeKey = "BgmVolume";
    private const string SeVolumeKey = "SeVolume";
    private const string BrightnessKey = "Brightness";

    public float BgmVolume { get; private set; } = 0.7f;
    public float SeVolume { get; private set; } = 0.9f;
    public float Brightness { get; private set; } = 1f;

    private Image brightnessOverlay;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // シーン遷移でも破壊しない

        Load();
        CreateBrightnessOverlay();
        ApplyAll();
    }

    // BGM音量を保存
    public void SetBgmVolume(float value)
    {
        BgmVolume = Mathf.Clamp01(value); // 0～1の範囲に収める
        PlayerPrefs.SetFloat(BgmVolumeKey, BgmVolume);
        PlayerPrefs.Save();

        AudioManager.Instance?.SetBgmVolume(BgmVolume);
    }

    // SE音量を保存
    public void SetSeVolume(float value)
    {
        SeVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SeVolumeKey, SeVolume);
        PlayerPrefs.Save();

        AudioManager.Instance?.SetSeVolume(SeVolume);
    }

    // 明るさを保存
    public void SetBrightness(float value)
    {
        Brightness = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(BgmVolumeKey, BgmVolume);
        PlayerPrefs.Save();

        ApplyBrightness();
    }

    // シーンロード時に呼ばれる初期設定
    private void Load()
    {
        BgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, 0.7f);
        SeVolume = PlayerPrefs.GetFloat(SeVolumeKey, 0.9f);
        Brightness = PlayerPrefs.GetFloat(BrightnessKey, 1.0f);
    }

    // 音量や明るさを反映させる
    private void ApplyAll()
    {
        AudioManager.Instance?.SetBgmVolume(BgmVolume);
        AudioManager.Instance?.SetSeVolume(SeVolume);
        ApplyBrightness();
    }

    // 画面を暗くするための画像を画面全体に作成
    private void CreateBrightnessOverlay()
    {
        GameObject canvasObject = new GameObject("BrightnessOverlayCanvas");
        DontDestroyOnLoad(canvasObject);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000;

        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject imageObject = new GameObject("BrightnessOverlay");
        imageObject.transform.SetParent(canvasObject.transform, false); // imageObjectをcanvasObjectの子オブジェクトにする

        brightnessOverlay = imageObject.AddComponent<Image>();
        brightnessOverlay.color = Color.clear;
        brightnessOverlay.raycastTarget = false; // クリック判定の対象にならない

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        // Canvas全体にぴったり広げるための設定
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    // 画像を黒くして画面を暗くする
    private void ApplyBrightness()
    {
        if (brightnessOverlay == null) return;

        float darkness = 1f - Brightness;
        brightnessOverlay.color = new Color(0f, 0f, 0f, darkness * 0.75f);
    }
}
