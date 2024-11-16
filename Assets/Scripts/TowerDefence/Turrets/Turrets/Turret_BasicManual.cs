using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicManual : Turret_Manual
    {
        [SerializeField] Transform _RotatingHead;

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
            Debug.Log(gameObject.name + " is activated.");
        }

        internal override void OnInitialized() { }
    }
}
