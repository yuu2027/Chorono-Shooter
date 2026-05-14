using UnityEngine;

// Rigidbody2DとCollider2Dを必ず使用する。unityが自動的に追加してくれる
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerBullet : BulletBase
{
    // Trigger設定されたColliderに接触したときにUnityから自動で呼ばれる関数
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        EnemyBase enemy = other.GetComponent<EnemyBase>();
        if (enemy == null) return;

        enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}
