using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class Enemy_Explosive : EnemyBase
    {
        [SerializeField] internal float explosivenessLingerDuration = 1;

        internal float _explosion_TargetTime = -1;

        public override void RemoveHealth(float amount, List<DamageType> damageTypes)
        {
            if (damageTypes.Contains(DamageType.Fire))
                _explosion_TargetTime = Time.time + explosivenessLingerDuration;

            base.RemoveHealth(amount, damageTypes);
        }
        internal override void Die()
        {
            if (_explosion_TargetTime >= Time.time)
            {
                var proj = Data?.ExplosionPrefab?.GetComponent<ProjectileExplosive>();

                if (proj != null)
                {
                    //change this objects layer to prevent a stack overflow
                    gameObject.layer = 0;

                    proj = Instantiate(proj.gameObject, transform.position, Quaternion.identity)
                        .GetComponent<ProjectileExplosive>();
                    proj.Activate(Data.ExplosionRange, Data.ExplosionDamage, new());
                }
            }

            base.Die();
        }
    }
}
