using System.Collections;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance;

    [Tooltip("Where the player should spawn (PowerCore)")]
    public Transform spawnPoint;

    private GameObject currentPlayer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // first spawn at start
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
    }

    public void OnPlayerDeath()
    {
        // wait 10s, then respawn
        StartCoroutine( RespawnAfterDelay() );
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(10f);
        SpawnPlayer();
    }
}
