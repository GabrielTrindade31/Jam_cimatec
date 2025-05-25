using System.Collections;
using UnityEngine;

public class GeneratorBuild : Build
{
    public Stat generationAmaunt;
    public float generationCooldown;

    void Awake()
    {
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
}
