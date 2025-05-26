using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("General")]
    public TowerBuilder towerBuilder;
    private PlayerInput playerInput;
    private PlayerStats playerStats;
    public bool inMenu;
    public bool isEnabled = false;
    public GameObject inGameCash;
    public GameObject upgradeUIObject;
    [SerializeField] Color cheapColor;
    [SerializeField] Color expensiveColor;

    [Header("Elements")]
    public Image imageIcon;
    public TextMeshProUGUI upgradeCost;
    public TextMeshProUGUI oldStat;
    public TextMeshProUGUI newStat;
    [SerializeField] private TextMeshProUGUI upgradeAmaunt;
    [SerializeField] private TextMeshProUGUI upgradeDescription;
    [SerializeField] private TextMeshProUGUI cashTxt;
    [SerializeField] private List<ButtonUpgrade> upgradesInfos;
    public ButtonUpgrade currentButton;

    public void Initialize(TowerBuilder towerBuilder)
    {
        this.towerBuilder = towerBuilder;
        playerInput = towerBuilder.gameObject.GetComponent<PlayerInput>();
        playerStats = towerBuilder.gameObject.GetComponent<PlayerStats>();
        towerBuilder.playerController.SetUpUpgradeUI(this);
        towerBuilder.upgradeUI = this;
        foreach (var item in upgradesInfos)
        {
            item.Setup(this);
        }
        currentButton = upgradesInfos[0];
        playerInput.actions["Menu"].performed += _ => Menu();
        isEnabled = true;
    }

    void OnDisable()
    {
        playerInput.actions["Menu"].performed -= _ => Menu();
    }

    void Menu()
    {
        if (!isEnabled) return;
        if (towerBuilder.playerController.isBuilding) return;
        inMenu = !inMenu;
        if (inMenu)
            UpdateStat();
    }

    void Update()
    {
        if (!isEnabled) return;
        upgradeUIObject.SetActive(inMenu);
        inGameCash.SetActive(!inMenu);
        ShowStat();
    }

    void ShowStat()
    {
        imageIcon.sprite = currentButton.icon;
        upgradeCost.text = $"Cost = R${currentButton.cost}";
        upgradeCost.color = (currentButton.cost > towerBuilder.cash) ? expensiveColor : cheapColor;
        oldStat.text = currentButton.stat.Value.ToString();
        newStat.text = (currentButton.stat.Value + currentButton.defaultIncrease).ToString();
        upgradeAmaunt.text = $"+{currentButton.defaultIncrease}";
        upgradeDescription.text = currentButton.description;
        cashTxt.text = $"Cash: R${towerBuilder.cash},00 ";
    }

    public void UpgradeStat(int index)
    {
        if (towerBuilder.cash < currentButton.cost) return;
        towerBuilder.cash -= currentButton.cost;
        currentButton.cost *= 2;

        if (index <= 3 || index == 11)
        {
            currentButton.stat.AddModifier(currentButton.defaultIncrease);
            if (index == 11)
                towerBuilder.safeZone.SetRadius(towerBuilder.safeZoneRadius.Value);
        }
        else
            UpgradeTowers(index);
        
        currentButton.defaultIncrease *= 1.4f;
    }

    void UpgradeTowers(int upIndex)
    {
        var towers = GameObject.FindGameObjectsWithTag("Tower");
        foreach (GameObject towerGO in towers)
        {
            if (!towerGO.TryGetComponent<Build>(out var tower))
            {
                Debug.LogWarning($"Objeto {towerGO.name} está tagueado como Tower mas não tem componente Tower.");
                continue;
            }
            if (upIndex == 5 || upIndex == 7 || upIndex == 9)
                tower.MaxLife.AddModifier(currentButton.defaultIncrease);
            else
            {
                AttackBuild atk = tower as AttackBuild;
                DefenseBuild dfs = tower as DefenseBuild;
                GeneratorBuild gnr = tower as GeneratorBuild;
                
                switch (upIndex)
                {
                    case 4:
                        if (atk != null)
                            atk.damage.AddModifier(currentButton.defaultIncrease);
                        break;
                    case 6:
                        if (atk != null)
                            atk.projectileSpeed.AddModifier(currentButton.defaultIncrease);
                        break;
                    case 8:
                        if (dfs != null)
                            dfs.regenAmaunt.AddModifier(currentButton.defaultIncrease);
                        break;
                    case 10:
                        if (gnr != null)
                        {
                            gnr.generationAmaunt.AddModifier(currentButton.defaultIncrease);
                            currentButton.cost *= 3 / 2;
                        }
                        break;
                }
            }
        }
    }
    void UpdateStat()
    {
        upgradesInfos[0].stat = playerStats.Damage;
        upgradesInfos[1].stat = playerStats.MaxHealth;
        upgradesInfos[2].stat = playerStats.AttackSpeed;
        upgradesInfos[3].stat = playerStats.Regeneration;

        upgradesInfos[4].stat = (towerBuilder.enabledBuilds[BuildType.ATTACK][0] as AttackBuild).damage;
        upgradesInfos[5].stat = towerBuilder.enabledBuilds[BuildType.ATTACK][0].MaxLife;
        upgradesInfos[6].stat = (towerBuilder.enabledBuilds[BuildType.ATTACK][0] as AttackBuild).projectileSpeed;

        upgradesInfos[7].stat = towerBuilder.enabledBuilds[BuildType.DEFENSE][0].MaxLife;
        upgradesInfos[8].stat = (towerBuilder.enabledBuilds[BuildType.DEFENSE][0] as DefenseBuild).regenAmaunt;
        upgradesInfos[9].stat = towerBuilder.enabledBuilds[BuildType.GENERATION][0].MaxLife;
        upgradesInfos[10].stat = (towerBuilder.enabledBuilds[BuildType.GENERATION][0] as GeneratorBuild).generationAmaunt;

        upgradesInfos[11].stat = towerBuilder.safeZoneRadius;
    }
}
