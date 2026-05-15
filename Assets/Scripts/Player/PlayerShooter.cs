using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("Shoot Settings")] // インスペクタービューに表示
    [SerializeField] private GameObject bulletPrefab;     // 弾のプレハブ
    [SerializeField] private Transform firePoint;         // 射出位置
    [SerializeField] private float shotInterval = 0.2f;   // 射出間隔
    [SerializeField] private float bulletSpeed = 12.0f;   // 弾のスピード
    [SerializeField] private float bulletLifeTime = 3.0f; // 弾の寿命
    [SerializeField] private int bulletDamage = 1; // 弾の攻撃力

    private float shotTimer;             // 次に弾を撃てるようになるまでの残り時間
    private Collider2D[] ownerColliders; // プレイヤー自身のCollider2Dを保存しておく配列

    private void Awake()
    {
        ownerColliders = GetComponentsInChildren<Collider2D>(); // Collider2Dコンポーネントを取得
    }

    // 連射間隔を管理し、キー入力されると弾を生成する関数
    public void TickShoot()
    {
        if (shotTimer > 0.0f) shotTimer -= Time.deltaTime; // クールタイム中なら弾を打たない
        if (shotTimer > 0.0f) return;
        if (!IsShootPressed()) return; // キーが押されてないなら弾を打たない

        Shoot();                  // 弾丸生成
        shotTimer = shotInterval; // 次のクールタイムを代入
    }

    // キーが押されたか判定する関数
    private bool IsShootPressed()
    {
        Keyboard keyboard = Keyboard.current;                   // 現在使用されているキーボードを取得
        return keyboard != null && keyboard.spaceKey.isPressed; // nullではない⋀スペースキーが押されたときTrue
    }

    // 弾丸を生成する関数
    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) // 弾丸プレハブと射出位置がnullの場合エラー処理
        {
            Debug.LogWarning("PlayerShooter: bulletPrefab または firePoint が未設定です。", this);
            return;
        }

        // ObjectPoolで弾を生成
        BulletBase bullet = BulletPool.Instance.Spawn(bulletPrefab, firePoint.position, firePoint.rotation, firePoint.up, bulletSpeed, bulletLifeTime, bulletDamage, ownerColliders);

        if (bullet == null)
        {
            Debug.LogWarning("PlayerShooter: 弾の生成に失敗しました。", this);
        }
    }
}
