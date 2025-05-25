using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class AttackBuild : Build
{
    public Stat damage;
    public Stat atkRadius;
    public Stat projectileSpeed;
    public float atkTime;
    [SerializeField] private float originRadius;
    public GameObject projectile;
    public LayerMask enemyLayer;

    public override void Start()
    {
        base.Start();
        StartCoroutine(nameof(AtkLoop));
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator AtkLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(atkTime);
            var targets = Physics2D.OverlapCircleAll(transform.position, atkRadius.Value, enemyLayer);
            if (targets.Length > 0)
            {
                Vector2 target = NearestTargetPos(targets);
                Vector2 dir = ((Vector3)target - transform.position).normalized;
                Vector2 origin = (Vector2)transform.position + originRadius * dir;
                var proj = Instantiate(projectile, origin, Quaternion.identity);
                proj.GetComponent<Projectile>().Shoot(projectileSpeed.Value, dir, damage);
            }
        }
    }

    Vector2 NearestTargetPos(Collider2D[] objects)
    {
        float minDistance = Vector3.Distance(transform.position, objects[0].transform.position);
        Vector3 target = objects[0].transform.position;
        foreach (var obj in objects)
        {
            float newDistance = Vector3.Distance(transform.position, obj.transform.position);
            if (newDistance < minDistance)
            {
                minDistance = newDistance;
                target = obj.transform.position;
            }
        }
        return (Vector2)target;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, atkRadius.Value);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, originRadius);
    }
}
