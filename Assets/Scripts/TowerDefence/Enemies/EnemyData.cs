using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Enemy Data")]
    public class EnemyData : Data<EnemyData>
    {
        public float MaxHealth => _MaxHealth;
        public float Speed => _Speed;
        public float Damage => _Damage;
        public float AttackCooldown => _AttackCooldown;
        public float AttackRange => _AttackRange;
        public EnemyDifficulty Difficulty => _Difficulty;
        public EnemyRange RangeType => _RangeType;
        public List<EnemyType> EnemyTypes => _EnemyTypes;
        public AudioClip AttackSFX => _AttackSFX;
        public GameObject EnemyPrefab => _enemyPrefab;

        [SerializeField, Min(.1f)] float _MaxHealth = 1;
        [SerializeField, Min(0)] float _Speed = 1;
        [SerializeField, Min(0)] float _Damage = 1;
        [SerializeField, Min(0)] float _AttackCooldown = 1;
        [SerializeField, Min(0)] float _AttackRange = 1;
        [SerializeField] EnemyDifficulty _Difficulty;
        [SerializeField] EnemyRange _RangeType;
        [SerializeField] List<EnemyType> _EnemyTypes = new List<EnemyType>() { EnemyType.None };
        [SerializeField] AudioClip _AttackSFX;
        [SerializeField] GameObject _enemyPrefab;
    } 

    // -- IMPORTANT, WHEN ADDING NEW VALUES: --
    // Do not forget to register ids after adding new values to these enums.
    // IDs are located in GLOBAL script. Add an id to the new enum in the corresponding dictionary.
    public enum EnemyDifficulty { Weak, Medium, Tough}
    public enum EnemyRange { Melee, Ranged, Special}
    public enum EnemyType { None, Durable, FastMoving, FastAttacking, HardHitting }
}
