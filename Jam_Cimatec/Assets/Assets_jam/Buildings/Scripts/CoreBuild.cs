using System.Collections;
using UnityEngine;

public class CoreBuild : Build
{
    public Stat regenAmaunt;
    public float regenCooldown;
    private TowerBuilder tw;
    public Stat generationAmaunt;
    public float generationCooldown;
    void Awake()
    {
        StartCoroutine(nameof(RegenerationLoop));
        StartCoroutine(nameof(GenerationLoop));
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void Initialize(TowerBuilder origin)
    {
        tw = origin;
    }

    public override GameObject BuildIn(Vector3 position, float rotation, TowerBuilder tw)
    {
        Quaternion rot = canRotate ? Quaternion.Euler(0, 0, rotation) : Quaternion.identity;
        GameObject newBuild = Instantiate(prefab, position, rot);
        newBuild.GetComponent<GeneratorBuild>().Initialize(tw);
        return newBuild;
    }

    IEnumerator GenerationLoop()
    {
        while (currentLife > 0)
        {
            yield return new WaitForSeconds(generationCooldown);
            tw.cash += (int)Mathf.Ceil(generationAmaunt.Value);
        }
    }

    IEnumerator RegenerationLoop()
    {
        while (currentLife > 0)
        {
            yield return new WaitForSeconds(regenCooldown);
            float increase = currentLife + regenAmaunt.Value;
            currentLife = (increase > MaxLife.Value) ? MaxLife.Value : increase;
        }
    }
}
