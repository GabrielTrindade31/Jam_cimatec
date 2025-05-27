using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDmanager : MonoBehaviour
{
    public PlayerStats playerStats;
    public TowerBuilder towerBuilder;
    public SpawnEnemys spawnEnemys;
    public Slider hpBar;
    public TextMeshProUGUI cashTxt;
    public TextMeshProUGUI waveTxt;
    public bool active;

    void Start()
    {
        towerBuilder = GameObject.FindWithTag("Player").GetComponent<TowerBuilder>();
        playerStats = towerBuilder.gameObject.GetComponent<PlayerStats>();
        spawnEnemys = GameObject.FindWithTag("SpawnEnemies").GetComponent<SpawnEnemys>();
        towerBuilder.SetupHUD(this);
    }

    void Update()
    {
        hpBar.maxValue = playerStats.MaxHealth.Value;
        hpBar.value = playerStats.CurrentHealth;
        cashTxt.text = $"Cash: R${towerBuilder.cash}";
        waveTxt.text = $"Wave: {spawnEnemys.currentWave}";
    }
}
