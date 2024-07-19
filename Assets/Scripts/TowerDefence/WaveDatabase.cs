using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Wave Manager")]
    public class WaveDatabase : ScriptableObject
    {
        public S_Wave[] Waves;
        public List<int> DefaultCooldowns = new List<int>();
    }

    [System.Serializable] public struct S_Wave
    {
        public List<S_LaneGroup> Lanes;
        public List<int> WaveCooldowns;
        public bool UseCustomCooldowns;
    }

    [System.Serializable] public struct S_LaneGroup
    {
        public List<S_EnemyWithCount> Enemies;
    }

    [System.Serializable] public struct S_EnemyWithCount
    {
        public EnemyData Enemy;
        public int count;
    }
}
