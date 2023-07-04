using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public static event EventHandler OnAnyDead;
    public event EventHandler OnDead;
    public event EventHandler OnDamage;
    [SerializeField] private int healthMax;
    private int health;

    private void Awake() {
        health = healthMax;
    }

    public void Damage(int damageAmount) {
        health = Mathf.Max(health - damageAmount, 0);
        OnDamage?.Invoke(this, EventArgs.Empty);
        if (health == 0) {
            Die();
        }
    }

    private void Die() {
        OnDead?.Invoke(this, EventArgs.Empty);
        OnAnyDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetNormalizedHealth() {
        return (float)health / healthMax;
    }
}
