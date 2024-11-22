using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public abstract class Turret_Manual : MonoBehaviour
    {
        public TurretData Data => _data;
        public float MaxHealth => _MaxHealth;
        public float Health => _health;

        internal float _MaxHealth;
        internal TurretData _data;
        internal Transform _lookTransform = null;
        internal IEnumerator IEnumUpdate_Handle = null;
        internal bool _isSelected = false;
        internal float _nextUse_TargetTime = -1;
        internal float _health;
        internal bool _isInitialized;

        virtual public void Initialize(TurretData data, Transform parent)
        {
            if (_isInitialized) return;

            _data = data;
            _MaxHealth = data.Health;
            transform.parent = parent;
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
            SetHealth(_MaxHealth);

            OnInitialized();

            _isInitialized = true;
        }
        virtual public void SetHealth(float setTo)
        {
            _health = Mathf.Clamp(setTo, 0, _MaxHealth);

            if (_health == 0) KillSelf();
        }
        virtual public void KillSelf()
        {
            Destroy(gameObject);
        }

        internal abstract void OnInitialized();

        public void SelectTurret(Transform lookTransform)
        {
            if (_isSelected) return;

            _lookTransform = lookTransform;
            IEnumUpdate_Handle = IEnumUpdate();
            StartCoroutine(IEnumUpdate_Handle);
            OnSelected();

            _isSelected = true;
        }
        internal abstract void OnSelected(Transform lookPosition = null);

        public void DeselectTurret()
        {
            if (_isSelected == false) return;

            _lookTransform = null;
            if (IEnumUpdate_Handle != null)
            {
                StopCoroutine(IEnumUpdate_Handle);
                IEnumUpdate_Handle = null;
            }

            OnDeselected();

            _isSelected = false;
        }
        abstract internal void OnDeselected();

        virtual internal IEnumerator IEnumUpdate()
        {
            while (true)
            {
                yield return null;
                OnUpdate();
            }
        }

        abstract internal void OnUpdate();
        abstract internal void ActivationMethod();

        public void UseTurret()
        {
            if (_nextUse_TargetTime > Time.time) return;

            ActivationMethod();

            _nextUse_TargetTime = Time.time + Data.ActivationCooldown;
        }
    }
}
