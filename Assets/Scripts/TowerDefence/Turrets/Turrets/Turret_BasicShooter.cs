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

        Ray _ray;

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
            if (_data.ProjectilePrefab.TryGetComponent<Projectile>(out _) == false) return;

            Projectile proj = Instantiate(_data.ProjectilePrefab, _Barrel.position, Quaternion.identity).GetComponent<Projectile>();
            proj.Initialize(transform.forward, _data.ProjectileSpeedMultiplier);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction * _RayLenght);
        }
    }
}
