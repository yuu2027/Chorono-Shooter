using UnityEngine;

public class ChaseEnemy : EnemyBase
{
    [Header("Chase Enemy Move")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float targetSearchInterval = 1.0f;

    private Transform target;
    private float targetSearchTimer;

    protected override void Awake()
    {
        base.Awake();
        FindTarget();
    }

    protected override void Move()
    {
        if (target == null)
        {
            targetSearchTimer -= TimeController.EnemyFixedDeltaTime;

            if (targetSearchTimer <= 0.0f)
            {
                FindTarget();
                targetSearchTimer = targetSearchInterval;
            }
        }

        if (target == null)
        {
            Vector2 fallbackDirection = Vector2.down;
            FaceDirection(fallbackDirection);
            MoveInDirection(fallbackDirection, moveSpeed);
            return;
        }

        Vector2 direction = target.position - transform.position;
        FaceDirection(direction);
        MoveInDirection(direction, moveSpeed);
    }

    private void FaceDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.0f) return;

        Vector2 normalizedDirection = direction.normalized;
        float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg - 90.0f;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    private void FindTarget()
    {
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();

        if(playerHealth != null)
        {
            target = playerHealth.transform;
        }
    }
}
