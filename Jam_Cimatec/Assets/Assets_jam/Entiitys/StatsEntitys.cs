using UnityEngine;


public class StatsEntitys : MonoBehaviour
{
    [Header("General Stats")]
    public Stat MaxHealth = new Stat { BaseValue = 100f };
    public Stat Damage = new Stat { BaseValue = 10f };
    public Stat AttackSpeed = new Stat { BaseValue = 1f };
    public Stat Regeneration = new Stat { BaseValue = 0f }; 

    [HideInInspector] public float CurrentHealth { get; private set; }

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
        if (CurrentHealth <= 0f)
            Die();
    }

    public virtual void Heal(float amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth.Value);
    }

    protected virtual void Regenerate()
    {
        if (Regeneration.Value > 0 && CurrentHealth < MaxHealth.Value)
            Heal(Regeneration.Value * Time.deltaTime);
    }

    protected virtual void Die()
    {
        // implementation here
    }
}
