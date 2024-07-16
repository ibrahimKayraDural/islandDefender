using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Turret Data")]
    public class TurretData : ScriptableObject
    {
        public string DisplayName => _displayName;
        public string ID => _id;
        public float ActivationCooldown => _activationCooldown;
        public GameObject PrefabObject => _prefabObject;
        public GameObject ProjectilePrefab => _projectilePrefab;
        public float ProjectileSpeedMultiplier => _projectileSpeedMultiplier;

        [SerializeField] string _displayName = GLOBAL.UnassignedString;
        [SerializeField] string _id = GLOBAL.UnassignedString;
        [SerializeField] float _activationCooldown = 1;
        [SerializeField,Tooltip("Main body of the turret, with turret unit script")] GameObject _prefabObject;
        [SerializeField] GameObject _projectilePrefab;
        [SerializeField, Tooltip("Do not touch if you are certain you know what you are doing")] float _projectileSpeedMultiplier = 1;
    } 
}
