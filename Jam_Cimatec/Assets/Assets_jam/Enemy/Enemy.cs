using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public float maxHealth      = 50f;
    public float visionRange    = 5f;
    public float attackRange    = 1.5f;
    public float attackInterval = 1f;
    public float navSpeed       = 3f;

    private float      currentHealth;
    private float      attackTimer;
    private NavMeshAgent agent;
    private Transform    powerCore;
    private Transform    player;

    void Start()
    {
        currentHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis   = false;
        agent.updateRotation = false;
        agent.speed          = navSpeed;

        powerCore = GameObject.FindWithTag("PowerCore").transform;
        var pObj = GameObject.FindWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        Transform target = null;
        float   bestDist = visionRange;
        var hits = Physics2D.OverlapCircleAll(transform.position, visionRange);

        // busca torre
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Tower"))
            {
                float d = Vector2.Distance(transform.position, hit.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    target   = hit.transform;
                }
            }
        }

        // se não achou torre, busca player
        if (target == null && player != null)
        {
            float pd = Vector2.Distance(transform.position, player.position);
            if (pd <= visionRange)
            {
                bestDist = pd;
                target   = player;
            }
        }

        // se nem player, vê se o core está no alcance de visão
        if (target == null && powerCore != null)
        {
            float cd = Vector2.Distance(transform.position, powerCore.position);
            if (cd <= visionRange)
            {
                bestDist = cd;
                target   = powerCore;
            }
        }

        // se tem alvo
        if (target != null)
        {
            float dist = Vector2.Distance(transform.position, target.position);
            if (dist <= attackRange)
            {
                agent.isStopped = true;
                if (attackTimer <= 0f)
                {
                    var dmg = GetComponent<EnemyStats>().Damage.Value;

                    if (target.CompareTag("Player"))
                    {
                        var ps = target.GetComponent<PlayerStats>();
                        if (ps != null) ps.TakeDamage(dmg);
                    }
                    else
                    {
                        var build = target.GetComponent<Build>();
                        if (build != null)
                            build.TakeDamage(dmg);
                    }

                    attackTimer = attackInterval;
                }
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(powerCore.position);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f) Die();
    }

    void Die()
    {
        SpawnEnemys.Instance.EnemyKilled();
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
