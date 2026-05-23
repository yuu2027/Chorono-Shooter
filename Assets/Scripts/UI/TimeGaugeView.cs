using UnityEngine;
using UnityEngine.UI;

public class TimeGaugeView : MonoBehaviour
{
    [SerializeField] private Slider gaugeSlider;

    private void Start()
    {
        if (TimeController.Instance == null) return;

        TimeController.Instance.GaugeChanged += OnGaugeChanged;
        OnGaugeChanged(TimeController.Instance.Gauge, TimeController.Instance.MaxGauge);
    }

    private void OnDestroy()
    {
        if (TimeController.Instance == null) return;

        TimeController.Instance.GaugeChanged -= OnGaugeChanged;
    }

    private void OnGaugeChanged(float current, float max)
    {
        if (gaugeSlider == null) return;

        gaugeSlider.maxValue = max;
        gaugeSlider.value = current;
    }
}
