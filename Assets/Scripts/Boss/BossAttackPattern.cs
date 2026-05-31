using System.Collections.Generic;
using UnityEngine;

public class BossAttackPattern : MonoBehaviour
{
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private float bulletSpeed = 5.0f;
    [SerializeField] private float bulletLifeTime = 6.0f;
    [SerializeField] private int bulletDamage = 1;

    private Collider2D[] ownerColliders;
    private readonly List<Transform> activeFirePoints = new List<Transform>();

    public IReadOnlyList<Transform> GetFirePoints()
    {
        if (activeFirePoints.Count == 0) RebuildFirePoints();
        return activeFirePoints;
    }

    private void Awake()
    {
        RebuildFirePoints();
        ownerColliders = GetComponentsInChildren<Collider2D>();
    }

    private void RebuildFirePoints()
    {
        activeFirePoints.Clear();

        if (firePoints != null)
        {
            foreach (Transform point in firePoints)
            {
                if (point != null) activeFirePoints.Add(point);
            }
        }
    }

    // プレイヤーを探しプレイヤーとの距離を計算しその方向に攻撃
    public void ShootAtPlayer()
    {
        PlayerHealth player = FindAnyObjectByType<PlayerHealth>();
        if(player == null) return;

        foreach (Transform point in GetFirePoints())
        {
            SpawnBullet(point, player.transform.position - point.position);
        }
    }

    // 円形に攻撃するパターン
    public void ShootCircle(int bulletCount)
    {
        int count = Mathf.Max(1, bulletCount);
        float step = 360.0f / count;

        foreach (Transform point in GetFirePoints())
        {
            for (int i = 0; i < count; i++)
            {
                float angle = step * i * Mathf.Deg2Rad;
                SpawnBullet(point, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
            }
        }
    }

    public void ShootHomingBullet()
    {
        PlayerHealth player = FindAnyObjectByType<PlayerHealth>();
        if (player == null) return;

        foreach (Transform point in GetFirePoints())
        {
            SpawnBullet(point, player.transform.position - point.position);
        }
    }

    // ボス用の弾を生成する
    private void SpawnBullet(Transform point, Vector2 direction)
    {
        if(enemyBulletPrefab == null || point == null) return;

        Vector2 finalDirection = direction.sqrMagnitude > 0.0f ? direction.normalized : Vector2.down;
        float angle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg - 90.0f;

        BulletPool.Instance.Spawn(enemyBulletPrefab, point.position, Quaternion.Euler(0.0f, 0.0f, angle), finalDirection, bulletSpeed, bulletLifeTime, bulletDamage, ownerColliders);
    }

}
