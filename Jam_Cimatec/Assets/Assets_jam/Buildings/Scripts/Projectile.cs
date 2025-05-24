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

    void Start()
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
        Invoke(nameof(DestroyViaLifetime), lifeTime);
    }

    public void DestroyViaLifetime()
    {
        alive = false;
        Destroy(gameObject);
    }
}
