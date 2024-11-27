using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicManual : Turret_Manual
    {
        internal enum TurnAxis { X, Y, Z }

        [SerializeField] internal Transform _RotatingHead;
        [SerializeField] internal Transform _Barrel;
        [SerializeField] internal GameObject _ProjectilePrefab;
        [SerializeField] internal float _ProjectileElevation = .5f;
        [SerializeField] internal TurnAxis _TurnAxis = TurnAxis.Y;

        internal Plane _plane;
        internal bool _projectileIsValid = false;

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
                float targetRot = rot.eulerAngles.y;
                Vector3 targetVector = Vector3.zero;

                switch (_TurnAxis)
                {
                    case TurnAxis.X: targetVector.x = targetRot; break;
                    case TurnAxis.Y: targetVector.y = targetRot; break;
                    case TurnAxis.Z: targetVector.z = targetRot; break;
                }


                _RotatingHead.localEulerAngles = targetVector;
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
