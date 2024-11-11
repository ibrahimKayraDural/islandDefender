using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public abstract class Turret_Remote : TurretUnit
    {
        internal IEnumerator IEnumUpdate_Handle = null;
        internal bool _isSelected = false;
        internal float _nextUse_TargetTime = -1;

        public void SetSelected(bool setTo)
        {
            if (setTo) AUTO_OnSelected();
            else AUTO_OnDeselected();
        }

        internal void AUTO_OnSelected()
        {
            if (_isSelected) return;

            IEnumUpdate_Handle = IEnumUpdate();
            StartCoroutine(IEnumUpdate_Handle);
            OnSelected();

            _isSelected = true;
        }
        abstract internal void OnSelected();

        internal void AUTO_OnDeselected()
        {
            if (_isSelected == false) return;

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

        public void UseTurret()
        {
            if (_nextUse_TargetTime > Time.time) return;

            ActivationMethod();

            _nextUse_TargetTime = Time.time + Data.ActivationCooldown;
        }
    }
}
