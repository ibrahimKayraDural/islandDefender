using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class BaseManager : MonoBehaviour, IHealth
{
    [Header("Values")]
    [SerializeField, Min(.1f)] float _MaxHealth = 1;
    [Header("Reference")]
    [SerializeField] TextMeshProUGUI _HealthTM;

    public float Health => _health;
    float _health;

    private void Start()
    {
        SetHealth(_MaxHealth);
    }

    public void SetHealth(float setTo)
    {
        _health = Mathf.Clamp(setTo, 0, _MaxHealth);
        _HealthTM.text = "BASE HEALTH : " + _health;
        if (_health == 0) Die();
    }

    void Die()
    {
        Debug.Log("Base is dead :(");
    }
}
