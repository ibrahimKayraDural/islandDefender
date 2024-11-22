using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicShooter : Turret_Auto
    {
        [SerializeField] Transform _VisualBarrel;
        [SerializeField] float _RayLenght = 100;
        [SerializeField] internal LayerMask _EnemyMask = 1 << 7;

        Ray _ray;
        Vector3 _shootPoint;

        public override void Initialize(TurretData data, TowerDefenceTileScript tile)
        {
            if (_isInitialized) return;

            _data = data;
            _parentTile = tile;
            transform.parent = tile.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            _health = _MaxHealth;
            _shootPoint = transform.position + Vector3.up * GLOBAL.TDColliderElevation;

            _ray = new Ray(_shootPoint, transform.forward);

            tile.SetOccupied(this);
            StartCoroutine(nameof(ActivationLoop), _data.ActivationCooldown);

            _isInitialized = true;
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

            Vector3 displacement = _VisualBarrel.position - _shootPoint;

            Projectile proj = Instantiate(_data.ProjectilePrefab, _shootPoint, Quaternion.identity).GetComponent<Projectile>();
            proj.Initialize(transform.forward, displacement, _data.ProjectileSpeedMultiplier);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction * _RayLenght);
        }
    }
}
