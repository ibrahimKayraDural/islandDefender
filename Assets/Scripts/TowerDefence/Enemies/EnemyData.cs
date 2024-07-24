using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public string DisplayName => _DisplayName;
        public string ID => _ID;
        public float MaxHealth => _MaxHealth;
        public float Speed => _Speed;
        public float Damage => _Damage;
        public float DamageToBase => _DamageToBase;
        public float AttackCooldown => _AttackCooldown;
        public float AttackRange => _AttackRange;
        public AudioClip AttackSFX => _AttackSFX;
        public AudioClip AttackToBaseSFX => _AttackToBaseSFX;
        public GameObject EnemyPrefab => _enemyPrefab;

        [SerializeField] string _DisplayName = GLOBAL.UnassignedString;
        [SerializeField] string _ID = GLOBAL.UnassignedString;
        [SerializeField, Min(.1f)] float _MaxHealth = 1;
        [SerializeField, Min(0)] float _Speed = 1;
        [SerializeField, Min(0)] float _Damage = 1;
        [SerializeField, Min(0)] float _DamageToBase = 1;
        [SerializeField, Min(0)] float _AttackCooldown = 1;
        [SerializeField, Min(0)] float _AttackRange = 1;
        [SerializeField] AudioClip _AttackSFX;
        [SerializeField] AudioClip _AttackToBaseSFX;
        [SerializeField] GameObject _enemyPrefab;
    } 
}
