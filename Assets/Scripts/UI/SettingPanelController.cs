using UnityEngine;
using UnityEngine.UI;

public class SettingPanelController : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;
    [SerializeField] private Slider brightnessSlider;

    private void Start()
    {
        InitializeSliders();
    }

    // 湯硬化されるたびに呼ばれる関数
    private void OnEnable()
    {
        InitializeSliders();
    }

    private void InitializeSliders()
    {
        if (SettingsManager.Instance == null) return;

        if (bgmSlider != null)
        {
            bgmSlider.SetValueWithoutNotify(SettingsManager.Instance.BgmVolume); // 値は変更するがイベントは発火させない
            bgmSlider.onValueChanged.RemoveListener(OnBgmVolumeChanged);         // 以前登録した関数を削除する処理
            bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        }

        if (seSlider != null)
        {
            seSlider.SetValueWithoutNotify(SettingsManager.Instance.SeVolume); // 値は変更するがイベントは発火させない
            seSlider.onValueChanged.RemoveListener(OnSeVolumeChanged);         // 以前登録した関数を削除する処理 
            seSlider.onValueChanged.AddListener(OnSeVolumeChanged);
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.SetValueWithoutNotify(SettingsManager.Instance.Brightness); // 値は変更するがイベントは発火させない
            brightnessSlider.onValueChanged.RemoveListener(OnBrightnessChanged);         // 以前登録した関数を削除する処理
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        }
    }

    private void OnBgmVolumeChanged(float value)
    {
        SettingsManager.Instance?.SetBgmVolume(value);
    }

    private void OnSeVolumeChanged(float value)
    {
        SettingsManager.Instance?.SetSeVolume(value);
    }

    private void OnBrightnessChanged(float value)
    {
        SettingsManager.Instance?.SetBrightness(value);
    }
}
