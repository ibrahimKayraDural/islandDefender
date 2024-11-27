using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicManualDoubleBarrelAnimationControlled : Turret_BasicManual
    {
        [SerializeField] Animator _Animator;

        internal override void ActivationMethod()
        {
            if (_projectileIsValid == false) return;
            if (_isInitialized == false) return;

            Vector3 targetPoint = _lookTransform.position;
            Ray ray = new Ray(_Barrel.position, -_plane.normal);
            if (_plane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);

                hitPoint += _plane.normal * _ProjectileElevation;
                targetPoint += _plane.normal * _ProjectileElevation;

                GameObject go = Instantiate(_ProjectilePrefab, hitPoint, Quaternion.identity);
                go.GetComponent<Projectile>().Initialize(targetPoint - hitPoint, _data);
            }
        }
    }
}
