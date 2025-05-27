using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody2D))]
public class Enemy1 : MonoBehaviour
{
    public float maxHealth      = 50f;
    public float visionRange    = 5f;
    public float attackRange    = 1.5f;
    public float attackInterval = 1f;
    public float navSpeed       = 3f;

    private float        currentHealth;
    private float        attackTimer;
    private NavMeshAgent agent;
    private Transform    powerCore;
    private Transform    player;
    private Animator     animator;

    void Start()
    {
        currentHealth = maxHealth;
        agent         = GetComponent<NavMeshAgent>();
        animator      = GetComponent<Animator>();

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

        // 1) escolhe o alvo
        Transform target = null;
        float   bestDist = visionRange;
        foreach (var hit in Physics2D.OverlapCircleAll(transform.position, visionRange))
        {
            if (hit.CompareTag("Tower"))
            {
                float d = Vector2.Distance(transform.position, hit.transform.position);
                if (d < bestDist) { bestDist = d; target = hit.transform; }
            }
        }
        if (target == null && player != null)
        {
            float pd = Vector2.Distance(transform.position, player.position);
            if (pd <= visionRange) { bestDist = pd; target = player; }
        }
        if (target == null && powerCore != null)
        {
            float cd = Vector2.Distance(transform.position, powerCore.position);
            if (cd <= visionRange) { bestDist = cd; target = powerCore; }
        }

        // 2) persegue ou ataca
        if (target != null)
        {
            float dist = Vector2.Distance(transform.position, target.position);
            Vector2 dir = (target.position - transform.position).normalized;

            if (dist <= attackRange)
            {
                agent.isStopped = true;
                PlayAttackAnimation(dir);

                if (attackTimer <= 0f)
                {
                    // aplica dano
                    float dmg = GetComponent<EnemyStats>().Damage.Value;
                    if (target.CompareTag("Player"))
                    {
                        var ps = target.GetComponent<PlayerStats>();
                        if (ps != null) ps.TakeDamage(dmg);
                    }
                    else
                    {
                        var b = target.GetComponent<Build>();
                        if (b != null) b.TakeDamage(dmg);
                    }
                    attackTimer = attackInterval;
                }
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
                PlayWalkAnimation(agent.velocity);
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(powerCore.position);
            PlayWalkAnimation(agent.velocity);
        }
    }

    // toque o clipe de andar conforme a direção da velocidade
    void PlayWalkAnimation(Vector2 vel)
    {
        if (vel.sqrMagnitude < 0.01f) return;

        if (Mathf.Abs(vel.x) > Mathf.Abs(vel.y))
        {
            if (vel.x > 0) animator.Play("WalkRight1");
            else           animator.Play("WalkLeft1");
        }
        else
        {
            if (vel.y > 0) animator.Play("WalkUp1");
            else           animator.Play("WalkDown1");
        }
    }

    // toque o clipe de ataque conforme a direção para o alvo
    void PlayAttackAnimation(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0) animator.Play("AttackRight1");
            else           animator.Play("AttackLeft1");
        }
        else
        {
            if (dir.y > 0) animator.Play("AttackUp1");
            else           animator.Play("AttackDown1");
        }
    }

    public void TakeDamage(float amount)
    {
         Debug.Log($"[Enemy2] Took {amount} damage. CurrentHealth = {currentHealth}");
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
