// Enemy.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public float maxHealth = 50f;
    public float speed = 2f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        var core = GameObject.FindWithTag("PowerCore");
        if (core != null)
        {
            var dir = (core.transform.position - transform.position).normalized;
            GetComponent<Rigidbody2D>().linearVelocity = dir * speed;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        SpawnEnemys.Instance.EnemyKilled();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tower") || other.CompareTag("PowerCore"))
        {
            var stats = other.GetComponent<StatsEntitys>();
            if (stats != null)
                stats.TakeDamage(GetComponent<EnemyStats>().Damage.Value);
        }
    }
}
