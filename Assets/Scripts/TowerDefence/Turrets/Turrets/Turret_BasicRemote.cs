using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_BasicRemote : Turret_Remote
    {
        [SerializeField] GameObject _MeshParent;

        Vector3 _origin = Vector3.zero;
        private void Awake()
        {
            _origin = _MeshParent.transform.localPosition;
        }

        internal override void OnDeselected()
        {
            _MeshParent.transform.localPosition = _origin;
        }

        internal override void OnSelected()
        {
            _MeshParent.transform.localPosition = _origin + Vector3.right * -.2f;
        }

        internal override void OnUpdate() { }

        internal override void ActivationMethod()
        {
            Debug.Log(gameObject.name + " is activated.");
        }
    }
}
