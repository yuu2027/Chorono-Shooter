using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float defaultSpeed = 12.0f;
    [SerializeField] private float defaultLifeTime = 3.0f;

    private Rigidbody2D rb;
    private Collider2D[] myColliders;
    private Vector2 moveDirection = Vector2.up;
    private float moveSpeed;
    bool initialized;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myColliders = GetComponentsInChildren<Collider2D>();

        rb.gravityScale = 0.0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        moveSpeed = defaultSpeed;
    }

    private void Start()
    {
        if (!initialized) Initialize(transform.up, defaultSpeed, defaultLifeTime, null);
    }

    public void Initialize(Vector2 direction,float speed,float lifeTime, Collider2D[] ownerColliders)
    {
        moveDirection = direction.sqrMagnitude > 0.0f ? direction.normalized : Vector2.up;
        moveSpeed = speed > 0.0f ? speed : defaultSpeed;
        initialized = true;

        float finalLifeTime = lifeTime > 0.0f ? lifeTime : defaultLifeTime;
        Destroy(gameObject, finalLifeTime);

        if (ownerColliders != null) return;

        for(int i = 0; i < ownerColliders.Length; i++)
        {
            Collider2D owner = ownerColliders[i];
            if(owner == null) continue;

            for(int j = 0; j < myColliders.Length; j++)
            {
                Collider2D mine = myColliders[j];
                if(mine == null) continue;
                Physics2D.IgnoreCollision(mine, owner, true);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!initialized) return;

        Vector3 delta = (Vector3)(moveDirection * moveSpeed * Time.fixedDeltaTime);
        transform.position += delta;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}
