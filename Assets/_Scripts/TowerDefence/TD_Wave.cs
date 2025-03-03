using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Wave Data")]

    [System.Serializable]
    public class TD_Wave : Data<TD_Wave>
    {
        public void SetValues(List<TD_Enemy> enemies, List<float> cooldowns)
        {
            _enemies = new List<TD_EnemyWithCooldown>();

            if (cooldowns == null || cooldowns.Count == 0)
                cooldowns = new List<float>() { 0 };

            int cdCount = cooldowns.Count;

            for (int i = 0; i < enemies.Count; i++)
            {
                float cooldown = cooldowns[i < cdCount ? i : cdCount - 1];
                _enemies.Add(new TD_EnemyWithCooldown(enemies[i], cooldown));
            }

            TrySaveAssetIfInEditor();
        }
        public void SetNameAndID(string waveName, string waveID)
        {
            _displayName = waveName;
            _id = waveID;

            TrySaveAssetIfInEditor();
        }

        void TrySaveAssetIfInEditor()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        public List<TD_EnemyWithCooldown> Enemies => _enemies;
        [SerializeField] List<TD_EnemyWithCooldown> _enemies;

        public int TotalEnemyCount => Enemies.Count;
    }

    [System.Serializable]
    public class TD_EnemyWithCooldown
    {
        public TD_Enemy Enemy;
        public float Cooldown;

        public TD_EnemyWithCooldown(TD_Enemy enemy, float cooldown)
        {
            Enemy = enemy;
            Cooldown = cooldown;
        }
    }

    [System.Serializable]
    public class TD_Enemy
    {
        public EnemyData Enemy
        {
            get
            {
                if (_isWildCard) LockEnemyData();

                return _enemy;
            }
        }

        [SerializeField] EnemyData _enemy;
        [SerializeField] bool _isWildCard;
        [SerializeField] string _wildCardID;
        [SerializeField] string _wildCardValue;

        /// <summary>
        /// Create this as an enemy
        /// </summary>
        public TD_Enemy(EnemyData enemyData)
        {
            _enemy = enemyData;

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
        public TD_Enemy(string wildCardID, string wildCardValue)
        {
            _isWildCard = true;
            _wildCardID = wildCardID;
            _wildCardValue = wildCardValue;

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
    }
}
