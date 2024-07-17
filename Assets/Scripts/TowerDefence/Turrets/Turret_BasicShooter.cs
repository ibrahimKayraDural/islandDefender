using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicShooter : TurretUnit
    {
        [SerializeField] Transform ShootPoint;
        [SerializeField] float rayLenght = 100;
        [SerializeField] internal LayerMask _enemyMask = 1 << 7;  //7th layer which is TowerDefenceEnemy
        Ray _ray;

        public override void Initialize(TurretData data, TowerDefenceTileScript tile)
        {
            if (_isInitialized) return;

            _data = data;
            _parentTile = tile;
            transform.parent = tile.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            _health = _MaxHealth;

            _ray = new Ray(ShootPoint.position, transform.forward);

            tile.SetOccupied(this);
            StartCoroutine(nameof(ActivationLoop), _data.ActivationCooldown);

            _isInitialized = true;
        }

        internal override void ActivationMethod()
        {
            if (Physics.Raycast(_ray, rayLenght, _enemyMask))
            {
                Shoot();
            }
        }

        void Shoot()
        {
            if (_data.ProjectilePrefab.TryGetComponent<Projectile>(out _) == false) return;
            Projectile proj = Instantiate(_data.ProjectilePrefab, ShootPoint.position, Quaternion.identity).GetComponent<Projectile>();
            proj.Initialize(transform.forward, _data.ProjectileSpeedMultiplier);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction * rayLenght);
        }
    }
}
