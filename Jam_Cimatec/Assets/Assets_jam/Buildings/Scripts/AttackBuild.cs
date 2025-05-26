using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[Serializable]
public class AttackBuild : Build
{
    private AudioSource audioSource;
    public Stat damage;
    public Stat projectileSpeed;
    public float atkRadius;
    public float atkTime;
    [SerializeField] private float originRadius;
    public GameObject projectile;
    public LayerMask enemyLayer;
    [SerializeField] private AudioClip shootSfx;

    public override void Start()
    {
        base.Start();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = shootSfx;
        StartCoroutine(nameof(AtkLoop));
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public override void DeleteBuild()
    {
        animator.SetTrigger("die");
        base.DeleteBuild();
    }

    IEnumerator AtkLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(atkTime);
            var targets = Physics2D.OverlapCircleAll(transform.position, atkRadius, enemyLayer);
            if (targets.Length > 0)
            {
                Vector2 target = NearestTargetPos(targets);
                Vector2 dir = ((Vector3)target - transform.position).normalized;

                float angle = Mathf.Atan2(-dir.y, dir.x) * Mathf.Rad2Deg;
                angle = (angle + 360f + 22.5f) % 360f;
                int directionIndex = Mathf.FloorToInt(angle / (360f / 8));
                animator.SetInteger("DirectionIndex", directionIndex);

                Vector2 origin = (Vector2)transform.position + originRadius * dir;
                var proj = Instantiate(projectile, origin, Quaternion.identity);
                proj.GetComponent<Projectile>().Shoot(projectileSpeed.Value, dir, damage);
                audioSource.Play();
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
        Gizmos.DrawWireSphere(transform.position, atkRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, originRadius);
    }
}
