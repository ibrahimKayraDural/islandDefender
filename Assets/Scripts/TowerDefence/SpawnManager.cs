using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class SpawnManager : MonoBehaviour
    {
        List<Transform> _spawners = new List<Transform>();
        WaveDatabase _waveDatabase;
        int _currentWaveIndex;
        S_Wave _currentWave => _waveDatabase.Waves[_currentWaveIndex];

        void Awake()
        {
            _waveDatabase = GLOBAL.GetWaveDatabase();
        }

        private void Start()
        {
            StartWave();
        }

        bool _waveIsActive;
        public void StartWave()
        {
            if (_waveIsActive) return;

            StartCoroutine(nameof(WaveCoroutine));
        }
        public void StopWave()
        {
            _waveIsActive = false;
            StopCoroutine(nameof(WaveCoroutine));
        }
        IEnumerator WaveCoroutine()
        {
            _waveIsActive = true;

            S_Wave tempWave = _currentWave;
            List<int> cooldownArr = tempWave.UseCustomCooldowns ? tempWave.WaveCooldowns : _waveDatabase.DefaultCooldowns;
            int totalEnemyCount = 0;
            foreach (var item in _currentWave.Lanes)
            {
                item.Enemies.ForEach(x => totalEnemyCount += x.count);
            }

            int cooldownArrCount = cooldownArr.Count;
            for (int i = 0; i < totalEnemyCount; i++)
            {
                tempWave = SpawnNextWave(tempWave);
                yield return new WaitForSeconds(cooldownArrCount > i ? cooldownArr[i] : cooldownArr[cooldownArrCount-1]);
            }
            _waveIsActive = false;
            Debug.Log("wave has ended");
        }

        S_Wave SpawnNextWave(S_Wave waveToEat)
        {
            List<EnemyData> enemyDatas = new List<EnemyData>();
            foreach (var lane in waveToEat.Lanes)
            {
                foreach (var eCount in lane.Enemies)
                {
                    for (int i = 0; i < eCount.count; i++)
                    {
                        enemyDatas.Add(eCount.Enemy);
                    }
                }
            }

            int randomInt = UnityEngine.Random.Range(0, enemyDatas.Count);

            return waveToEat;
        }

        public void SpawnSpawnerAt(Vector3 position, Transform parent = null)
        {
            Transform temp = new GameObject($"Spawner{_spawners.Count + 1}").transform;
            if (parent != null) temp.parent = parent;
            temp.position = position;
            _spawners.Add(temp);
        }
        public void DeleteSpawners()
        {
            foreach (var target in _spawners)
            {
                if (target == null) continue;

                if (Application.isEditor)
                    DestroyImmediate(target.gameObject);
                else
                    Destroy(target.gameObject);
            }
            _spawners = new List<Transform>();
        }
    } 
}
