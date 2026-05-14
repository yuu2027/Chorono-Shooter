using Unity.Properties;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BulletBase : MonoBehaviour
{
    [SerializeField] protected float defaultSpeed = 8.0f;    // 弾プレハブ側に設定しておく初期値・予備値
    [SerializeField] protected float defaultLifeTime = 3.0f; // 弾プレハブ側に設定しておく初期値・予備値
    [SerializeField] protected int defaultDamage = 1;        // 弾プレハブ側に設定しておく初期値・予備値

    protected Rigidbody2D rb;           // Rigidbody2D用の変数
    protected Collider2D[] myColliders; // 弾自身についている Collider2D を保存する配列

    protected Vector2 moveDirection = Vector2.up; // 移動方向を上に設定
    protected float moveSpeed;                    // 実際にこの弾が移動するときに使う速度
    protected int damage;                         // 与える攻撃力
    protected bool initialized;                   // 初期化判定用

    public int Damage => damage;

    protected Vector2 direction;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myColliders = GetComponentsInChildren<Collider2D>();

        rb.gravityScale = 0.0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // すり抜け対策

        moveSpeed = defaultSpeed;
        damage = defaultDamage;
    }

    protected virtual void Start()
    {
        if (!initialized)
        {
            Initialize(transform.up, defaultSpeed, defaultLifeTime, defaultDamage, null);
        }
    }

    public virtual void Initialize(Vector2 direction, float speed, float lifeTime, int bulletDamage, Collider2D[] ownerColliders)
    {
        moveDirection = direction.sqrMagnitude > 0.0f ? direction.normalized : Vector2.up;
        moveSpeed = speed > 0.0f ? speed : defaultSpeed;
        damage = bulletDamage > 0 ? bulletDamage : defaultDamage;
        initialized = true;

        IgnoreOwnerCollision(ownerColliders);
        
        float finalLifeTime = lifeTime > 0.0f ? lifeTime : defaultLifeTime;
        Destroy(gameObject, finalLifeTime);
    }

    protected virtual void FixedUpdate()
    {
        if (!initialized) return;

        Move();
    }

    protected virtual void Move()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }

    // 弾が発射した本人に当たらないようにする処理
    protected void IgnoreOwnerCollision(Collider2D[] ownerColliders)
    {
        if (ownerColliders == null) return;
        
        for(int i = 0; i < ownerColliders.Length; i++)
        {
            Collider2D owner = ownerColliders[i];
            if (owner == null) continue;
            
            for(int j = 0; j < myColliders.Length; j++)
            {
                Collider2D mine = myColliders[j];
                if (mine == null) continue;

                Physics2D.IgnoreCollision(mine, owner, true); // 当たり判定を無効化
            }
        }
    }
}
