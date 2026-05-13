using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    public float HPPercent => currentHP / maxHP;

    // HUDManager and future oxygen UI listen to this
    public event Action<float, float> OnHPChanged;   // (current, max)
    public event Action OnDeath;

    private bool isDead = false;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void DrainHP(float amount)
    {
        if (isDead) return;

        currentHP = Mathf.Max(0f, currentHP - amount);
        OnHPChanged?.Invoke(currentHP, maxHP);

        if (currentHP <= 0f)
            TriggerDeath();
    }

    public void RefillHP(float amount)
    {
        if (isDead) return;

        currentHP = Mathf.Min(maxHP, currentHP + amount);
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    private void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;
        OnDeath?.Invoke();

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
            pc.ForceKill();
    }
}
