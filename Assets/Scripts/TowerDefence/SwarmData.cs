using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;

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
                List<S_Wave> waves = new List<S_Wave>();
                for (int i = 0; i < Waves.Count; i++)
                {
                    waves.Add((S_Wave)Waves[i].Clone());
                }

                return new SwarmDataValueContainer(waves, DefaultEnemyCooldowns, DefaultWaveCooldowns);
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

            Waves = new List<S_Wave>();
            for (int i = 0; i < setTo.Waves.Count; i++)
            {
                Waves.Add((S_Wave)setTo.Waves[i].Clone());
            }

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

    [System.Serializable] public struct S_Wave : ICloneable
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

        public object Clone()
        {
            S_Wave wave = (S_Wave)this.MemberwiseClone();
            wave.Lanes = new List<S_LaneGroup>();
            for (int i = 0; i < Lanes.Count; i++)
            {
                wave.Lanes.Add((S_LaneGroup)Lanes[i].Clone());
            }
            return wave as object;
        }
    }

    [System.Serializable] public struct S_LaneGroup : ICloneable
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

        public object Clone()
        {
            S_LaneGroup lane = (S_LaneGroup)this.MemberwiseClone();
            lane.Enemies = new List<S_EnemyWithCount>();
            for (int i = 0; i < Enemies.Count; i++)
            {
                lane.Enemies.Add((S_EnemyWithCount)Enemies[i].Clone());
            }
            return lane as object;
        }
    }

    [System.Serializable] public struct S_EnemyWithCount : ICloneable
    {
        public EnemyData Enemy
        {
            get
            {
                if (_isWildCard) LockEnemyData();

                return _enemy;
            }
        }

        public int Count;

        [SerializeField] EnemyData _enemy;
        [SerializeField] bool _isWildCard;
        [SerializeField] string _wildCardID;
        [SerializeField] string _wildCardValue;

        /// <summary>
        /// Create this as an enemy
        /// </summary>
        public S_EnemyWithCount(EnemyData enemyData, int count)
        {
            _enemy = enemyData;
            Count = count;

            _isWildCard = false;
            _wildCardID = GLOBAL.UnassignedString;
            _wildCardValue = GLOBAL.UnassignedString;

//#if UNITY_EDITOR
//            EditorUtility.SetDirty(_enemy);
//#endif
        }

        /// <summary>
        /// Create this as a wildcard
        /// </summary>
        public S_EnemyWithCount(string wildCardID, string wildCardValue, int count)
        {
            _isWildCard = true;
            _wildCardID = wildCardID;
            _wildCardValue = wildCardValue;
            Count = count;

            _enemy = null;
        }

        public void LockEnemyData()
        {
            if (_isWildCard == false) return;

            EnemyData data = EnemyData.HandleWildCard(_wildCardID, _wildCardValue);

            if (data == null)
            {
                Debug.LogError("Wild card is not valid");
                return;
            }
            _enemy = data;

            _isWildCard = false;
            _wildCardID = GLOBAL.UnassignedString;
            _wildCardValue = GLOBAL.UnassignedString;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
