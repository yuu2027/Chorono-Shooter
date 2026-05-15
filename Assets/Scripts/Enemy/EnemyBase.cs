using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int maxHp = 1;        // 敵のHP
    [SerializeField] private int scoreValue = 100; // 倒したらもらえるスコア

    [Header("Screen Out Settings")]
    [SerializeField] private bool destroyWhenOutOfScreen = true; // 画面外に出た敵を削除するかどうか
    [SerializeField] private Camera targetCamera;                // どのカメラを基準に画面外判定をするか
    [SerializeField] private float viewportMargin = 0.2f;        // 画面外判定に少し余白を持たせるための値

    private Rigidbody2D rb; // Rigidbody2D用の変数
    protected Collider2D[] myColliders;
    private int currentHp;  // 現在のHP
    private bool isDead;    // 死亡フラグ

    public int CurrentHp => currentHp;       // 読み取り専用のプロパティ
    public int MaxHp => Mathf.Max(1, maxHp); // 読み取り専用のプロパティ
    public int ScoreValue => scoreValue;     // 読み取り専用のプロパティ

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2Dコンポーネントを取得
        myColliders = GetComponentsInChildren<Collider2D>();

        rb.gravityScale = 0.0f;           // 重力0
        rb.freezeRotation = true;         // 回転なし

        currentHp = MaxHp; // HPを設定

        if (targetCamera == null) // MainCamera を自動で取得する処理
        {
            targetCamera = Camera.main;
        }
    }

    protected virtual void Update()
    {
        if (isDead) return;

        Attack();

        if (destroyWhenOutOfScreen)
        {
            CheckOutOfScreen();
        }
    }

    protected virtual void FixedUpdate()
    {
        if(isDead) return;

        Move();
    }

    // 敵の移動処理
    protected virtual void Move()
    {
        
    }

    // 敵の攻撃処理
    protected virtual void Attack()
    {
        
    }

    protected void MoveInDirection(Vector2 direction, float speed)
    {
        if (speed <= 0.0f) return;

        Vector2 finalDirection = direction.sqrMagnitude > 0.0f
            ? direction.normalized
            : Vector2.down;

        Vector2 nextPosition = rb.position + finalDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    protected GameObject ShootBullet(
        GameObject bulletPrefab,
        Transform firePoint,
        Vector2 direction,
        float speed,
        float lifeTime,
        int damage
    )
    {
        if (bulletPrefab == null) return null;
        if (firePoint == null) return null;

        Vector2 finalDirection = direction.sqrMagnitude > 0.0f
            ? direction.normalized
            : Vector2.down;

        float angle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg - 90.0f;
        GameObject bulletObject = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.Euler(0.0f, 0.0f, angle)
        );

        BulletBase bullet = bulletObject.GetComponent<BulletBase>();
        if (bullet != null)
        {
            bullet.Initialize(finalDirection, speed, lifeTime, damage, myColliders);
            return bulletObject;
        }

        Rigidbody2D bulletRb = bulletObject.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.gravityScale = 0.0f;
            bulletRb.freezeRotation = true;
            bulletRb.linearVelocity = finalDirection * speed;
        }

        if (lifeTime > 0.0f)
        {
            Destroy(bulletObject, lifeTime);
        }

        return bulletObject;
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
