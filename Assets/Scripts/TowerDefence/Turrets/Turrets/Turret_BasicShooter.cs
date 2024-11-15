using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicShooter : Turret_Auto
    {
        [SerializeField] Transform _Barrel;
        [SerializeField] float _RayLenght = 100;
        [SerializeField] internal LayerMask _EnemyMask = 1 << 7;
        [SerializeField] internal GameObject _ProjectilePrefab;

        Ray _ray;
        bool _projectileIsValid;

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

        void Shoot()
        {
            if (_projectileIsValid == false) return;

            Projectile proj = Instantiate(_ProjectilePrefab, _Barrel.position, Quaternion.identity).GetComponent<Projectile>();
            proj.Initialize(transform.forward, _data.ProjectileSpeedMultiplier);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction * _RayLenght);
        }
    }
}
