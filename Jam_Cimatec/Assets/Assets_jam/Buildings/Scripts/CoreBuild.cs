using System.Collections;
using UnityEngine;

public class CoreBuild : Build
{
    [Header("Core Extras")]
    public Stat regenAmount;
    public float regenCooldown;
    public Stat generationAmount;
    public float generationCooldown;

    Coroutine regenRoutine;
    Coroutine genRoutine;

    void Awake()
    {
        currentLife = MaxLife.Value;
        tw = FindAnyObjectByType<TowerBuilder>();

        regenRoutine = StartCoroutine(RegenerationLoop());
        genRoutine   = StartCoroutine(GenerationLoop());
    }

    public override void TakenDamage(float amount)
    {
        base.TakenDamage(amount);

        if (currentLife <= 0f)
        {

            if (regenRoutine != null) StopCoroutine(regenRoutine);
            if (genRoutine   != null) StopCoroutine(genRoutine);
            Destroy(gameObject);
        }
    }

    IEnumerator GenerationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(generationCooldown);
            if (currentLife <= 0f) yield break;
            tw.cash += Mathf.CeilToInt(generationAmount.Value);
        }
    }

    IEnumerator RegenerationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenCooldown);
            if (currentLife <= 0f) yield break;

            currentLife = Mathf.Min(currentLife + regenAmount.Value, MaxLife.Value);
        }
    }
}
