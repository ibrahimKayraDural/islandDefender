using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicAreaShooter : Turret_Auto
    {
        [SerializeField] internal float _ShootLenght = 5;
        [SerializeField] internal LayerMask _EnemyMask = 1 << 7;

        [SerializeField] internal AudioClip ShootSFX;
        [SerializeField] internal string ShootID = "BasicAreaShooterShoot";
        [SerializeField] ParticleSystem _ShootingEffect;

        internal AudioManager _audioManager
        {
            get
            {
                if (AUTO_audioManager == null)
                    AUTO_audioManager = AudioManager.Instance;
                return AUTO_audioManager;
            }
        }
        AudioManager AUTO_audioManager = null;
        internal Ray _ray;
        internal Vector3 _shootOrigin => transform.position + Vector3.up;
        internal Vector3 _shootPos;
        internal Vector3 _halfExtents;

        float _damage;
        List<DamageType> _damageTypes;

        internal override void OnInitialized()
        {
            base.OnInitialized();

            _ray = new Ray(_shootOrigin, transform.forward);
            _shootPos = _shootOrigin + (transform.forward * (_ShootLenght / 2));
            _halfExtents = new Vector3(.5f, .5f,_ShootLenght / 2);
            _damage = Data.Damage;
            _damageTypes = Data.DamageTypes;
        }

        internal override void ActivationMethod()
        {
            if (Physics.Raycast(_ray, _ShootLenght, _EnemyMask))
            {
                Shoot();
            }
        }

        public virtual void Shoot()
        {
            _audioManager?.PlayClip(ShootID, ShootSFX);
            _ShootingEffect?.Play();

            var cols = Physics.OverlapBox(_shootPos, _halfExtents, Quaternion.identity,
                _EnemyMask, QueryTriggerInteraction.Collide);

            foreach (var col in cols)
            {
                if (col.TryGetComponent(out IHealth enemyH)) enemyH.RemoveHealth(_damage, _damageTypes);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            //Gizmos.DrawLine(_ray.origin, _ray.origin + _ray.direction * _RayLenght);
            Gizmos.DrawLine(_shootOrigin, _shootOrigin + (Vector3.forward * _ShootLenght));
            Gizmos.DrawWireCube(_shootOrigin, _halfExtents * 2);
        }
    }
}
