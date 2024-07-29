namespace Overworld
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField, Min(0)] float _Speed = 1;
        [SerializeField, Min(0.001f)] float groundFriction = 10;
        [SerializeField, Min(0)] float _TurnSpeed = 1;

        [Header("Reference")]
        [SerializeField] Rigidbody _RB;
        [SerializeField] Camera _Cam;
        [SerializeField] GameObject _Rotator;
        [SerializeField] Transform _GunPoint;
        [SerializeField] List<Gun> _Guns = new List<Gun>();

        // this is an auto value. do not touch this value. >:(
        int AUTOVALUE_gunIdx = 0;

        // to change the gun, simply change this value. The value will automatically normalize itself to _Guns array.
        int _gunIndex
        {
            get => AUTOVALUE_gunIdx;
            set
            {
                if (_Guns.Count <= 0)
                {
                    Debug.LogError("No guns are in the _Guns list");
                    return;
                }

                _currentGun?.Unequip();

                if (value >= _Guns.Count) AUTOVALUE_gunIdx = 0;
                else if (value < 0) AUTOVALUE_gunIdx = _Guns.Count - 1;
                else AUTOVALUE_gunIdx = value;

                _currentGun.Equip();
            }
        }

        Gun _currentGun
        {
            get
            {
                if (_Guns.Count <= 0) return null;
                try { return _Guns[_gunIndex]; }
                catch { return null; }
            }
        }
        bool _anyGunIsEquipped => _currentGun != null;

        Vector3 _direction
        {
            get
            {
                Vector3 targetVector = Input.mousePosition;
                targetVector.z = _Cam.transform.position.y;
                targetVector = _Cam.ScreenToWorldPoint(targetVector) - transform.position;
                targetVector.y = 0;
                targetVector.Normalize();
                return targetVector;
            }
        }
        Quaternion _dirAsRot => Quaternion.LookRotation(_direction, Vector3.up);

        Vector3 _currentMovement = Vector3.zero;
        float _changeTurret_TargetTime = -1;
        float _changeTurret_Cooldown = .2f;

        private void Start()
        {
            _RB = GetComponent<Rigidbody>();
            _RB.useGravity = false;
            _RB.angularDrag = 0;
            _RB.drag = 0;

            List<Gun> temp = new List<Gun>();

            foreach (var item in _Guns)
            {
                foreach (var typeItem in temp)
                {
                    if (typeItem.GetType().IsEquivalentTo(item.GetType())) goto Checkpoint1;
                }

                Gun og = Instantiate(item.gameObject).GetComponent<Gun>();
                og.Initialize(_GunPoint);
                temp.Add(og);

            Checkpoint1:;
            }

            _Guns = temp;

            _gunIndex = 0;
        }

        void Update()
        {
            _Rotator.transform.rotation = Quaternion.Lerp(_Rotator.transform.rotation, _dirAsRot, Time.deltaTime * _TurnSpeed);

            if (Input.GetButton("Fire1")) ShootGun();
            TryChangeGun(Input.GetAxisRaw("ChangeGun"));
        }

        void FixedUpdate()
        {
            Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * _Speed * Time.deltaTime;
            _currentMovement = Vector3.Lerp(_currentMovement, movement, groundFriction * Time.deltaTime);

            _RB.MovePosition(transform.position + _currentMovement);
        }

        void ShootGun()
        {
            _currentGun?.Activate();
        }

        void TryChangeGun(float v)
        {
            if (v == 0) return;
            if (_changeTurret_TargetTime >= Time.time) return;

            v = v > 0 ? 1 : -1;
            _gunIndex += (int)v;

            _changeTurret_TargetTime = Time.time + _changeTurret_Cooldown;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _direction + transform.position);
        }
    }

}