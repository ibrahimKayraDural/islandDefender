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
        public event EventHandler<SwarmDataValueContainer> e_ValuesHaveChanged;
        public SwarmDataValueContainer AsValue
        {
            get
            {
                List<S_Wave> tempWave = Waves;
                List<float> tempDEC = DefaultEnemyCooldowns;
                List<int> tempDWC = DefaultWaveCooldowns;
                return new SwarmDataValueContainer(tempWave, tempDEC, tempDWC);
            }
        }

        public List<S_Wave> Waves = new List<S_Wave>();
        public List<float> DefaultEnemyCooldowns = new List<float>();
        public List<int> DefaultWaveCooldowns = new List<int>();

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
            targetID = targetID.Trim().Replace(" ", "-").ToLower(new System.Globalization.CultureInfo("en-US"));

            _displayName = targetName;
            _id = targetID;
        }
        public void SetNameAndID(string targetName, string targetID)
        {
            _displayName = targetName;
            _id = targetID;
        }


        /// <summary>
        /// <para>Will set 'Waves' and 'DefaultCooldowns' values.</para>
        /// <para>Does not change 'DisplayName' or 'ID' </para>
        /// </summary>
        public void SetSwarmValues(SwarmDataValueContainer setTo, bool invokeEvent = true)
        {
            if (setTo.Waves == null || setTo.Waves.Count <= 0) return;

            Waves = setTo.Waves;
            DefaultEnemyCooldowns = setTo.DefaultEnemyCooldowns;
            DefaultWaveCooldowns = setTo.DefaultWaveCooldowns;

            if (DefaultEnemyCooldowns == null || DefaultEnemyCooldowns.Count <= 0) DefaultEnemyCooldowns = new List<float>() { 0 };
            if (DefaultWaveCooldowns == null || DefaultWaveCooldowns.Count <= 0) DefaultWaveCooldowns = new List<int>() { 0 };

            //repeat last value untill sizes are the same
            int waveCount = Waves.Count;

            int diff = waveCount - DefaultEnemyCooldowns.Count;
            for (int i = 0; i < diff; i++) DefaultEnemyCooldowns.Add(DefaultEnemyCooldowns[DefaultEnemyCooldowns.Count - 1]);
            DefaultEnemyCooldowns = DefaultEnemyCooldowns.GetRange(0, waveCount);

            diff = waveCount - DefaultWaveCooldowns.Count;
            for (int i = 0; i < diff; i++) DefaultWaveCooldowns.Add(DefaultWaveCooldowns[DefaultWaveCooldowns.Count - 1]);
            DefaultWaveCooldowns = DefaultWaveCooldowns.GetRange(0, waveCount);


#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif

            if (invokeEvent) e_ValuesHaveChanged?.Invoke(this, AsValue);
        }

        public void Refresh() => SetSwarmValues(AsValue, false);
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
