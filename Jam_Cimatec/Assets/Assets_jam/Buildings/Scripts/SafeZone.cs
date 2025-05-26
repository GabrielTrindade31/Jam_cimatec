using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Transform))]
public class SafeZone : MonoBehaviour
{
    public float    radius = 5f;
    public Tilemap  safeMap;    // arraste aqui o SafeZoneMap
    public TileBase safeTile;   // arraste aqui o SafeTile

    private List<Vector3Int> cells = new List<Vector3Int>();

    [Range(0,1f)] public float overlayAlpha = 0.8f;

    void Start()
    {
        SetOverlayAlpha(overlayAlpha);
        UpdateZone();
    }

    public void SetOverlayAlpha(float a)
    {
        Color c = safeMap.color;
        safeMap.color = new Color(c.r, c.g, c.b, a);
    }

    public void SetRadius(float r)
    {
        radius = r;
        UpdateZone();
    }

    void UpdateZone()
    {
        // limpa o overlay antigo
        foreach (var c in cells)
            safeMap.SetTile(c, null);
        cells.Clear();

        int cr = Mathf.CeilToInt(radius / safeMap.cellSize.x);
        Vector3Int center = safeMap.WorldToCell(transform.position);

        for (int dx = -cr; dx <= cr; dx++)
        for (int dy = -cr; dy <= cr; dy++)
        {
            var c = new Vector3Int(center.x + dx, center.y + dy, center.z);
            var w = safeMap.GetCellCenterWorld(c);
            if (Vector3.Distance(w, transform.position) <= radius)
            {
                safeMap.SetTile(c, safeTile);
                cells.Add(c);
            }
        }
    }
}
