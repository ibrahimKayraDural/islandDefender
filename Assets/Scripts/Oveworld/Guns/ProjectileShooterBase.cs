using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    public abstract class ProjectileShooterBase : Gun
    {
        [SerializeField] internal Projectile _Projectile;
        [SerializeField] internal float speedMultiplier = 1;

        public override void ActivationImplementation()
        {
            base.ActivationImplementation();
            Projectile proj = Instantiate(_Projectile.gameObject, _Barrel.position, Quaternion.identity).GetComponent<Projectile>();
            proj.Initialize(_direction, speedMultiplier);
        }
    } 
}
