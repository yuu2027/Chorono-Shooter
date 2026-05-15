using UnityEngine;

// Rigidbody2DとCollider2Dを必ず使用する。unityが自動的に追加してくれる
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBullet : BulletBase
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        playerHealth.TakeDamage(damage);
        ReturnToPool();
    }
}
