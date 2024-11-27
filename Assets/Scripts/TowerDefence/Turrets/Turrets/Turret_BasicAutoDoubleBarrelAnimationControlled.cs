using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicAutoDoubleBarrelAnimationControlled : Turret_BasicShooter
    {
        [SerializeField] Animator _Animator;
        [SerializeField] Transform _SecondBarrel;
        [SerializeField] AudioClip ShootSFX;
        AudioManager _audioManager => AudioManager.Instance;
        readonly string ShootID = "BolterShoot";


        bool _isRight;

        internal override void ActivationMethod()
        {
            if (Physics.Raycast(_ray, _RayLenght, _EnemyMask))
            {
                _isRight = !_isRight;
                _Animator.SetBool("isRight", _isRight);
                _Animator.SetTrigger("shoot");
                _audioManager.PlayClip(ShootID, ShootSFX);
            }
        }

        public override void Shoot()
        {
            if (_projectileIsValid == false) return;

            Transform CurrentBarrel = _isRight ? _SecondBarrel : _Barrel;
            Projectile proj = Instantiate(_projectile.gameObject, CurrentBarrel.position, Quaternion.identity).GetComponent<Projectile>();
            proj.Initialize(transform.forward, _data);
        }
    }
}
