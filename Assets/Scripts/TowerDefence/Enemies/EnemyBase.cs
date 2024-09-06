using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyBase : MonoBehaviour, IHealth
    {
        public float Health => _health;
        public EnemyData Data => _data;

        [SerializeField] internal Transform _AttackPoint;
        [SerializeField] internal LayerMask _AttackLayer = 1 << 9;
        [SerializeField] internal EnemyData _data;
        [SerializeField] internal GameObject OneShotNull;

        internal bool _hasWon = false;
        internal float _health;
        internal Rigidbody _rb;
        internal float _nextAttack_TargetTime = -1;

        virtual public void Awake()
        {
            _health = _data.MaxHealth;
            _rb = GetComponent<Rigidbody>();
        }
        virtual internal void FixedUpdate()
        {
            if (_hasWon) return;

            Ray ray = new Ray(_AttackPoint.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, _data.AttackRange, _AttackLayer))
            {
                if (_nextAttack_TargetTime <= Time.time)
                {
                    if (hit.transform.TryGetComponent(out IHealth ih))
                    {
                        ih.RemoveHealth(_data.Damage);
                        if (_data.AttackSFX != null)
                        {
                            PlayOneShot temp = OneShotNull.GetComponent<PlayOneShot>();
                            temp.SetClip(_data.AttackSFX);
                            Instantiate(temp.gameObject, transform.position, Quaternion.identity);
                        }

                        _nextAttack_TargetTime = Time.time + _data.AttackCooldown;
                    }
                }
            }
            else
            {
                _rb.MovePosition(transform.position + transform.forward * _data.Speed * Time.deltaTime);
            }
        }
        virtual public void SetHealth(float setTo)
        {
            _health = Mathf.Clamp(setTo, 0, _data.MaxHealth);

            if (_health == 0) Die();
        }

        virtual internal void Die()
        {
            SpawnManager.RemoveFromActiveEnemyList(gameObject);
            Destroy(gameObject);
        }

        virtual internal void Win()
        {
            _hasWon = true;
            Invoke(nameof(Die), 2);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(_AttackPoint.position, _AttackPoint.position + transform.forward * _data.AttackRange);
        }
    }
}
