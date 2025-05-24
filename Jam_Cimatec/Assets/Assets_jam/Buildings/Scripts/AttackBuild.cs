using System.Collections;
using UnityEngine;

public class AttackBuild : Build
{
    public int damage;
    public float atkRadius;
    public float atkTime;
    [SerializeField] private float originRadius;
    public GameObject projectile;
    public LayerMask enemyLayer;

    void Start()
    {
        StartCoroutine(nameof(AtkLoop));
    }

    void OnDestroy()       
    {
        StopAllCoroutines();
    }

    IEnumerator AtkLoop()
    {
        yield return new WaitForSeconds(atkTime);
        var targets = Physics2D.OverlapCircleAll(transform.position, atkRadius, enemyLayer);
        if (targets.Length > 0)
        {
            Vector2 target = NearestTargetPos(targets);
            Vector2 origin = (Vector2)transform.position + originRadius*target.normalized;
            var proj = Instantiate(projectile, origin, Quaternion.identity);
            proj.GetComponent<Projectile>().Shoot(target, damage);
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
}
