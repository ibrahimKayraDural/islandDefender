using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    public abstract class Gun : MonoBehaviour
    {
        public string DisplayName => _DisplayName;
        public string ID => _ID;

        [SerializeField, Min(0)] internal float _ShootCooldown = 1;
        [SerializeField] internal Transform _Barrel;
        [SerializeField] internal string _DisplayName = GLOBAL.UnassignedString;
        [SerializeField] internal string _ID = GLOBAL.UnassignedString;
        [SerializeField] internal GameObject[] Visuals;

        internal virtual Vector3 _direction => transform.forward;

        internal bool _isEquipped;
        internal float _shoot_TargetTime = -1;


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

            foreach (var item in Visuals) item.SetActive(false);

            _isEquipped = false;
        }
        public void Activate()
        {
            if (_isEquipped == false) return;
            if (_shoot_TargetTime > Time.time) return;

            ActivationImplementation();
            _shoot_TargetTime = Time.time + _ShootCooldown;
        }

        //override this method to handle what happens after activation (like bullet spawning)
        virtual public void ActivationImplementation() { }
    } 
}
