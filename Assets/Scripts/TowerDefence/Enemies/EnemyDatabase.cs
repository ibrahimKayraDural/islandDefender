namespace TowerDefence
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Tower Defence/Enemy Database")]
    public class EnemyDatabase : Database<EnemyData>
    {
        public EnemyData[] GetAllEnemiesByRange(EnemyRange range) => DataList.FindAll(x => x.RangeType == range).ToArray();
        public EnemyData[] GetAllEnemiesByDifficulty(EnemyDifficulty difficulty) => DataList.FindAll(x => x.Difficulty == difficulty).ToArray();
        public EnemyData[] GetAllEnemiesByEnemyType(EnemyType type) => DataList.FindAll(x => x.EnemyTypes.Contains(type)).ToArray();
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