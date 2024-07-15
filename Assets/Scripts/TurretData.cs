using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Turret Data")]
    public class TurretData : ScriptableObject
    {
        public string TurretName => TurretName;

        [SerializeField] string _TurretName = GLOBAL.UnassignedString;
    } 
}
