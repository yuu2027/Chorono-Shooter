using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [Header("Shoot Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shotInterval = 0.2f;
    [SerializeField] private float bulletSpeed = 12.0f;
    [SerializeField] private float bulletLifeTime = 3.0f;

    private float shotTimer;
    private Collider2D[] ownerColliders;

    private void Awake()
    {
        ownerColliders = GetComponentsInChildren<Collider2D>();
    }

    public void TickShoot()
    {
        if (shotTimer > 0.0f) shotTimer -= Time.deltaTime;
        if (shotTimer > 0.0f) return;
        if (!IsShootPressed()) return;

        Shoot();
        shotTimer = shotInterval;
    }

    private bool IsShootPressed()
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard.spaceKey.isPressed;
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("PlayerShooter: bulletPrefab Ç‹ÇΩÇÕ firePoint Ç™ñ¢ê›íËÇ≈Ç∑ÅB", this);
            return;
        }

        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        PlayerBullet bullet = bulletObject.GetComponent<PlayerBullet>();

        if (bullet == null)
        {
            Debug.LogWarning("PlayerShooter: ê∂ê¨ÇµÇΩíeÇ… PlayerBullet Ç™Ç†ÇËÇ‹ÇπÇÒÅB", bulletObject);
            Destroy(bulletObject);
            return;
        }

        bullet.Initialize(firePoint.up, bulletSpeed, bulletLifeTime, ownerColliders);
    }
}
