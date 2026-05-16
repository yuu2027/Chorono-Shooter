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
    private Coroutine lifeTimeCoroutine;
    private bool released = true;

    private readonly List<Collider2D> ignoredOwnerColliders = new List<Collider2D>();

    public BulletBase SourcePrefab => sourcePrefab; // 他のファイルから値は変更させないが中身だけ見せる

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

        StopLifeTimeCoroutine();
        RestoreOwnerCollision();

        moveDirection = direction.sqrMagnitude > 0.0f ? direction.normalized : Vector2.up;
        moveSpeed = speed > 0.0f ? speed : defaultSpeed;
        damage = bulletDamage > 0 ? bulletDamage : defaultDamage;

        IgnoreOwnerCollision(ownerColliders);
        
        float finalLifeTime = lifeTime > 0.0f ? lifeTime : defaultLifeTime;
        lifeTimeCoroutine = StartCoroutine(LifeTimeRoutine(finalLifeTime));
    }

    protected void ReturnToPool()
    {
        if(released) return;

        released = true;
        initialized = false;

        StopLifeTimeCoroutine();
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

    // 弾の寿命を管理する関数
    private IEnumerator LifeTimeRoutine(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);

        lifeTimeCoroutine = null;
        ReturnToPool();
    }

    // LifeTimeRoutineを止めるための関数
    private void StopLifeTimeCoroutine()
    {
        if (lifeTimeCoroutine == null) return;
        
        StopCoroutine(lifeTimeCoroutine);
        lifeTimeCoroutine = null;
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

        Move();
    }

    protected virtual void Move()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
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
