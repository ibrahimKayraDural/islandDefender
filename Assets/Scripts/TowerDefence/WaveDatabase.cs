using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Wave Manager")]
    public class WaveDatabase : ScriptableObject
    {
        public S_Wave[] Waves;
        public List<float> DefaultCooldowns = new List<float>();
    }

    [System.Serializable] public struct S_Wave
    {
        public List<S_LaneGroup> Lanes;
        public List<float> WaveCooldowns;
        public bool UseCustomCooldowns;

        public int TotalEnemyCount
        {
            get
            {
                int count = 0;
                foreach (var lane in Lanes)
                {
                    count += lane.TotalEnemyCount;
                }
                return count;
            }
        }
    }

    [System.Serializable] public struct S_LaneGroup
    {
        public List<S_EnemyWithCount> Enemies;

        public int TotalEnemyCount
        {
            get
            {
                int count = 0;
                foreach (var ec in Enemies)
                {
                    count += ec.Count;
                }
                return count;
            }
        }
    }

    [System.Serializable] public struct S_EnemyWithCount
    {
        public EnemyData Enemy;
        public int Count;

        public S_EnemyWithCount(EnemyData enemyData, int count)
        {
            Enemy = enemyData;
            Count = count;
        }
    }
}
