using UnityEngine;

[System.Serializable]
public class Stat
{
    [Tooltip("Valor base sem modificadores")]
    public float BaseValue = 1f;
    [Tooltip("Modificadores acumulados (upgrades, buffs etc)")]
    public float ModifierValue = 0f;

    public float Value => BaseValue + ModifierValue;

    public void ResetModifiers()    => ModifierValue = 0f;
    public void AddModifier(float m) => ModifierValue += m;
}
