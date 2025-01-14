using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public enum TurretControlType { Automatic, Remote, Manual }
    public enum TurretAttribute { Penetrater, Buffer, AreaOfEffect }

    [CreateAssetMenu(menuName = "Tower Defence/Turret Data")]
    public class TurretData : GameplayElementData<TurretData>
    {
        [HideInInspector] public int PenetrationCount = 0;
        [HideInInspector] public int AOE_Radius = 0;

        public TurretControlType Type => _Type;
        public List<TurretAttribute> Attributes => _Attributes;
        public int Health => _Health;
        public int Damage => _Damage;
        public float ActivationCooldown => _activationCooldown;
        public Cost[] Costs => _costs;
        public GameObject PrefabObject => _prefabObject;
        public GameObject ProjectilePrefab => _projectilePrefab;
        public float ProjectileSpeedMultiplier => _projectileSpeedMultiplier;
        public AudioClip GetDamagedSFX => _GetDamagedSFX;
        public AudioClip DeathSFX => _DeathSFX;

        [SerializeField] TurretControlType _Type;
        [SerializeField] List<TurretAttribute> _Attributes = new List<TurretAttribute>();
        [SerializeField] int _Health = 1;
        [SerializeField] int _Damage = 1;
        [SerializeField] float _activationCooldown = 1;
        [SerializeField] Cost[] _costs = new Cost[0];
        [SerializeField, Tooltip("Main body of the turret, with turret unit script")] GameObject _prefabObject;
        [SerializeField] GameObject _projectilePrefab;
        [SerializeField, Tooltip("Do not touch if you are certain you know what you are doing")] float _projectileSpeedMultiplier = 1;
        [SerializeField] AudioClip _GetDamagedSFX;
        [SerializeField] AudioClip _DeathSFX;
    }
}
