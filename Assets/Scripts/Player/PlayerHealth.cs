using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerController))]
public class PlayerHealth : MonoBehaviour
{
    [Serializable]
    public class HealthChangedEvent : UnityEvent<int, int> { }

    [Header("Health Settings")]
    [SerializeField] private int maxHp = 5;               // プレイヤーのHP
    [SerializeField] private float invincibleTime = 1.0f; // 無敵タイム

    [Header("Contact Damage")]
    [SerializeField] private int contactDamage = 1; // 敵との接触によるダメージ量

    [Header("Death Settings")]
    [SerializeField] private bool disablePlayerControllerOnDeath = true; // 死亡時にプレイヤーコントローラを無効

    [Header("Events")]
    [SerializeField] private HealthChangedEvent onHealthChanged = new HealthChangedEvent(); // HPが変わったときに実行するUnityEventの変数
    [SerializeField] private UnityEvent onDamaged = new UnityEvent();                       // プレイヤーがダメージを受けたときのイベント
    [SerializeField] private UnityEvent onDied = new UnityEvent();                          // プレイヤーが死亡したときのイベント

    private int currentHp;                     // 現在のHP
    private bool isInvincible;                 // 無敵フラグ
    private bool isDead;                       // 死亡フラグ
    private Coroutine invincibleCoroutine;     // 現在実行中の無敵時間コルーチンを覚えておくための変数
    private PlayerController playerController; // PlayerController用変数

    public int CurrentHp => currentHp;         // 読み取り専用のプロパティ
    public int MaxHp => Mathf.Max(1, maxHp); // 読み取り専用のプロパティ
    public bool IsInvincible => isInvincible;  // 読み取り専用のプロパティ

    public event Action<int, int> HealthChanged;
    public event Action Damaged;
    public event Action Died;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        currentHp = maxHp;
    }

    private void Start()
    {
        NotifyHealthChanged();
    }

    // ダメージ量を計算して死亡か確認
    public void TakeDamage(int damage)
    {
        //Debug.Log(
        //$"TakeDamage called. damage = {damage}, CurrentHp = {CurrentHp}\n" +
        //StackTraceUtility.ExtractStackTrace(),
        //this
        //);

        if (isDead) return;
        if(isInvincible) return;
        if (damage <= 0) return;

        currentHp = Mathf.Max(currentHp - damage, 0);
        NotifyHealthChanged(); // イベントを通知
        onDamaged.Invoke();    // UnityEventに登録されている処理を実行
        Damaged?.Invoke();     // C#イベントに登録されている関数を実行

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        StartInvincible(); // 無敵にする
    }

    // 死亡か確認用関数
    public bool IsDead()
    {
        return isDead;
    }

    // 無敵処理
    private void StartInvincible()
    {
        if (invincibleTime <= 0.0f) return;

        if (invincibleCoroutine != null)
        {
            StopCoroutine(invincibleCoroutine);
        }

        invincibleCoroutine = StartCoroutine(InvincibleCoroutine());
    }

    // 無敵時間を管理
    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;
        invincibleCoroutine = null;
    }

    // 死亡処理
    private void Die()
    {
        if (isDead) return; // すでに死亡なら終了

        isDead = true;
        isInvincible = false;

        if(invincibleCoroutine != null)
        {
            StopCoroutine(invincibleCoroutine);
            invincibleCoroutine = null;
        }

        // プレイヤーのコントロールが無効⋀playerControllerがnullではない場合
        if(disablePlayerControllerOnDeath && playerController != null)
        {
            playerController.enabled = false; // PlayerControllerコンポーネントを無効化
        }

        onDied.Invoke();
        Died?.Invoke();

        Debug.Log("Game Over", this);

        Destroy(gameObject);
    }

    // HPが変わったことを外部へ通知する
    private void NotifyHealthChanged()
    {
        onHealthChanged.Invoke(currentHp, maxHp);
        HealthChanged?.Invoke(currentHp, maxHp);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryApplyContactDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryApplyContactDamage(other);
    }

    // 敵と接触したときにダメージを受ける
    private void TryApplyContactDamage(Collider2D other)
    {
        if (other == null) return;
        if (other.GetComponentInParent<EnemyBase>() == null) return; // 子オブジェクトのコンポーネントまで探せる

        TakeDamage(contactDamage);
    }
}
