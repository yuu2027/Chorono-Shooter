using UnityEngine;

// Rigidbody2DとCollider2Dを必ず使用する。unityが自動的に追加してくれる
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float defaultSpeed = 12.0f;       // 弾プレハブ側に設定しておく初期値・予備値
    [SerializeField] private float defaultLifeTime = 3.0f;     // 弾プレハブ側に設定しておく初期値・予備値
    [SerializeField] private int defaultDamage = 1; // 弾プレハブ側に設定しておく初期値・予備値

    private Rigidbody2D rb;                     // Rigidbody2D用の変数
    private Collider2D[] myColliders;           // 弾自身についている Collider2D を保存する配列
    private Vector2 moveDirection = Vector2.up; // 移動方向を上に設定
    private float moveSpeed;                    // 実際にこの弾が移動するときに使う速度
    private int damage;                         // 敵に与える攻撃力
    bool initialized;                           // 初期化判定用

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();                    // Rigidbody2Dコンポーネントの取得
        myColliders = GetComponentsInChildren<Collider2D>(); // Collider2Dコンポーネントの取得

        rb.gravityScale = 0.0f;                                          // 重力を0
        rb.freezeRotation = true;                                        // 回転無
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // 高速で移動する物体のすり抜けを防ぎやすくする設定

        moveSpeed = defaultSpeed; // デフォルトで用意したスピードに設定
    }

    private void Start()
    {
        // 初期化できていないなら初期化
        if (!initialized) Initialize(transform.up, defaultSpeed, defaultLifeTime, defaultDamage, null);
    }

    // 初期化用関数
    public void Initialize(Vector2 direction,float speed,float lifeTime, int bulletDamage, Collider2D[] ownerColliders)
    {
        moveDirection = direction.sqrMagnitude > 0.0f ? direction.normalized : Vector2.up; // 不正な値が来たときに安全な値へ置き換える処理
        moveSpeed = speed > 0.0f ? speed : defaultSpeed;                                   // 不正な値が来たときに安全な値へ置き換える処理
        damage = bulletDamage > 0 ? bulletDamage : defaultDamage;                          // 不正な値が来たときに安全な値へ置き換える処理
        initialized = true; // 初期化完了

        float finalLifeTime = lifeTime > 0.0f ? lifeTime : defaultLifeTime; // 不正な値が来たときに安全な値へ置き換える処理
        Destroy(gameObject, finalLifeTime); // 制限時間後にオブジェクトを破壊

        // 撃った本人のCollider情報がないなら、当たり判定無視処理は行わない
        if (ownerColliders == null) return;

        // プレイヤー自身のColliderと弾自身のColliderの衝突を無視する処理
        for (int i = 0; i < ownerColliders.Length; i++)
        {
            Collider2D owner = ownerColliders[i]; // プレイヤーのCollider
            if(owner == null) continue; // 無いなら次に進む

            for(int j = 0; j < myColliders.Length; j++)
            {
                Collider2D mine = myColliders[j]; // 弾丸のCollider
                if(mine == null) continue; // 無いなら次に進む
                Physics2D.IgnoreCollision(mine, owner, true); // プレイヤーと弾丸の当たり判定をなくす
            }
        }
    }

    private void FixedUpdate()
    {
        if (!initialized) return; // 初期化できないなら実行しない

        Vector3 delta = (Vector3)(moveDirection * moveSpeed * Time.fixedDeltaTime); // 移動量を計算
        transform.position += delta; // 移動させる
    }

    // Trigger設定されたColliderに接触したときにUnityから自動で呼ばれる関数
    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy == null) return;
        enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}
