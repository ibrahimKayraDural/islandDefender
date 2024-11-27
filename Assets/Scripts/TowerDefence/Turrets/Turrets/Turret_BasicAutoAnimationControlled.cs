using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicAutoAnimationControlled : Turret_BasicShooter
    {
        [SerializeField] Animator _Animator;

        internal override void ActivationMethod()
        {
            if (Physics.Raycast(_ray, _RayLenght, _EnemyMask))
            {
                _Animator.SetTrigger("shoot");
            }
        }

        public override void Shoot()
        {
            if (_projectileIsValid == false) return;

            Projectile proj = Instantiate(_projectile.gameObject, _Barrel.position, Quaternion.identity).GetComponent<Projectile>();
            proj.Initialize(transform.forward, _data);
        }
    } 
}
