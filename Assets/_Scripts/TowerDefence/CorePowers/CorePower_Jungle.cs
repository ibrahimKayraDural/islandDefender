using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CorePowers
{
    public class CorePower_Jungle : CorePower_AtRandomTiles
    {
        [SerializeField] EnemySpeedModifier _EnemySpeedModifierPrefab;

        internal override void OnActivated()
        {
            foreach (var t in _tiles)
            {
                var trans = Instantiate(_EnemySpeedModifierPrefab, CleanBeforeWaveParent).transform;
                trans.position = t.transform.position;
                trans.rotation = Quaternion.identity;
            }
        }
    }
}
