using System.Collections;
using UnityEngine;

public class DefenseBuild : Build
{
    public Stat regenAmaunt;
    public float regenCooldown;

    void Awake()
    {
        StartCoroutine(nameof(RegenerationLoop));
    }

    void OnDestroy()
    {
        StopAllCoroutines();
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
