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
        public AudioClip AttackSFX => _AttackSFX;
        public GameObject EnemyPrefab => _enemyPrefab;

        [SerializeField, Min(.1f)] float _MaxHealth = 1;
        [SerializeField, Min(0)] float _Speed = 1;
        [SerializeField, Min(0)] float _Damage = 1;
        [SerializeField, Min(0)] float _AttackCooldown = 1;
        [SerializeField, Min(0)] float _AttackRange = 1;
        [SerializeField] AudioClip _AttackSFX;
        [SerializeField] GameObject _enemyPrefab;
    } 
}
