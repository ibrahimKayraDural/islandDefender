using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBase : MonoBehaviour, IGetHit
{
    public float Health => _health;
    public float Speed => _Speed;
    public float Damage => _Damage;
    public float AttackCooldown => _AttackCooldown;

    [SerializeField, Min(.1f)] internal float _MaxHealth = 1;
    [SerializeField, Min(0)] internal float _Speed = 1;
    [SerializeField, Min(0)] internal float _Damage = 1;
    [SerializeField, Min(0)] internal float _AttackCooldown = 1;
    [SerializeField, Min(0)] internal float _AttackRange = 1;
    [SerializeField] internal Transform _AttackPoint;
    [SerializeField] internal LayerMask _AttackLayer = 1 << 9;
    [SerializeField] internal AudioClip _AttackSFX;

    internal float _health;
    internal Rigidbody _rb;
    internal AudioSource _asource;
    internal float _nextAttack_TargetTime = -1;

    virtual internal void Awake()
    {
        _health = _MaxHealth;
        _rb = GetComponent<Rigidbody>();
        _asource = GetComponent<AudioSource>();
    }
    virtual internal void FixedUpdate()
    {
        Ray ray = new Ray(_AttackPoint.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _AttackRange, _AttackLayer))
        {
            if (_nextAttack_TargetTime <= Time.time)
            {
                if (hit.transform.TryGetComponent(out IGetHit igh))
                {
                    igh.GetHit(_Damage);
                    _nextAttack_TargetTime = Time.time + _AttackCooldown;

                    if (_AttackSFX != null) _asource?.PlayOneShot(_AttackSFX);
                }
            }
        }
        else
        {
            _rb.MovePosition(transform.position + transform.forward * _Speed * Time.deltaTime);
        }
    }

    public void GetHit(float damage)
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
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(_AttackPoint.position, _AttackPoint.position + transform.forward * _AttackRange);
    }
}
