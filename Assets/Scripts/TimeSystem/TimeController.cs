using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance { get; private set; }

    [SerializeField, Range(0.05f, 1.0f)] private float slowScale = 0.3f;
    [SerializeField] private float maxGauge = 100.0f;
    [SerializeField] private float minGaugeToStart = 10.0f;
    [SerializeField] private float consumePerSecond = 20.0f;
    [SerializeField] private float recoverPerSecond = 10.0f;

    public float Gauge {  get; private set; }
    public float MaxGauge => maxGauge;
    public bool IsSlowing { get; private set; }
    public float CurrentEnemyScale => IsSlowing ? slowScale : 1.0f;

    public static float EnemyScale => Instance != null ? Instance.CurrentEnemyScale : 1.0f;
    public static float EnemyDeltaTime => Time.deltaTime * EnemyScale;
    public static float EnemyFixedDeltaTime => Time.fixedDeltaTime * EnemyScale;

    public event Action<float, float> GaugeChanged;
    public event Action<bool> SlowStateChanged;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        } 

        Instance = this;
        maxGauge = Mathf.Max(1.0f, maxGauge);
        Gauge = maxGauge;
        GaugeChanged?.Invoke(Gauge, maxGauge);
    }

    private void Update()
    {
        if(GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            SetSlowing(false);
            return;
        }
        
        bool wantsSlow = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        bool hasGauge = IsSlowing ? Gauge > 0.0f : Gauge >= minGaugeToStart;

        SetSlowing(wantsSlow && hasGauge);

        float delta = Time.unscaledDeltaTime;
        float nextGauge = IsSlowing ? Mathf.Max(0.0f, Gauge - consumePerSecond * delta) : Mathf.Min(maxGauge, Gauge + recoverPerSecond * delta);

        if(!Mathf.Approximately(Gauge, nextGauge))
        {
            Gauge = nextGauge;
            GaugeChanged?.Invoke(Gauge, maxGauge);
        }

        if (Gauge <= 0.0f)
        {
            SetSlowing(false);
        }
    }

    private void SetSlowing(bool value)
    {
        if (IsSlowing == value) return;

        IsSlowing = value;
        SlowStateChanged?.Invoke(IsSlowing);
    }
}
