using System.Collections;
using System.Collections.Generic;
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

    private BulletPool bulletPool;
    private BulletBase sourcePrefab;
    private bool released = true;
    private float remainingLifeTime;

    private readonly List<Collider2D> ignoredOwnerColliders = new List<Collider2D>();

    public BulletBase SourcePrefab => sourcePrefab; // 他のファイルから値は変更させないが中身だけ見せる
    protected virtual float TimeScale => 1.0f;
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

    // どのプレハブのObjectPoolを使うかを保存
    public void SetPool(BulletPool pool, BulletBase prefab)
    {
        bulletPool = pool;
        sourcePrefab = prefab;
    }

    public virtual void Initialize(Vector2 direction, float speed, float lifeTime, int bulletDamage, Collider2D[] ownerColliders)
    {
        released = false;
        initialized = true;

        RestoreOwnerCollision();

        moveDirection = direction.sqrMagnitude > 0.0f ? direction.normalized : Vector2.up;
        moveSpeed = speed > 0.0f ? speed : defaultSpeed;
        damage = bulletDamage > 0 ? bulletDamage : defaultDamage;
        remainingLifeTime = lifeTime > 0.0f ? lifeTime : defaultLifeTime;

        IgnoreOwnerCollision(ownerColliders);
    }

    protected virtual void Update()
    {
        if (!initialized) return;
        if(!CanRunGameLogic()) return;

        remainingLifeTime -= Time.deltaTime * TimeScale;

        if (remainingLifeTime <= 0.0f)
        {
            ReturnToPool();
        }
    }

    // プールに弾を集める
    protected void ReturnToPool()
    {
        if(released) return;

        released = true;
        initialized = false;
        remainingLifeTime = 0.0f;

        RestoreOwnerCollision();

        if(rb!= null)
        {
            rb.linearVelocity = Vector2.zero; // 速度を0
            rb.angularVelocity = 0.0f;        // 回転速度を0
        }

        if (bulletPool != null)
        {
            bulletPool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 一時的に無効化していた発射者との当たり判定を元に戻す処理
    private void RestoreOwnerCollision()
    {
        for (int i = 0; i < ignoredOwnerColliders.Count; i++)
        {
            Collider2D owner = ignoredOwnerColliders[i];
            if (owner == null) continue;

            for(int j = 0;j < myColliders.Length; j++)
            {
                Collider2D mine = myColliders[j];
                if(mine == null) continue;

                Physics2D.IgnoreCollision(mine, owner, false);
            }
        }

        ignoredOwnerColliders.Clear();
    }

    protected virtual void FixedUpdate()
    {
        if (!initialized) return;

        if (!CanRunGameLogic())
        {
            if(rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            return;
        }

        Move();
    }

    // ゲームがプレイ中か判定
    protected bool CanRunGameLogic()
    {
        return GameManager.Instance == null || GameManager.Instance.CurrentState == GameState.Playing;
    }

    protected virtual void Move()
    {
        rb.linearVelocity = moveDirection * moveSpeed * TimeScale;
    }

    public void Despawn()
    {
        ReturnToPool();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        ReturnToPool();
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

            if (!ignoredOwnerColliders.Contains(owner))
            {
                ignoredOwnerColliders.Add(owner);
            }
        }
    }
}
