using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D rig;
    private Vector2 direction;
    private bool alive = false;
    public float speed;
    public Stat damage;
    public float lifeTime;

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (alive)
            rig.linearVelocity = direction * speed;
    }

    public void Shoot(float speed, Vector2 direction, Stat damage)
    {
        this.speed = speed;
        this.damage = damage;
        this.direction = direction.normalized;
        alive = true;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        Invoke(nameof(DestroyViaLifetime), lifeTime);
    }

    public void DestroyViaLifetime()
    {
        alive = false;
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet colidiu com um objeto! " + other.gameObject.name);
        
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<Enemy>(out var e))
                e.TakeDamage(damage.Value);
            else if (other.TryGetComponent<Enemy1>(out var e1))
                e1.TakeDamage(damage.Value);
            else if (other.TryGetComponent<Enemy2>(out var e2))
                e2.TakeDamage(damage.Value);
            Destroy(gameObject);
        }
    }
}
