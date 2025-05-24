using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerInput), typeof(PlayerController))]
public class TowerBuilder : MonoBehaviour
{
    public Tilemap tilemap;
    [SerializeField] private List<BuildEntry> buildSet = new();
    public BuildType selectedBuildType;
    public int buildIndex;
    public int cash;
    private Dictionary<BuildType, List<Build>> enabledBuilds;
    private Dictionary<Vector2Int, GameObject> notEmptySpaces = new();
    private PlayerInput input;
    private PlayerController playerController;
    private GameObject currentGhost;
    private Build CurrentBuild => enabledBuilds[selectedBuildType][buildIndex];
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
        input.actions["Previous"].performed += _ => PreviousBuilding();
        input.actions["Next"].performed += _ => NextBuilding();
        input.actions["Jump"].performed += _ => ChangeBuildMode();
        input.actions["Fire"].performed += _ => TryBuild();
    }

    void OnDisable()
    {
        input.actions["Previous"].performed -= _ => PreviousBuilding();
        input.actions["Next"].performed -= _ => NextBuilding();
        input.actions["Jump"].performed -= _ => ChangeBuildMode();
        input.actions["Fire"].performed -= _ => TryBuild();
    }

    void Update()
    {
        if (playerController.isBuilding)
            UpdateGhostPosition();
        else
            currentGhost = null;   
    }

    void ChangeBuildMode()
    {
        playerController.isBuilding = !playerController.isBuilding;
        if (playerController.isBuilding)
        {
            if (currentGhost != null)
                Destroy(currentGhost);

            currentGhost = CurrentBuild.CreateGhostInstance();
        }
        else if (currentGhost != null)
            Destroy(currentGhost);
    }

    void PreviousBuilding()
    {
        int count = enabledBuilds[selectedBuildType].Count;
        int newIndex = (buildIndex - 1 + count) % count;
        SelectBuild(newIndex);
        SelectBuild(buildIndex - 1);
    }

    void NextBuilding()
    {
        int count = enabledBuilds[selectedBuildType].Count;
        int newIndex = (buildIndex + 1) % count;
        SelectBuild(newIndex);
        SelectBuild(buildIndex + 1);
    }

    void SelectBuild(int index)
    {
        if (index >= 0 && index < enabledBuilds[selectedBuildType].Count)
        {
            buildIndex = index;

            if (currentGhost != null)
                Destroy(currentGhost);
            
            if (playerController.isBuilding)
                currentGhost = CurrentBuild.CreateGhostInstance();

            if (playerController.isBuilding)
                currentGhost = Instantiate(enabledBuilds[selectedBuildType][buildIndex].CreateGhost());
        }
    }

    void UpdateGhostPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;

        Vector2Int gridPos = World2Grid(mousePos);
        Vector3 ghostPos = Grid2World(gridPos);
        Vector2Int gridPos = World2Grid(mousePos);
        Vector3 ghostPos = Grid2World(gridPos);
        if (playerController.isBuilding && currentGhost == null)
            SelectBuild(buildIndex);

        currentGhost.transform.position = ghostPos;
    }

    Vector2Int World2Grid(Vector3 worldPos)
    {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        return new Vector2Int(cellPos.x, cellPos.y);
    }

    Vector3 Grid2World(Vector2Int gridPos)
    {
        return tilemap.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
    }

    void TryBuild()
    {
        if (playerController.isBuilding && cash >= CurrentBuild.cost)
        if (playerController.isBuilding)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;
            Vector2Int gridPos = World2Grid(mousePos);

            if (notEmptySpaces.ContainsKey(gridPos)) return;

            Vector3 finalPosition = Grid2World(gridPos);
            cash -= CurrentBuild.cost;
            GameObject newTower = CurrentBuild.BuildIn(finalPosition);
            GameObject newTower = Instantiate(enabledBuilds[selectedBuildType][buildIndex].prefab, finalPosition, Quaternion.identity);
            notEmptySpaces[gridPos] = newTower;
        }
    }
}
