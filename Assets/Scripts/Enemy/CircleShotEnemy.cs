using UnityEngine;

public class CircleShotEnemy : EnemyBase
{
    [Header("Circle Shot Move")]
    [SerializeField] private Vector2 moveDirection = Vector2.down;
    [SerializeField] private float moveSpeed = 1.0f;

    [Header("Circle Shot Attack")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float attackInterval = 2.0f;
    [SerializeField] private int bulletCount = 12;
    [SerializeField] private float bulletSpeed = 4.0f;
    [SerializeField] private float bulletLifeTime = 6.0f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private float angleOffset;

    private float attackTimer;

    protected override void Awake()
    {
        base.Awake();

        if(firePoint == null)
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

        attackTimer -= TimeController.EnemyDeltaTime;
        if (attackTimer > 0.0f) return;

        attackTimer = Mathf.Max(0.01f, attackInterval);
        ShootCircle();
    }

    private void ShootCircle()
    {
        int count = Mathf.Max(1, bulletCount);
        float angleStep = 360.0f / count;

        for(int i = 0; i < count; i++)
        {
            float angle = angleOffset + angleStep * i;
            float radians = angle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

            ShootBullet(enemyBulletPrefab, firePoint, direction, bulletSpeed, bulletLifeTime, bulletDamage);
        }
    }
}
