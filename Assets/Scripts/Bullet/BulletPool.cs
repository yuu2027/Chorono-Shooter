using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private int defaultCapacity = 32;    // プール内部の初期容量
    [SerializeField] private int maxSize = 128;           // プールに保持できる最大数
    [SerializeField] private bool collectionCheck = true; // 同じ弾を二重にRelease()していないかをチェックするための設定

    private static BulletPool instance; // BulletPool の実体を1つだけ覚えておくための変数
    private readonly Dictionary<BulletBase, ObjectPool<BulletBase>> pools = new Dictionary<BulletBase, ObjectPool<BulletBase>>(); // 弾Prefabごとに別々のプールを管理する辞書

    // 
    public static BulletPool Instance
    {
        get
        {
            if (instance != null) return instance; // 既に登録済みか確認

            instance = FindAnyObjectByType<BulletPool>(); // BulletPool コンポーネントを探す
            if (instance != null) return instance;        // 探して見つかったらそれを返す

            GameObject poolObject = new GameObject("BulletPool");
            instance = poolObject.AddComponent<BulletPool>();
            return instance;
        }
    }

    private void Awake()
    {
        // BulletPoolが複数存在しないようにする処理
        if (instance!= null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        defaultCapacity = Mathf.Max(1, defaultCapacity); // 不正な値でないかチェック
        maxSize = Mathf.Max(defaultCapacity, maxSize);   // 不正な値でないかチェック
    }

    // GameObjectをBulletBaseに変換
    public BulletBase Spawn(GameObject bulletPrefab, Vector3 position, Quaternion rotation, Vector2 direction, float speed, float lifeTime, int damage, Collider2D[] ownerColliders)
    {
        if (bulletPrefab == null) return null;

        BulletBase prefab = bulletPrefab.GetComponent<BulletBase>();
        if (prefab == null)
        {
            Debug.LogWarning("BulletPool: bulletPrefab に BulletBase がありません。", bulletPrefab);
            return null;
        }

        return Spawn(prefab, position, rotation, direction, speed, lifeTime, damage, ownerColliders);
    }

    public BulletBase Spawn(BulletBase bulletPrefab, Vector3 position, Quaternion rotation, Vector2 direction, float speed, float lifeTime, int damage, Collider2D[] ownerColliders)
    {
        if (bulletPrefab == null) return null;
        ObjectPool<BulletBase> pool = GetPool(bulletPrefab); // 対応する ObjectPool を取得
        BulletBase bullet = pool.Get();                      // 弾を取り出す

        bullet.transform.SetPositionAndRotation(position, rotation);           // 位置と回転を設定
        bullet.Initialize(direction, speed, lifeTime, damage, ownerColliders); // 速度・寿命・ダメージなどを初期化

        return bullet; // 弾を返す
    }

    public void Release(BulletBase bullet)
    {
        if (bullet == null) return;

        BulletBase sourcePrefab = bullet.SourcePrefab;
        if(sourcePrefab == null || !pools.TryGetValue(sourcePrefab, out ObjectPool<BulletBase> pool))
        {
            Destroy(bullet.gameObject);
            return;
        }

        pool.Release(bullet);
    }

    private ObjectPool<BulletBase> GetPool(BulletBase prefab)
    {
        if(pools.TryGetValue(prefab, out ObjectPool<BulletBase> pool))
        {
            return pool;
        }

        pool = new ObjectPool<BulletBase>(
            () => CreateBullet(prefab), OnGetBullet, OnReleaseBullet, OnDestroyBullet, collectionCheck, defaultCapacity, maxSize);

        pools.Add(prefab, pool);
        return pool;
    }

    private BulletBase CreateBullet(BulletBase prefab)
    {
        BulletBase bullet = Instantiate(prefab, transform);
        bullet.SetPool(this, prefab);
        bullet.gameObject.SetActive(false);
        return bullet;
    }

    private void OnGetBullet(BulletBase bullet)
    {
        bullet.gameObject.SetActive(true);
    }

    private void OnReleaseBullet(BulletBase bullet)
    {
        bullet.transform.SetParent(transform);
        bullet.gameObject.SetActive(false);
    }

    private void OnDestroyBullet(BulletBase bullet)
    {
        if (bullet == null) return;
        Destroy(bullet.gameObject);
    }
}
