using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicRemote : Turret_Remote
    {
        [SerializeField] GameObject _MeshParent;
        [SerializeField] Transform _RotatingHead;

        Vector3 _origin = Vector3.zero;

        private void Awake()
        {
            _origin = _MeshParent.transform.localPosition;
        }

        internal override void OnDeselected()
        {
            _MeshParent.transform.localPosition = _origin;
            _RotatingHead.localEulerAngles = new Vector3(0, 0, 0);
        }

        internal override void OnSelected(Transform lookPosition = null)
        {
            _MeshParent.transform.localPosition = _origin + Vector3.right * -.2f;
        }

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
