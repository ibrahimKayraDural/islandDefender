using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicManual : Turret_Manual
    {
        [SerializeField] Transform _RotatingHead;
        [SerializeField] Transform _Barrel;
        [SerializeField] GameObject _ProjectilePrefab;
        [SerializeField] float _ProjectileElevation = .5f;

        Plane _plane;
        bool _projectileIsValid = false;

        internal override void OnDeselected()
        {
            _RotatingHead.localEulerAngles = new Vector3(0, 0, 0);
        }

        internal override void OnSelected(Transform lookPosition = null) { }

        internal override void OnUpdate()
        {
            if (_lookTransform != null)
            {
                Quaternion rot = Quaternion.LookRotation(_lookTransform.position - _RotatingHead.position, Vector3.up);
                _RotatingHead.localEulerAngles = new Vector3(0, rot.eulerAngles.y, 0);
            }
        }

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

        internal override void OnInitialized()
        {
            if (_ProjectilePrefab != null)
                _projectileIsValid = _ProjectilePrefab.TryGetComponent<Projectile>(out _);

            _plane = new Plane(new Vector3(1, 1, 0).normalized, transform.parent.position);
        }
    }
}
