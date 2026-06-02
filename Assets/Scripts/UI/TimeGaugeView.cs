using System.Linq;
using UnityEngine;

public class TimeGaugeView : MonoBehaviour
{
    [SerializeField] private Transform segmentRoot;
    [SerializeField] private Sprite litSprite;
    [SerializeField] private Sprite emptySprite;

    private SpriteRenderer[] segments;

    private TimeController subscribedController;


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
    }

    private void Start()
    {
        subscribedController = TimeController.Instance;
        if (subscribedController == null) return;

        subscribedController.GaugeChanged += OnGaugeChanged;
        OnGaugeChanged(subscribedController.Gauge, subscribedController.MaxGauge);
    }

    private void OnDestroy()
    {
        if (subscribedController == null) return;

        subscribedController.GaugeChanged -= OnGaugeChanged;
        subscribedController = null;
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