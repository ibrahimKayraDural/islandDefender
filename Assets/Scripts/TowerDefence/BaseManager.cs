using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

public class BaseManager : MonoBehaviour, IHealth
{
    public event EventHandler e_BaseHasDied;
    public event EventHandler<float> e_BaseIsAttacked;

    [Header("Values")]
    [SerializeField, Min(.1f)] float _MaxHealth = 10;
    [Header("Reference")]
    [SerializeField] TextMeshProUGUI _HealthTM;
    [SerializeField] AudioClip _DeathSFX;
    [SerializeField] AudioClip _HitSFX;
    [SerializeField] UnityEvent OnDeath;

    public float Health => _health;
    float _health;

    private void Start()
    {
        SetHealth(_MaxHealth);
    }

    public void RemoveHealth(float amount)
    {
        float targetHealth = Mathf.Clamp(Health - amount, 0, _MaxHealth);
        e_BaseIsAttacked?.Invoke(this, targetHealth);
        AudioManager.Instance?.PlayClip(this + "_GettingHit", _HitSFX);
        SetHealth(targetHealth);
    }

    public void SetHealth(float setTo)
    {
        _health = Mathf.Clamp(setTo, 0, _MaxHealth);
        _HealthTM.text = "BASE HEALTH : " + _health;

        if (_health == 0) Die();
    }

    void Die()
    {
        e_BaseHasDied?.Invoke(this, EventArgs.Empty);

        AudioManager.Instance?.PlayClip(this + "_explosion", _DeathSFX);

        OnDeath?.Invoke();
    }
}
