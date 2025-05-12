using UnityEngine;

namespace TowerDefence
{
    public abstract class Turret_LockOn : Turret_Auto
    {
        [SerializeField] internal float _RayLenght = 100;
        [SerializeField] internal LayerMask _EnemyMask = 1 << 7;
        [SerializeField] internal Transform _Barrel;

        internal Ray _ray;
        internal EnemyBase _lockedEnemy = null;

        internal override void OnInitialized()
        {
            base.OnInitialized();
            _ray = new Ray(_Barrel.position, transform.forward);
        }
        internal override void ActivationMethod()
        {
            if (_lockedEnemy != null)
                Shoot();
            else if (Physics.Raycast(_ray, out RaycastHit hit, _RayLenght, _EnemyMask))
                LockOnTarget(hit);
        }

        internal virtual void LockOnTarget(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent(out EnemyBase eb) == false) return;

            _lockedEnemy = eb;
            _lockedEnemy.OnDeath.AddListener(ReleaseEnemy);
        }
        internal abstract void Shoot();
        internal virtual void ReleaseEnemy()
        {
            _lockedEnemy?.OnDeath.RemoveListener(ReleaseEnemy);
            _lockedEnemy = null;
        }
        internal override void OnDestroy()
        {
            ReleaseEnemy();

            base.OnDestroy();
        }
    }
}
