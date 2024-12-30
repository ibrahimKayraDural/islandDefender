using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Enemy Data")]
    public class EnemyData : GameplayElementData<EnemyData>
    {
        public float MaxHealth => _MaxHealth;
        public float Speed => _Speed;
        public float Damage => _Damage;
        public float AttackCooldown => _AttackCooldown;
        public float AttackRange => _AttackRange;
        public EnemyDifficulty Difficulty => _Difficulty;
        public EnemyRange RangeType => _RangeType;
        public List<EnemyType> EnemyTypes => _EnemyTypes;
        public AudioClip GettingDamagedSFX => _GettingDamagedSFX;
        public GameObject EnemyPrefab => _enemyPrefab;

        [SerializeField, Min(.1f)] float _MaxHealth = 1;
        [SerializeField, Min(0)] float _Speed = 1;
        [SerializeField, Min(0)] float _Damage = 1;
        [SerializeField, Min(0)] float _AttackCooldown = 1;
        [SerializeField, Min(0)] float _AttackRange = 1;
        [SerializeField] EnemyDifficulty _Difficulty;
        [SerializeField] EnemyRange _RangeType;
        [SerializeField] List<EnemyType> _EnemyTypes = new List<EnemyType>() { EnemyType.None };
        [SerializeField] AudioClip _GettingDamagedSFX;
        [SerializeField] GameObject _enemyPrefab;

        public static EnemyData HandleWildCard(string wildCardID, string wildCardValue)
        {
            EnemyDatabase enemyDB = GLOBAL.GetEnemyDatabase();
            if (enemyDB == null) return null;

            switch (wildCardID)
            {
                case "difficulty":
                    return enemyDB.GetRandomEnemyByDifficulty(GLOBAL.EnemyDifficultyIDs[wildCardValue]);

                case "range":
                    return enemyDB.GetRandomEnemyByRange(GLOBAL.EnemyRangeIDs[wildCardValue]);

                case "type":
                    return enemyDB.GetRandomEnemyByEnemyType(GLOBAL.EnemyTypeIDs[wildCardValue]);

                default: return null;
            }
        }
    } 

    // -- IMPORTANT, WHEN ADDING NEW VALUES: --
    // Do not forget to register ids after adding new values to these enums.
    // IDs are located in GLOBAL script. Add an id to the new enum in the corresponding dictionary.
    public enum EnemyDifficulty { Weak, Medium, Tough}
    public enum EnemyRange { Melee, Ranged, Special}
    public enum EnemyType { None, Durable, FastMoving, FastAttacking, HardHitting }
}
