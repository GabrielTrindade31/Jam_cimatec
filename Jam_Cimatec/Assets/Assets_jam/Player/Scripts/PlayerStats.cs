using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("General Stats")]
    public Stat MaxHealth     = new Stat { BaseValue = 100f };
    public Stat Damage        = new Stat { BaseValue = 10f };
    public Stat AttackSpeed   = new Stat { BaseValue = 1f  };
    public Stat Regeneration  = new Stat { BaseValue = 0f  };

    [HideInInspector]
    public float CurrentHealth { get; private set; }

    [Header("Upgrade Stats")]
    public int SkillPoints = 0;

    protected virtual void Awake()
    {
        CurrentHealth = MaxHealth.Value;
    }

    protected virtual void Update()
    {
        Regenerate();
    }

    public virtual void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        Debug.Log($"[PlayerStats] Took {amount} damage. CurrentHealth = {CurrentHealth}");
        if (CurrentHealth <= 0f)
            Die();
    }

    public virtual void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth.Value);
    }

    protected virtual void Regenerate()
    {
        if (Regeneration.Value > 0f && CurrentHealth < MaxHealth.Value)
            Heal(Regeneration.Value * Time.deltaTime);
    }

    protected void Die()
    {
        Debug.Log("Player morreu!");
        // aqui você pode chamar a animação de morte, reload de cena, etc.
    }

    public void UpgradeHealth(float bonus)
    {
        MaxHealth.BaseValue += bonus;
        CurrentHealth += bonus; // opcional: cura o player quando aumenta vida
    }

    public void UpgradeDamage(float bonus)
    {
        Damage.AddModifier(bonus);
    }

    // desenha a vida no canto superior esquerdo
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 25),
            $"Vida: {CurrentHealth:0}/{MaxHealth.Value:0}");
    }
}
