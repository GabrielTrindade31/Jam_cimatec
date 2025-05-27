using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
   public float speed = 10f;
    public float Damage { get; private set; }

    void Start()
    {
        Destroy(gameObject, 3f);
        var bullets = GameObject.FindGameObjectsWithTag("Bullet");
        if (bullets.Length >= 10)
            Destroy(bullets[0]);
    }

    public void Init(float dmg)
    {
        Damage = dmg;
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Bullet colidiu com um objeto! " + other.gameObject.name);
        
        if (other.CompareTag("Enemy"))
        {
            var e = other.GetComponent<Enemy>();
            if (e != null)
                e.TakeDamage(Damage);
            if (other.TryGetComponent<Enemy1>(out var e1))
            e1.TakeDamage(Damage);
            else if (other.TryGetComponent<Enemy2>(out var e2))
            e2.TakeDamage(Damage);
            Destroy(gameObject);
        }
    }
}
