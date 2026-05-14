using UnityEngine;

public class NormalEnemy : EnemyBase
{
    [Header("Normal Enemy Move")]
    [SerializeField] private Vector2 moveDirection = Vector2.down;
    [SerializeField] private float moveSpeed = 2.0f;

    [Header("Normal Enemy Attack")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Vector2 attackDirection = Vector2.down;
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private float bulletSpeed = 5.0f;
    [SerializeField] private float bulletLifeTime = 5.0f;
    [SerializeField] private int bulletDamage = 1;

    private float attackTimer;

    protected override void Awake()
    {
        base.Awake();

        if (firePoint == null)
        {
            firePoint = transform;
        }

        attackTimer = Mathf.Max(0.0f, attackInterval);
    }

    protected override void Move()
    {
        MoveInDirection(moveDirection, moveSpeed);
    }

    protected override void Attack()
    {
        if (enemyBulletPrefab == null) return;

        attackTimer -= Time.deltaTime;
        if (attackTimer > 0.0f) return;

        attackTimer = Mathf.Max(0.01f, attackInterval);

        ShootBullet(
            enemyBulletPrefab,
            firePoint,
            attackDirection,
            bulletSpeed,
            bulletLifeTime,
            bulletDamage
        );
    }
}
