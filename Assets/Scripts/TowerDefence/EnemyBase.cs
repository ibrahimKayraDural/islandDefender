using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IGetShot
{
    public float Health => _health;

    [SerializeField, Min(.1f)] internal float _MaxHealth = 1;

    internal float _health;

    void Awake()
    {
        _health = _MaxHealth;
    }

    public void GetShot(float damage)
    {
        RemoveHealth(damage);
    }

    virtual public void AddHealth(float value) => SetHealth(_health + value);
    virtual public void RemoveHealth(float value) => SetHealth(_health - value);
    virtual public void SetHealth(float setTo)
    {
        _health = Mathf.Clamp(setTo, 0, _MaxHealth);

        if (_health == 0) Die();
    }

    virtual internal void Die() 
    {
        Destroy(gameObject);
    }
}
