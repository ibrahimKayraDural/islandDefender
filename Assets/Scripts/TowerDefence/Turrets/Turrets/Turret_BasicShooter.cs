using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicShooter : Turret_Auto
    {
        [SerializeField] internal Transform _Barrel;
        [SerializeField] internal float _RayLenght = 100;
        [SerializeField] internal LayerMask _EnemyMask = 1 << 7;
        [SerializeField] internal GameObject _ProjectilePrefab;

        internal Ray _ray;
        internal bool _projectileIsValid;

        private void Awake()
        {
            _projectileIsValid = _ProjectilePrefab.TryGetComponent<Projectile>(out _);
        }

        internal override void OnInitialized()
        {
            base.OnInitialized();
            _ray = new Ray(_Barrel.position, transform.forward);
        }

        internal override void ActivationMethod()
        {
            if (Physics.Raycast(_ray, _RayLenght, _EnemyMask))
            {
                Shoot();
            }
        }

        public virtual void Shoot()
        {
            if (_projectileIsValid == false) return;

            Projectile proj = Instantiate(_ProjectilePrefab, _Barrel.position, Quaternion.identity).GetComponent<Projectile>();
            proj.Initialize(transform.forward, _data);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction * _RayLenght);
        }
    }
}
