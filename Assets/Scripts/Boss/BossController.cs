using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    [SerializeField] private Sprite brokenBossSprite;
    [SerializeField] private float deathExplosionInterval = 0.35f;
    [SerializeField] private float deathFallDuration = 1.0f;
    [SerializeField] private float deathFallDistance = 6.0f;
    [SerializeField] private float smokeInterval = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Collider2D[] colliders;
    private bool entranceWaiting;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>();

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

    public void StartEntranceWait(float duration)
    {
        StartCoroutine(EntranceWaitCoroutine(duration));
    }

    // ボスの登場
    private IEnumerator EntranceWaitCoroutine(float duration)
    {
        entranceWaiting = true;
        if (rb != null) rb.linearVelocity = Vector2.zero; // 下に落ちる処理

        yield return new WaitForSeconds(duration);

        entranceWaiting = false;
        GameManager.Instance?.ResumeFromCinematic();
    }

    private bool CanRun()
    {
        if (currentState == BossState.Dead) return false;
        if (entranceWaiting) return false;
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

        EffectManager.Instance?.Play(EffectCueId.BossDamaged, transform.position);
        AudioManager.Instance?.PlaySe(SeId.BossDamage);

    }

    // ボスが死亡したときの処理
    private void Die()
    {
        if (currentState == BossState.Dead) return;

        BulletPool.Instance.ClearAllActiveBullets();

        currentState = BossState.Dead;
        BossDied?.Invoke();

        if (GameManager.Instance != null) // ゲームクリア状態にする
        {
            GameManager.Instance?.AddScore(scoreValue);
            GameManager.Instance?.EnterCinematic();
        }

        foreach (Collider2D col in colliders) col.enabled = false; // コライダーを無効にする
        StartCoroutine(DeathSequenceCoroutine());
    }

    private IEnumerator DeathSequenceCoroutine()
    {
        // 追加や削除ができないリスト
        IReadOnlyList<Transform> points = attackPattern != null ? attackPattern.GetFirePoints() : null;

        // 順番に爆破つさせる
        for (int i = 0; i < 3; i++)
        {
            Vector3 position = points != null && points.Count > 0 ? points[i % points.Count].position : transform.position;

            EffectManager.Instance?.Play(EffectCueId.BossDamaged, position);
            AudioManager.Instance?.PlaySe(SeId.BossDamage);
            yield return new WaitForSecondsRealtime(deathExplosionInterval); // 待つ
        }

        yield return new WaitForSecondsRealtime(1.5f);
        EffectManager.Instance?.Play(EffectCueId.BossDamaged, transform.position, 4.0f);
        AudioManager.Instance?.PlaySe(SeId.BossDestroyed);

        // 壊れたボスに変更
        if (spriteRenderer != null)
        {
            if (brokenBossSprite != null) spriteRenderer.sprite = brokenBossSprite;
            else spriteRenderer.color = new Color(0.35f, 0.35f, 0.35f, 1.0f);
        }

        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * deathFallDistance;
        float elapsed = 0.0f;
        float smokeTimer = 0.0f;

        // ボスを下に移動させながら煙を出す
        while (elapsed < deathFallDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / deathFallDuration);

            transform.position = Vector3.Lerp(start, end, t);

            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1.0f - t;
                spriteRenderer.color = color;
            }

            smokeTimer -= Time.deltaTime;
            if (smokeTimer <= 0.0f)
            {
                GameObject smoke = EffectManager.Instance?.Play(EffectCueId.BossSmoke, transform.position);

                if (smoke != null)
                {
                    smoke.transform.SetParent(transform, true);
                }

                smokeTimer = smokeInterval;
            }

            yield return null;
        }

        yield return new WaitForSecondsRealtime(1.0f);
        GameManager.Instance?.GameClear();
    }
}
