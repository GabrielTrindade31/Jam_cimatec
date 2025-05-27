using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpawnEnemys : MonoBehaviour
{
    public static SpawnEnemys Instance;

    [System.Serializable]
    public class EnemyConfig
    {
        [Tooltip("Prefab of this enemy type")]
        public GameObject prefab;
        [Tooltip("Wave at which this enemy first appears (1-based)")]
        public int startWave = 1;
        [Tooltip("How many of this enemy to spawn on its first wave")]
        public int baseCount = 1;
        [Tooltip("How many extra per wave after startWave")]
        public int incrementPerWave = 0;
    }

    [Header("Enemy Types")]
    [Tooltip("Configure each enemy type here")]
    public List<EnemyConfig> enemyConfigs = new List<EnemyConfig>();

    [Header("Wave Settings")]
    public float spawnInterval    = 1f;
    public float timeBetweenWaves = 5f;
    public bool  startWave        = false;

    public int  currentWave = 1;
    private int  enemiesToSpawn;
    private int  enemiesAlive;
    private bool isSpawning;

    void Awake() => Instance = this;

    void Update()
    {
        if (startWave && !isSpawning)
        {
            startWave = false;
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;

        // 1) build a list of prefab references to spawn this wave
        var spawnList = new List<GameObject>();
        foreach (var cfg in enemyConfigs)
        {
            if (currentWave < cfg.startWave) 
                continue;

            int waveOffset = currentWave - cfg.startWave;
            int count = cfg.baseCount + waveOffset * cfg.incrementPerWave;
            for (int i = 0; i < count; i++)
                spawnList.Add(cfg.prefab);
        }

        // track how many we will spawn
        enemiesToSpawn = spawnList.Count;
        enemiesAlive   = 0;

        // 2) shuffle for randomness
        for (int i = 0; i < spawnList.Count; i++)
        {
            int j = Random.Range(i, spawnList.Count);
            var tmp = spawnList[i];
            spawnList[i] = spawnList[j];
            spawnList[j] = tmp;
        }

        // 3) spawn one by one
        foreach (var prefab in spawnList)
        {
            // pick a random spawn point
            var pt = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(prefab, pt.position, Quaternion.identity);
            enemiesAlive++;
            yield return new WaitForSeconds(spawnInterval);
        }

        // 4) wait until all are killed
        isSpawning = false;
        yield return new WaitUntil(() => enemiesAlive <= 0);

        // 5) prepare next wave
        currentWave++;
        yield return new WaitForSeconds(timeBetweenWaves);
    }

    public void EnemyKilled()
    {
        enemiesAlive--;
    }

    /// <summary>
    /// Call this to abort any ongoing spawns and reset all counters.
    /// </summary>
    public void ResetToStart()
    {
        StopAllCoroutines();
        currentWave = 1;
        enemiesAlive = 0;
        enemiesToSpawn = 0;
        isSpawning = false;
        startWave = false;
    }
     public void OnWaveButtonPressed()
    {
        // dispara a próxima wave
        if (SpawnEnemys.Instance != null)
            SpawnEnemys.Instance.startWave = true;
    }

    [Header("Spawn Points")]
    public Transform[] spawnPoints;
}
