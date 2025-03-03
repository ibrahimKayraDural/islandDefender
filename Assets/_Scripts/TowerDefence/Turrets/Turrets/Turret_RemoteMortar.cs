using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Turret_RemoteMortar : Turret_Remote
    {
        [SerializeField] LayerMask _TileMask = 1 << 6;
        [SerializeField] ProjectileExplosive _ProjectilePrefab;
        [SerializeField] GameObject _MeshParent;
        [SerializeField] Animator _Animator;

        Vector3 _origin = Vector3.zero;

        private void Awake()
        {
            _origin = _MeshParent.transform.localPosition;
        }

        internal override void ActivationMethod()
        {
            Ray ray = new Ray(_lookTransform.position + Vector3.up, Vector3.down);
            if(Physics.Raycast(ray, out RaycastHit hit, 10, _TileMask))
            {
                _Animator.SetTrigger("shoot");
                Vector3 point = hit.collider.transform.position;
                ProjectileExplosive p = Instantiate(_ProjectilePrefab, point, Quaternion.identity).GetComponent<ProjectileExplosive>();
                p.Activate(Data.AOE_Radius, Data.Damage);
            }
        }

        internal override void OnDeselected()
        {
            _MeshParent.transform.localPosition = _origin;
        }

        internal override void OnSelected(Transform lookPosition = null)
        {
            _MeshParent.transform.localPosition = _origin + Vector3.right * -.2f;
        }

        internal override void OnInitialized()
        {
        }

        internal override void OnUpdate()
        {
        }
    }
}
