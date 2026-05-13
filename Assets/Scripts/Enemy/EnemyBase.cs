using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int maxHp = 1;                  // 敵のHP
    [SerializeField] private int damageFromPlayerBullet = 1; // プレイヤーからの攻撃力
    [SerializeField] private int scoreValue = 100;           // 倒したらもらえるスコア

    [Header("Screen Out Settings")]
    [SerializeField] private bool destroyWhenOutOfScreen = true; // 画面外に出た敵を削除するかどうか
    [SerializeField] private Camera targetCamera;                // どのカメラを基準に画面外判定をするか
    [SerializeField] private float viewportMargin = 0.2f;        // 画面外判定に少し余白を持たせるための値

    private int currentHp; // 現在のHP
    private bool isDead;   // 死亡フラグ

    public int CurrentHp => currentHp;       // 読み取り専用のプロパティ
    public int MaxHp => Mathf.Max(1, maxHp); // 読み取り専用のプロパティ
    public int ScoreValue => scoreValue;     // 読み取り専用のプロパティ

    protected virtual void Awake()
    {
        currentHp = MaxHp; // HPを設定

        if (targetCamera == null) // MainCamera を自動で取得する処理
        {
            targetCamera = Camera.main;
        }
    }

    protected virtual void Update()
    {
        if (destroyWhenOutOfScreen)
        {
            CheckOutOfScreen();
        }
    }

    // ダメージを受けた時の処理
    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;      // すでに死んでいるなら実行しない
        if (damage <= 0) return; // ダメージが０以下なら実行しない

        currentHp = Mathf.Max(currentHp - damage, 0); // HPを削る

        if (currentHp <= 0) // HPが0なら死亡
        {
            Die(); // 死亡処理
        }
    }

    // 死亡処理
    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true; // 死亡フラグを立てる

        // ScoreManagerを作ったら、ここでscoreValueを加算する。
        Destroy(gameObject);
    }

    // 画面外に出た時に破壊する関数
    private void CheckOutOfScreen()
    {
        if (targetCamera == null) return;

        Vector3 viewportPosition = targetCamera.WorldToViewportPoint(transform.position);

        bool isOutOfScreen =
            viewportPosition.x < -viewportMargin ||
            viewportPosition.x > 1.0f + viewportMargin ||
            viewportPosition.y < -viewportMargin ||
            viewportPosition.y > 1.0f + viewportMargin;

        if (isOutOfScreen)
        {
            Destroy(gameObject);
        }
    }
}
