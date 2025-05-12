using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_Additive : Turret_LockOn
    {
        [SerializeField]
        List<float> _DamageMultipliers = new()
        { .25f, .25f, .35f, .55f, .70f, .80f, .85f, .90f, 1, 1.2f, 1.5f, 2, 2.5f, 3, 3.25f };

        internal int _shotCount = 0;
        [SerializeField] internal AudioClip ShootSFX;
        [SerializeField] internal string ShootID = "AdditiveShooter";

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

        internal override void Shoot()
        {
            if (_lockedEnemy == null) return;

            var max = _DamageMultipliers.Count - 1;
            _shotCount = Mathf.Min(_shotCount + 1, max);
            float damage = Data.Damage * _DamageMultipliers[_shotCount];
            var pitch = Mathf.InverseLerp(0, max, _shotCount);
            pitch = 1 + pitch * 2;

            _audioManager?.PlayClip(ShootID, ShootSFX, pitch: pitch);

            _lockedEnemy.RemoveHealth(damage, Data.DamageTypes);
        }
        internal override void LockOnTarget(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent(out EnemyBase eb) == false) return;

            base.LockOnTarget(hit);
            _shotCount = 0;
        }
    }
}
