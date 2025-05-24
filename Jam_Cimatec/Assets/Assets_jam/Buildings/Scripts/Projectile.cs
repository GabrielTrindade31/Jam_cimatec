using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    public int damage;
    public float lifeTime;

    public void Shoot(Vector2 direction, int damage)
    {
        this.damage = damage;
        Invoke(nameof(DestroyViaLifetime), lifeTime);
    }

    public void DestroyViaLifetime()
    {
        Destroy(gameObject);
    }
}
