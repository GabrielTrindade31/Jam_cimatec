using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GeneratorBuild : Build
{
    private TowerBuilder tw;
    public Stat generationAmaunt;
    public float generationCooldown;

    public override GameObject BuildIn(Vector3 position, float rotation, TowerBuilder tw)
    {
        this.tw = tw;
        Quaternion rot = canRotate ? Quaternion.Euler(0, 0, rotation) : Quaternion.identity;
        GameObject newBuild = Instantiate(prefab, position, rot);
        return newBuild;
    }

    void Start()
    {
        StartCoroutine(nameof(GenerationLoop));
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    IEnumerator GenerationLoop()
    {
        yield return new WaitForSeconds(generationCooldown);
        tw.cash += (int)Mathf.Ceil(generationAmaunt.Value);
    }
}
