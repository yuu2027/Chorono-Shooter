using UnityEngine;

// Rigidbody2DとCollider2Dを必ず使用する。unityが自動的に追加してくれる
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyBullet : BulletBase
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // PlayerHealth や PlayerController に TakeDamage(int) を作ったら呼ばれる。
        // まだ無い場合でもエラーにはならず、弾だけ消える。
        other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

        Destroy(gameObject);

        //PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        //if (playerHealth == null) return;

        //playerHealth.TakeDamage(damage);
        //Destroy(gameObject);
    }


}
