using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    public abstract class ProjectileGun : Tool
    {
        [SerializeField] internal Projectile _Projectile;
        [SerializeField] internal float speedMultiplier = 1;
        [SerializeField] internal PlayOneShot OneShotNull;
        [SerializeField] internal AudioClip ShootSFX;

        internal override void ActivationImplementation()
        {
            Projectile proj = Instantiate(_Projectile.gameObject, _Barrel.position, Quaternion.identity).GetComponent<Projectile>();
            proj.Initialize(_direction, speedMultiplier);
            if(OneShotNull != null && ShootSFX != null)
            {
                PlayOneShot temp = OneShotNull;
                temp.SetClip(ShootSFX);
                Instantiate(temp, transform.position, Quaternion.identity);
            }
        }
    } 
}
