using System.Collections;
using UnityEngine;

public class GeneratorBuild : Build
{
    public Stat generationAmaunt;
    public float generationCooldown;

    public override void Start()
    {
        base.Start();
        StartCoroutine(nameof(GenerationLoop));
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    public override void DeleteBuild()
    {
        animator.SetTrigger("die");
        base.DeleteBuild();
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
