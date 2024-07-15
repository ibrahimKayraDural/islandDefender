using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Turret Data")]
    public class TurretData : ScriptableObject
    {
        public string DisplayName => _DisplayName;
        public string ID => _id;
        public GameObject PrefabObject => _PrefabObject;

        [SerializeField] string _DisplayName = GLOBAL.UnassignedString;
        [SerializeField] string _id = GLOBAL.UnassignedString;
        [SerializeField] GameObject _PrefabObject;
    } 
}
