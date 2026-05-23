using UnityEngine;

public class BossAttackPattern : MonoBehaviour
{
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 5.0f;
    [SerializeField] private float bulletLifeTime = 6.0f;
    [SerializeField] private int bulletDamage = 1;

    private Collider2D[] ownerColliders;

    private void Awake()
    {
        if (firePoint == null) firePoint = transform;
        ownerColliders = GetComponentsInChildren<Collider2D>();
    }

    // プレイヤーを探しプレイヤーとの距離を計算しその方向に攻撃
    public void ShootAtPlayer()
    {
        PlayerHealth player = FindAnyObjectByType<PlayerHealth>();
        if(player == null) return;

        Vector2 direction = player.transform.position - firePoint.position;
        SpawnBullet(direction);
    }

    // 円形に攻撃するパターン
    public void ShootCircle(int bulletCount)
    {
        int count = Mathf.Max(1, bulletCount);
        float step = 360.0f / count;

        for(int i = 0; i < count; i++)
        {
            float angle = step * i * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            SpawnBullet(direction);
        }
    }

    // ボス用の弾を生成する
    private void SpawnBullet(Vector2 direction)
    {
        if(enemyBulletPrefab == null || firePoint == null) return;

        Vector2 finalDirection = direction.sqrMagnitude > 0.0f ? direction.normalized : Vector2.down;
        float angle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg - 90.0f;

        BulletPool.Instance.Spawn(enemyBulletPrefab, firePoint.position, Quaternion.Euler(0.0f, 0.0f, angle), finalDirection, bulletSpeed, bulletLifeTime, bulletDamage, ownerColliders);
    }
}
