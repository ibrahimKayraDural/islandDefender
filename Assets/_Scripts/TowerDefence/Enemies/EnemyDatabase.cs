namespace TowerDefence
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Tower Defence/Enemy Database")]
    public class EnemyDatabase : Database<EnemyData>
    {
        GameplayManager _GameplayManager
        {
            get
            {
                if (AUTO_GameplayManager == null)
                    AUTO_GameplayManager = GameplayManager.Instance;
                return AUTO_GameplayManager;
            }
        }
        GameplayManager AUTO_GameplayManager = null;

        public EnemyData[] GetAllEnemiesByRange(EnemyRange range) =>
            DataList.FindAll(x => x.RangeType == range && _GameplayManager.EnemyPool.Contains(x))?.ToArray();
        public EnemyData[] GetAllEnemiesByDifficulty(EnemyDifficulty difficulty) => 
            DataList.FindAll(x => x.Difficulty == difficulty && _GameplayManager.EnemyPool.Contains(x))?.ToArray();
        public EnemyData[] GetAllEnemiesByEnemyType(EnemyType type) => 
            DataList.FindAll(x => x.EnemyTypes.Contains(type) && _GameplayManager.EnemyPool.Contains(x))?.ToArray();
        public EnemyData GetRandomEnemyByRange(EnemyRange range) => ReturnRandom(GetAllEnemiesByRange(range));
        public EnemyData GetRandomEnemyByDifficulty(EnemyDifficulty difficulty) => ReturnRandom(GetAllEnemiesByDifficulty(difficulty));
        public EnemyData GetRandomEnemyByEnemyType(EnemyType type) => ReturnRandom(GetAllEnemiesByEnemyType(type));

        EnemyData ReturnRandom(EnemyData[] datas)
        {
            if (datas == null || datas.Length <= 0) return null;
            int random = Random.Range(0, datas.Length);
            return datas[random];
        }
    }
}