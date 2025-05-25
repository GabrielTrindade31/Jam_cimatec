using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerInput), typeof(PlayerController))]
public class TowerBuilder : MonoBehaviour
{
    public Tilemap tilemap;
    public Stat safeZoneRadius;
    [HideInInspector] public UpgradeUI upgradeUI;
    [SerializeField] private List<BuildEntry> buildSet = new();
    [SerializeField] private TextMeshProUGUI textMesh;
    public BuildType selectedBuildType;
    public int buildIndex;
    public int cash;
    public float tunrPerClick = 22.5f;
    public float currentRotation;

    public Dictionary<BuildType, List<Build>> enabledBuilds;
    private readonly Dictionary<Vector2Int, GameObject> notEmptySpaces = new();
    private PlayerInput input;
    [HideInInspector] public PlayerController playerController;
    private GameObject currentGhost;
    private Build CurrentBuild => enabledBuilds[selectedBuildType][buildIndex];

    private float lastScrollTime;
    private readonly float scrollCooldown = 0.15f;
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        playerController = GetComponent<PlayerController>();
        SetupBuilds();
    }

    void SetupBuilds()
    {
        enabledBuilds = new Dictionary<BuildType, List<Build>>();

        foreach (var entry in buildSet)
        {
            if (!enabledBuilds.ContainsKey(entry.buildType))
                enabledBuilds[entry.buildType] = new List<Build>();

            enabledBuilds[entry.buildType].AddRange(entry.constructions);
        }
    }

    void OnEnable()
    {
        input.actions["Previous"].performed += _ => PreviousBuildClass();
        input.actions["Next"].performed += _ => NextBuildClass();
        input.actions["Jump"].performed += _ => ChangeBuildMode();
        input.actions["Fire"].performed += _ => TryBuild();
        input.actions["Rotate"].performed += _ => Rotate();
        input.actions["Remove"].performed += _ => TryDestroyBuild();
    }

    void OnDisable()
    {
        input.actions["Previous"].performed -= _ => PreviousBuildClass();
        input.actions["Next"].performed -= _ => NextBuildClass();
        input.actions["Jump"].performed -= _ => ChangeBuildMode();
        input.actions["Fire"].performed -= _ => TryBuild();
        input.actions["Rotate"].performed -= _ => Rotate();
        input.actions["Remove"].performed += _ => TryDestroyBuild();
    }

    void Update()
    {
        textMesh.text = $"Cash: R${cash},00";
        if (playerController.isBuilding && !upgradeUI.inMenu)
            UpdateGhostPosition();
        else
            currentGhost = null;

        HandleScrollInput();
    }

    void HandleScrollInput()
    {
        if (Time.time - lastScrollTime < scrollCooldown) return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0f)
        {
            NextBuilding();
            lastScrollTime = Time.time;
        }
        else if (scroll < 0f)
        {
            PreviousBuilding();
            lastScrollTime = Time.time;
        }
    }

    void Rotate()
    {
        currentRotation += tunrPerClick;
    }

    void ChangeBuildMode()
    {
        playerController.isBuilding = !playerController.isBuilding;
        if (playerController.isBuilding && !upgradeUI.inMenu)
        {
            if (currentGhost != null)
                Destroy(currentGhost);

            currentGhost = CurrentBuild.CreateGhostInstance(0f);
        }
        else if (currentGhost != null)
            Destroy(currentGhost);
    }

    void PreviousBuildClass()
    {
        switch (selectedBuildType)
        {
            case BuildType.ATTACK:
                selectedBuildType = BuildType.GENERATION;
                break;
            case BuildType.GENERATION:
                selectedBuildType = BuildType.DEFENSE;
                break;
            case BuildType.DEFENSE:
                selectedBuildType = BuildType.ATTACK;
                break;
        }
        SelectBuild(0);
    }

    void NextBuildClass()
    {
        switch (selectedBuildType)
        {
            case BuildType.ATTACK:
                selectedBuildType = BuildType.DEFENSE;
                break;
            case BuildType.GENERATION:
                selectedBuildType = BuildType.ATTACK;
                break;
            case BuildType.DEFENSE:
                selectedBuildType = BuildType.GENERATION;
                break;
        }
        SelectBuild(0);
    }

    void PreviousBuilding()
    {
        int count = enabledBuilds[selectedBuildType].Count;
        int newIndex = (buildIndex - 1 + count) % count;
        SelectBuild(newIndex);
    }

    void NextBuilding()
    {
        int count = enabledBuilds[selectedBuildType].Count;
        int newIndex = (buildIndex + 1) % count;
        SelectBuild(newIndex);
    }

    void SelectBuild(int index)
    {
        if (index >= 0 && index < enabledBuilds[selectedBuildType].Count)
        {
            buildIndex = index;

            if (currentGhost != null)
                Destroy(currentGhost);

            if (playerController.isBuilding && !upgradeUI.inMenu)
                currentGhost = CurrentBuild.CreateGhostInstance(0f);
        }
    }

    void UpdateGhostPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;

        Vector2Int gridPos = World2Grid(mousePos);
        Vector3 ghostPos = Grid2World(gridPos);

        currentGhost.transform.position = ghostPos;
        if (CurrentBuild.canRotate)
            currentGhost.transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        else
            currentGhost.transform.rotation = Quaternion.identity;
    }

    public Vector2Int World2Grid(Vector3 worldPos)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        return new Vector2Int(cellPos.x, cellPos.y);
    }

    Vector3 Grid2World(Vector2Int gridPos)
    {
        return tilemap.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
    }

    public void RemoveBuildFromMap(Vector2Int position)
    {
        if (notEmptySpaces.ContainsKey(position))
        {
            int cashBack = (int)Mathf.Ceil(notEmptySpaces[position].GetComponent<Build>().cost / 10);
            cash += cashBack;
            Destroy(notEmptySpaces[position]);
            notEmptySpaces.Remove(position);
        }
    }

    void TryDestroyBuild()
    {
        if (playerController.isBuilding && !upgradeUI.inMenu)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;
            Vector2Int gridPos = World2Grid(mousePos);
            RemoveBuildFromMap(gridPos);
        }
    }

    void TryBuild()
    {
        if (playerController.isBuilding && !upgradeUI.inMenu && cash >= CurrentBuild.cost)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;
            Vector2Int gridPos = World2Grid(mousePos);

            if (notEmptySpaces.ContainsKey(gridPos)) return;

            Vector3 finalPosition = Grid2World(gridPos);

            if (finalPosition.magnitude > safeZoneRadius.Value) return;

            cash -= CurrentBuild.cost;
            float buildRotation = CurrentBuild.canRotate ? currentRotation : 0f;
            GameObject newTower = CurrentBuild.BuildIn(finalPosition, buildRotation, this);
            notEmptySpaces[gridPos] = newTower;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Vector3.zero, safeZoneRadius.Value);
    }
}