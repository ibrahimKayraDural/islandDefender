using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] internal float _DeathDuration = .5f;
        [SerializeField] internal Renderer _Renderer;
        [SerializeField] internal Animator _Animator;

        [Space(15)]
        [SerializeField] internal UnityEvent OnDeath;

        internal bool _hasWon = false;
        internal bool _isDead = false;
        internal float _health;
        internal Rigidbody _rb;
        internal float _nextAttack_TargetTime = -1;
        internal AudioManager _audioManager => AudioManager.Instance;

        virtual public void Awake()
        {
            _health = _data.MaxHealth;
            _rb = GetComponent<Rigidbody>();
        }

        virtual internal void FixedUpdate()
        {
            if (_hasWon || _isDead) return;

            Ray ray = new Ray(_AttackPoint.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, _data.AttackRange, _AttackLayer))
            {
                _Animator.SetBool("IsMoving", false);

                if (_nextAttack_TargetTime <= Time.time)
                {
                    if (hit.transform.TryGetComponent(out IHealth ih))
                    {
                        ih.RemoveHealth(_data.Damage);

                        _audioManager.PlayClip(this + "_attack", Data.AttackSFX);
                        _Animator.SetTrigger("Attack");

                        _nextAttack_TargetTime = Time.time + _data.AttackCooldown;
                    }
                }
            }
            else
            {
                _Animator.SetBool("IsMoving", true);
                _rb.MovePosition(transform.position + transform.forward * _data.Speed * Time.deltaTime);
            }
        }
        virtual public void SetHealth(float setTo)
        {
            if (_isDead) return;

            _health = Mathf.Clamp(setTo, 0, _data.MaxHealth);

            if (_health == 0) Die();
        }

        virtual internal void Die()
        {
            SpawnManager.RemoveFromActiveEnemyList(gameObject);
            _isDead = true;
            OnDeath?.Invoke();
            StartCoroutine(DeathAnim());
        }

        IEnumerator DeathAnim()
        {
            const float step = .05f;
            float progress = 0;
            Material mat = _Renderer.material;

            while (progress < _DeathDuration)
            {
                yield return new WaitForSeconds(step);
                progress += step;
                mat.SetFloat("_Fill", progress / _DeathDuration);
            }

            mat.SetFloat("_Fill", 1);
            Destroy(gameObject);
        }

        virtual internal void Win()
        {
            if (_isDead) return;

            _hasWon = true;
            Invoke(nameof(Die), 2);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(_AttackPoint.position, _AttackPoint.position + transform.forward * _data.AttackRange);
        }
    }
}
