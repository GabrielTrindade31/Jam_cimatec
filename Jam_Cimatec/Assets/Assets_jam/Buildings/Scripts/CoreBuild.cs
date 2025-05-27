using System.Collections;
using UnityEngine;

public class CoreBuild : Build
{
    public Stat regenAmaunt;
    public float regenCooldown;
    public Stat generationAmaunt;
    public float generationCooldown;

    public override void Start()
    {
        base.Start();
        StartCoroutine(nameof(RegenerationLoop));
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

    public override void Update()
    {
        base.Update();
        if (currentLife <= 0)
            GameOverManager.Instance.ShowGameOver(tw.cash);
    }

    IEnumerator GenerationLoop()
    {
        yield return new WaitForSeconds(generationCooldown);
        tw = FindAnyObjectByType<TowerBuilder>();
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
