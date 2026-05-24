using System.Linq;
using UnityEngine;

public class TimeGaugeView : MonoBehaviour
{
    [SerializeField] private Transform segmentRoot;
    [SerializeField] private Sprite litSprite;
    [SerializeField] private Sprite emptySprite;

    private SpriteRenderer[] segments;

    private void Awake()
    {
        if (segmentRoot == null)
        {
            segmentRoot = transform;
        }

        segments = segmentRoot
            .GetComponentsInChildren<SpriteRenderer>(true)
            .OrderBy(renderer => renderer.transform.GetSiblingIndex())
            .ToArray();

        Debug.Log($"TimeGaugeView: segments={segments.Length}", this);
    }

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
        if (segments == null || segments.Length == 0) return;

        float rate = max <= 0f ? 0f : Mathf.Clamp01(current / max);
        int litCount = Mathf.CeilToInt(rate * segments.Length);

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].sprite = i < litCount ? litSprite : emptySprite;
        }
    }
}