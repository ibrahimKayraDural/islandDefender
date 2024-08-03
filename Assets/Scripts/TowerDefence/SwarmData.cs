using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Swarm Data")]
    public class SwarmData : Data<SwarmData>
    {
        public List<S_Wave> Waves;
        public List<float> DefaultEnemyCooldowns = new List<float>();
        public List<int> DefaultUntillNextWave = new List<int>();

        public string DisplayNameAccess
        {
            get => _displayName;
            set => _displayName = value;
        }
        public string IDAccess
        {
            get => _id;
            set => _id = value;
        }

        public void SetNameAndID(string targetName)
        {
            string targetID = targetName;
            targetID = targetID.Trim().Replace(" ", "-").ToLower();

            _displayName = targetName;
            _id = targetID;
        }


        /// <summary>
        /// <para>Will set 'Waves' and 'DefaultCooldowns' values.</para>
        /// <para>Does not change 'DisplayName' or 'ID' </para>
        /// </summary>
        public void SetSwarmValues(SwarmData setTo)
        {
            Waves = setTo.Waves;
            DefaultEnemyCooldowns = setTo.DefaultEnemyCooldowns;
            DefaultUntillNextWave = setTo.DefaultUntillNextWave;
        }
    }

    [System.Serializable] public struct S_Wave
    {
        public List<S_LaneGroup> Lanes;

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
