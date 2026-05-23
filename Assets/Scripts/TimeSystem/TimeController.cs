using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance { get; private set; }

    [SerializeField, Range(0.05f, 1.0f)] private float slowScale = 0.3f;
    [SerializeField] private float maxGauge = 100.0f;
    [SerializeField] private float minGaugeToStart = 10.0f;  // ゲージの使用量
    [SerializeField] private float consumePerSecond = 20.0f; // ゲージの消費量
    [SerializeField] private float recoverPerSecond = 10.0f; // ゲージの回復量

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
        // オブジェクトが2以上の場合片方を削除
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

    // キー入力が行われたとき、スローゲージの更新
    private void Update()
    {
        // プレイ中ではない場合スローにしない
        if(GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
        {
            SetSlowing(false);
            return;
        }
        
        bool wantsSlow = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed; // キー入力
        bool hasGauge = IsSlowing ? Gauge > 0.0f : Gauge >= minGaugeToStart;                  // スローを使えるだけのゲージがあるかを判定

        SetSlowing(wantsSlow && hasGauge); // IsSlowingをtrueかfalseに変える

        float delta = Time.unscaledDeltaTime;
        // IsSlowingがtrueならゲージを減らしfalseなら増やす
        float nextGauge = IsSlowing ? Mathf.Max(0.0f, Gauge - consumePerSecond * delta) : Mathf.Min(maxGauge, Gauge + recoverPerSecond * delta);

        if(!Mathf.Approximately(Gauge, nextGauge)) // ゲージの更新
        {
            Gauge = nextGauge;
            GaugeChanged?.Invoke(Gauge, maxGauge);
        }

        if (Gauge <= 0.0f)
        {
            SetSlowing(false);
        }
    }

    // スロー状態を安全に変更するための関数
    private void SetSlowing(bool value)
    {
        if (IsSlowing == value) return;

        IsSlowing = value;
        SlowStateChanged?.Invoke(IsSlowing);
    }
}
