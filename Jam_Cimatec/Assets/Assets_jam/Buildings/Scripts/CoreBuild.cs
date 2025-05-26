using System.Collections;
using UnityEngine;

public class CoreBuild : Build
{
    public Stat regenAmaunt;
    public float regenCooldown;
    public Stat generationAmaunt;
    public float generationCooldown;
    void Awake()
    {
        currentLife = MaxLife.Value;
        tw = FindAnyObjectByType<TowerBuilder>();
        StartCoroutine(nameof(RegenerationLoop));
        StartCoroutine(nameof(GenerationLoop));
    }

    void OnDestroy()
    {
        StopAllCoroutines();
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
