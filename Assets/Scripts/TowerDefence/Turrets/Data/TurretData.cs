using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Turret Data")]
    public class TurretData : Data<TurretData>
    {
        public float ActivationCooldown => _activationCooldown;
        public Cost[] Costs => _costs;
        public GameObject PrefabObject => _prefabObject;
        public GameObject ProjectilePrefab => _projectilePrefab;
        public float ProjectileSpeedMultiplier => _projectileSpeedMultiplier;

        [SerializeField] float _activationCooldown = 1;
        [SerializeField] Cost[] _costs = new Cost[0];
        [SerializeField,Tooltip("Main body of the turret, with turret unit script")] GameObject _prefabObject;
        [SerializeField] GameObject _projectilePrefab;
        [SerializeField, Tooltip("Do not touch if you are certain you know what you are doing")] float _projectileSpeedMultiplier = 1;
    } 
}
