
using UnityEngine;

public class EnemyStats : StatsEntitys
{
    protected override void Die()
    {
        SpawnEnemys.Instance.EnemyKilled();
        Destroy(gameObject);
    }
}
