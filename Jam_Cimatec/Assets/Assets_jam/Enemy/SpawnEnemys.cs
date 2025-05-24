using System.Collections;
using UnityEngine;

public class SpawnEnemys : MonoBehaviour
{
    public static SpawnEnemys Instance;
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int baseEnemies = 5;
    public int waveIncrement = 2;
    public float spawnInterval = 1f;
    public float timeBetweenWaves = 5f;
    public bool startWave = false;

    private int currentWave = 1;
    private int enemiesToSpawn;
    private int enemiesAlive;
    private bool isSpawning;

    void Awake() => Instance = this;

    void Update()
    {
        if (startWave && !isSpawning)
        {
            startWave = false;
            enemiesToSpawn = baseEnemies + (currentWave - 1) * waveIncrement;
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        isSpawning = true;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            var spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
            enemiesAlive++;
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
        yield return new WaitUntil(() => enemiesAlive <= 0);
        currentWave++;
        yield return new WaitForSeconds(timeBetweenWaves);
    }

    public void EnemyKilled() => enemiesAlive--;
}
