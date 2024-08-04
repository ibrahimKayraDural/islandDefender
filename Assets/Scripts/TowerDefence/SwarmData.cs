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
            if (setTo.Waves == null || setTo.Waves.Count <= 0) return;

            Waves = setTo.Waves;
            DefaultEnemyCooldowns = setTo.DefaultEnemyCooldowns;
            DefaultUntillNextWave = setTo.DefaultUntillNextWave;

            if (DefaultEnemyCooldowns == null || DefaultEnemyCooldowns.Count <= 0) DefaultEnemyCooldowns = new List<float>() {0};
            if (DefaultUntillNextWave == null || DefaultUntillNextWave.Count <= 0) DefaultUntillNextWave = new List<int>() {0};

            //repeat last value untill sizes are the same
            int diff = Waves.Count - DefaultEnemyCooldowns.Count;
            for (int i = 0; i < diff; i++) DefaultEnemyCooldowns.Add(DefaultEnemyCooldowns[DefaultEnemyCooldowns.Count - 1]);
            DefaultEnemyCooldowns.RemoveRange(Waves.Count, (int)Mathf.Max(0, DefaultEnemyCooldowns.Count - Waves.Count));

            diff = Waves.Count - DefaultUntillNextWave.Count;
            for (int i = 0; i < diff; i++) DefaultUntillNextWave.Add(DefaultUntillNextWave[DefaultUntillNextWave.Count - 1]);
            DefaultUntillNextWave.RemoveRange(Waves.Count, (int)Mathf.Max(0, DefaultUntillNextWave.Count - Waves.Count));
        }

        public void InsertSwarmValues(SwarmData insertion, int insertAt)
        {
            if (insertion == null) return;

            Refresh();
            insertion.Refresh();
            insertAt = Mathf.Max(insertAt, 0);
            int oldCount = insertion.Waves.Count;

            if (insertAt >= oldCount)
            {
                Waves.AddRange(insertion.Waves);
                DefaultEnemyCooldowns.AddRange(insertion.DefaultEnemyCooldowns);
                DefaultUntillNextWave.AddRange(insertion.DefaultUntillNextWave);
            }
            else
            {
                Waves.InsertRange(insertAt, insertion.Waves);
                DefaultEnemyCooldowns.InsertRange(insertAt, insertion.DefaultEnemyCooldowns);
                DefaultUntillNextWave.InsertRange(insertAt, insertion.DefaultUntillNextWave);
            }
        }

        public void Refresh() => SetSwarmValues(this);
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
