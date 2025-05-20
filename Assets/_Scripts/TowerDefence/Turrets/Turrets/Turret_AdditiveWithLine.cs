using UnityEngine;

namespace TowerDefence
{
    public class Turret_AdditiveWithLine : Turret_Additive
    {
        [SerializeField] LineRenderer _LineRenderer;

        Vector3 _barrelPos;

        internal virtual void Update()
        {
            if (_lockedEnemy != null)
            {
                var targetPos = _barrelPos;
                targetPos.z = _lockedEnemy.transform.position.z;
                _LineRenderer.SetPosition(1, targetPos);
            }
        }

        internal override void LockOnTarget(RaycastHit hit)
        {
            base.LockOnTarget(hit);
            _LineRenderer.gameObject.SetActive(true);
        }
        internal override void ReleaseEnemy()
        {
            base.ReleaseEnemy();
            _LineRenderer.gameObject.SetActive(false);
        }

        internal override void OnInitialized()
        {
            base.OnInitialized();

            _barrelPos = _Barrel.position;
            _LineRenderer.SetPositions(new Vector3[] { _barrelPos, _barrelPos });
        }
    }
}
