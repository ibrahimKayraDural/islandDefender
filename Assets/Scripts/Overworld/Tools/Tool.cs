using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    public abstract class Tool : MonoBehaviour
    {
        public string DisplayName => _DisplayName;
        public string ID => _ID;

        [SerializeField, Min(0)] internal float _ShootCooldown = 1;
        [SerializeField] internal string _DisplayName = GLOBAL.UnassignedString;
        [SerializeField] internal string _ID = GLOBAL.UnassignedString;
        [SerializeField] internal GameObject _Prefab;
        [SerializeField] internal Transform _Barrel;
        [SerializeField] internal GameObject[] Visuals;

        internal virtual Vector3 _direction => transform.forward;

        internal bool _isEquipped;
        internal float _shoot_TargetTime = -1;

        virtual internal void OnApplicationFocus(bool focus)
        {
            if (_isEquipped && focus == false) StopFiring();
        }
        virtual public void Initialize(Transform equipPoint)
        {
            transform.parent = equipPoint;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            foreach (var item in Visuals) item.SetActive(false);
        }
        virtual public void Equip()
        {
            if (_isEquipped) return;

            foreach (var item in Visuals) item.SetActive(true);

            _isEquipped = true;
        }
        virtual public void Unequip()
        {
            if (_isEquipped == false) return;

            StopFiring();
            foreach (var item in Visuals) item.SetActive(false);

            _isEquipped = false;
        }

        internal bool _isFiring;
        virtual public void StartFiring()
        {
            if (_isFiring) return;
            StartCoroutine(nameof(FireIEnum));
        }
        virtual public void StopFiring()
        {
            if (_isFiring == false) return;
            StopCoroutine(nameof(FireIEnum));
            _isFiring = false;
        }

        virtual internal IEnumerator FireIEnum()
        {
            while(true)
            {
                _isFiring = true;
                float duration = Mathf.Max(_shoot_TargetTime - Time.time, 0);
                yield return new WaitForSeconds(duration);
                Activate();
            }
        }

        virtual internal void Activate()
        {
            if (_isEquipped == false) return;

            ActivationImplementation();
            _shoot_TargetTime = Time.time + _ShootCooldown;
        }

        abstract internal void ActivationImplementation();
    } 
}
