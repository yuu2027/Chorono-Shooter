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
        if(target == null)
        {
            targetSearchTimer -= TimeController.EnemyFixedDeltaTime;

            if(targetSearchTimer <= 0.0f)
            {
                FindTarget();
                targetSearchTimer = targetSearchInterval;
            }
        }

        if(target == null)
        {
            MoveInDirection(Vector2.down, moveSpeed);
            return;
        }

        Vector2 direction = target.position - transform.position;
        MoveInDirection(direction, moveSpeed);
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
