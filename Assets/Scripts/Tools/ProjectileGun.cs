using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    public abstract class ProjectileGun : Tool
    {
        [SerializeField] internal Projectile _Projectile;
        [SerializeField] internal float speedMultiplier = 1;
        [SerializeField] internal AudioClip ShootSFX;

        AudioManager _audioManager => AudioManager.Instance;

        internal override void ActivationImplementation()
        {
            Projectile proj = Instantiate(_Projectile.gameObject, _Barrel.position, Quaternion.identity).GetComponent<Projectile>();
            proj.Initialize(_direction, speedMultiplier: speedMultiplier);

            _audioManager?.PlayClip(this + "_shooting", ShootSFX);
        }
    } 
}
