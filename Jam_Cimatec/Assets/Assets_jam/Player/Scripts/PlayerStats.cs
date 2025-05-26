using UnityEngine;
using System.Collections;
public class PlayerStats : MonoBehaviour
{
    [Header("General Stats")]
    public Stat MaxHealth     = new Stat { BaseValue = 100f };
    public Stat Damage        = new Stat { BaseValue = 10f };
    public Stat AttackSpeed   = new Stat { BaseValue = 1f  };
    public Stat Regeneration  = new Stat { BaseValue = 0f  };

    [HideInInspector]
    public float CurrentHealth { get; private set; }
    [HideInInspector] public bool isDead;

    [Header("Upgrade Stats")]
    public int SkillPoints = 0;

    protected virtual void Awake()
    {
        CurrentHealth = MaxHealth.Value;
    }

    protected virtual void Update()
    {
        if (CurrentHealth <= 0f) return;
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
        if (isDead) return;
        isDead = true;
        StartCoroutine(RespawnRoutine());
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
    
    IEnumerator RespawnRoutine()
    {
        // 1) “Kill” the player
        isDead = true;
        GetComponent<PlayerController>().enabled = false;
        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = false;

        // 2) Wait
        yield return new WaitForSeconds(10f);

        // 3) “Re­spawn” by moving and re-enabling
        transform.position = GameObject.FindWithTag("PowerCore").transform.position;
        CurrentHealth      = MaxHealth.Value;
        isDead             = false;
        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = true;
        foreach (var c in GetComponentsInChildren<Collider2D>()) c.enabled = true;
        GetComponent<PlayerController>().enabled = true;
    }
}
