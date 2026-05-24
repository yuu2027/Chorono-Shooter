using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHp = 300;
    [SerializeField] private int scoreValue = 5000;

    [Header("Attack")]
    [SerializeField] private float attackInterval = 2.0f;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float changeTargetInterval = 2.0f;
    [SerializeField] private Vector2 minPosition = new Vector2(-6.5f, 1.0f);
    [SerializeField] private Vector2 maxPosition = new Vector2(6.5f, 4.0f);
    [SerializeField] private float arriveDistance = 0.15f;

    private Rigidbody2D rb;
    private BossAttackPattern attackPattern;

    private int currentHp;
    private float attackTimer;
    private float moveTargetTimer;
    private Vector2 moveTarget;
    private BossState currentState = BossState.Idle; // 最初は待機状態

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;

    public event Action<int, int> HealthChanged;
    public event Action BossDied;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        attackPattern = GetComponent<BossAttackPattern>();

        rb.gravityScale = 0.0f;
        rb.freezeRotation = true;

        currentHp = maxHp;
        PickNewMoveTarget();
    }

    private void Start()
    {
        HealthChanged?.Invoke(currentHp, maxHp); // ボス用のHP
    }

    // ボスが死んでいないもしくはプレイ中ならボスの攻撃間隔を計算
    private void Update()
    {
        if (!CanRun()) return;

        attackTimer -= TimeController.EnemyDeltaTime;
        if (attackTimer <= 0.0f)
        {
            attackTimer = attackInterval;
            AttackByHpRate();
        }

        moveTargetTimer -= TimeController.EnemyDeltaTime;
        if (moveTargetTimer <= 0.0f)
        {
            PickNewMoveTarget();
        }

    }

    private void FixedUpdate()
    {
        if (!CanRun()) return;

        MoveToTarget();
    }

    private bool CanRun()
    {
        if (currentState == BossState.Dead) return false;
        return GameManager.Instance == null || GameManager.Instance.CurrentState == GameState.Playing;
    }

    private void MoveToTarget()
    {
        Vector2 currentPosition = rb.position;
        Vector2 toTarget = moveTarget - currentPosition;

        if (toTarget.magnitude <= arriveDistance)
        {
            PickNewMoveTarget();
            return;
        }

        currentState = BossState.Move;

        Vector2 nextPosition = currentPosition + toTarget.normalized * moveSpeed * TimeController.EnemyFixedDeltaTime;
        nextPosition.x = Mathf.Clamp(nextPosition.x, minPosition.x, maxPosition.x);
        nextPosition.y = Mathf.Clamp(nextPosition.y, minPosition.y, maxPosition.y);

        rb.MovePosition(nextPosition);
    }

    private void PickNewMoveTarget()
    {
        float x = UnityEngine.Random.Range(minPosition.x, maxPosition.x);
        float y = UnityEngine.Random.Range(minPosition.y, maxPosition.y);

        moveTarget = new Vector2(x, y);
        moveTargetTimer = changeTargetInterval;
    }

    // HPの割合に応じて使用する攻撃を変更する
    private void AttackByHpRate()
    {
        if (attackPattern == null) return;

        float hpRate = currentHp / (float)maxHp;

        if(hpRate > 0.7f) // 70%より小さい場合
        {
            currentState = BossState.AttackA;
            attackPattern.ShootAtPlayer();
        }
        else if(hpRate > 0.4f)// 70%より大きい場合
        {
            currentState = BossState.AttackB;
            attackPattern.ShootCircle(12);
        }
        else
        {
            currentState = BossState.AttackC;
            attackPattern.ShootCircle(16);
            attackPattern.ShootHomingBullet();
        }
    }

    // ダメージを受けた時の処理
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;
        if (currentState == BossState.Dead) return;

        currentHp = Mathf.Max(0, currentHp - damage);
        HealthChanged?.Invoke(currentHp, maxHp);

        if(currentHp <= 0)
        {
            Die();
        }
    }

    // ボスが死亡したときの処理
    private void Die()
    {
        currentState = BossState.Dead;
        BossDied?.Invoke();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
            GameManager.Instance.GameClear();
        }

        Destroy(gameObject);
    }
}
