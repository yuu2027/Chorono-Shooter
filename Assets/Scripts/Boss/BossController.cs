using System;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private int maxHp = 300;
    [SerializeField] private int scoreValue = 5000;
    [SerializeField] private float attackInterval = 2.0f;
    
    private BossAttackPattern attackPattern;
    private int currentHp;
    private float attackTimer;
    private BossState currentState = BossState.Idle; // 最初は待機状態

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;

    public event Action<int, int> HealthChanged;
    public event Action BossDied;

    private void Awake()
    {
        currentHp = maxHp;
        if (attackPattern == null) attackPattern = GetComponent<BossAttackPattern>();
    }

    private void Start()
    {
        HealthChanged?.Invoke(currentHp, maxHp); // ボス用のHP
    }

    // ボスが死んでいないもしくはプレイ中ならボスの攻撃間隔を計算
    private void Update()
    {
        if (currentState == BossState.Dead) return;
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing) return;

        attackTimer -= TimeController.EnemyDeltaTime;
        if (attackTimer > 0.0f) return;

        attackTimer = attackInterval;
        AttackByHpRate();

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
        else // 70%より大きい場合
        {
            currentState = BossState.AttackB;
            attackPattern.ShootCircle(12);
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
