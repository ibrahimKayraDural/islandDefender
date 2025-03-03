using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicManualDoubleBarrelAnimationControlled : Turret_BasicManual
    {
        [SerializeField] Animator _Animator;
        [SerializeField] Transform _RightBarrel;
        [SerializeField] float _SFXPitch = .8f;

        bool _isRight;

        internal override void ActivationMethod()
        {
            if (_projectileIsValid == false) return;
            if (_isInitialized == false) return;

            _isRight = !_isRight;
            _Animator.SetBool("isRight", _isRight);
            _Animator.SetTrigger("shoot");
            _audioManager.PlayClip(_shootClipID, _ShootClip, pitch: _SFXPitch);
        }

        public void Shoot()
        {
            Transform currentBarrel = _isRight ? _RightBarrel : _Barrel;
            GameObject go = Instantiate(_ProjectilePrefab, currentBarrel.position, Quaternion.identity);
            go.GetComponent<Projectile>().Initialize(Vector3.forward, _data);

            //if (_lookTransform == null) return;

            //Vector3 targetPoint = _lookTransform.position;
            //Transform currentBarrel = _isRight ? _RightBarrel : _Barrel;
            //Ray ray = new Ray(currentBarrel.position, -_plane.normal);

            //if (_plane.Raycast(ray, out float enter))
            //{
            //    Vector3 hitPoint = ray.GetPoint(enter);

            //    hitPoint += _plane.normal * _ProjectileElevation;
            //    targetPoint += _plane.normal * _ProjectileElevation;

            //    GameObject go = Instantiate(_ProjectilePrefab, hitPoint, Quaternion.identity);
            //    go.GetComponent<Projectile>().Initialize(targetPoint - hitPoint, _data);
            //}
        }
    }
}
