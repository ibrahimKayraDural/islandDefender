using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyBase : MonoBehaviour, IGetHit
    {
        public float Health => _health;
        public EnemyData Data => _data;

        [SerializeField] internal Transform _AttackPoint;
        [SerializeField] internal LayerMask _AttackLayer = 1 << 9;
        [SerializeField] internal EnemyData _data;

        internal float _health;
        internal Rigidbody _rb;
        internal AudioSource _asource;
        internal float _nextAttack_TargetTime = -1;

        virtual public void Awake()
        {
            _health = _data.MaxHealth;
            _rb = GetComponent<Rigidbody>();
            _asource = GetComponent<AudioSource>();
        }
        virtual internal void FixedUpdate()
        {
            Ray ray = new Ray(_AttackPoint.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, _data.AttackRange, _AttackLayer))
            {
                if (_nextAttack_TargetTime <= Time.time)
                {
                    if (hit.transform.TryGetComponent(out IGetHit igh))
                    {
                        igh.GetHit(_data.Damage);
                        _nextAttack_TargetTime = Time.time + _data.AttackCooldown;

                        if (_data.AttackSFX != null) _asource?.PlayOneShot(_data.AttackSFX);
                    }
                }
            }
            else
            {
                _rb.MovePosition(transform.position + transform.forward * _data.Speed * Time.deltaTime);
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
            _health = Mathf.Clamp(setTo, 0, _data.MaxHealth);

            if (_health == 0) Die();
        }

        virtual internal void Die()
        {
            Destroy(gameObject);
        }
        void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(_AttackPoint.position, _AttackPoint.position + transform.forward * _data.AttackRange);
        }
    } 
}
