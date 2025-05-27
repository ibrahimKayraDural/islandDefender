using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CorePowers
{
    public class CorePower_Force : CorePower_OnPoint
    {
        [SerializeField] ProjectileExplosive _ExplosionPrefab;
        [SerializeField] int _Damage;
        [SerializeField] int _TileRadius;
        [SerializeField] List<DamageType> _DamageTypes;

        internal override void OnActivated()
        {
            ProjectileExplosive pe = Instantiate(_ExplosionPrefab.gameObject, _point.Value,
                Quaternion.identity).GetComponent<ProjectileExplosive>();
            pe.Activate(_TileRadius, _Damage, _DamageTypes);
        }
    }
}
