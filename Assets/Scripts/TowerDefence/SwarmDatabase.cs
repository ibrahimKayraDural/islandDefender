namespace TowerDefence
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Tower Defence/Swarm Database")]
    public class SwarmDatabase : Database<SwarmData>
    {
        public List<Data<SwarmData>> DataListAccess
        {
            get => _DataList;
            set => _DataList = value;
        }

        /// <summary>
        /// Insert values to swarmData at index
        /// </summary>
        /// <param name="swarmIndex">swarm index</param>
        /// <param name="insertion">swarm values</param>
        /// <param name="insertAt">insertion index. put -1 for current position</param>
        public void InsertSwarmValues(int swarmIndex, SwarmDataValueContainer insertion, int insertAt)
        {
            if (insertion == null) return;
            if (DataList.Count <= swarmIndex) return;

            var target = DataList[swarmIndex].AsValue;

            target.Refresh();
            insertion.Refresh();
            int oldCount = target.Waves.Count;

            if (insertAt == -1) insertAt = SpawnManager.CurrentWaveIndex + 1;

            if (insertAt >= oldCount || insertAt < 0)
            {
                target.Waves.AddRange(insertion.Waves);
                target.DefaultEnemyCooldowns.AddRange(insertion.DefaultEnemyCooldowns);
                target.DefaultWaveCooldowns.AddRange(insertion.DefaultWaveCooldowns);
            }
            else
            {
                target.Waves.InsertRange(insertAt, insertion.Waves);
                target.DefaultEnemyCooldowns.InsertRange(insertAt, insertion.DefaultEnemyCooldowns);
                target.DefaultWaveCooldowns.InsertRange(insertAt, insertion.DefaultWaveCooldowns);
            }

            DataList[swarmIndex].SetSwarmValues(target);
        }
    }
}