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

            List<KeyValuePair<S_EnemyWithCount, int>> enemiesWithLanes = new List<KeyValuePair<S_EnemyWithCount, int>>();

            for (int i = 0; i < _currentWave.Lanes.Count; i++)
            {
                S_LaneGroup lane = _currentWave.Lanes[i];

                for (int n = 0; n < lane.Enemies.Count; n++)
                {
                    //register enemy with a lane
                    enemiesWithLanes.Add(new KeyValuePair<S_EnemyWithCount, int>(lane.Enemies[n], i));
                }
            }

            List<float> cooldownArr = _currentWave.UseCustomCooldowns ? _currentWave.WaveCooldowns : _waveDatabase.DefaultCooldowns;
            int cooldownArrCount = cooldownArr.Count;

            for (int i = 0; enemiesWithLanes.Count > 0 && i <= 10000; i++)
            {
                SpawnNextWave(ref enemiesWithLanes);

                yield return new WaitForSeconds(cooldownArrCount > i ? cooldownArr[i] : cooldownArr[cooldownArrCount-1]);

                if (i == 10000) Debug.LogError("Failsafe cap was reached while spawning waves");
            }
            _waveIsActive = false;
            Debug.Log("wave has ended");
        }

        void SpawnNextWave(ref List<KeyValuePair<S_EnemyWithCount, int>> currentWave)
        {
            List<int> laneIndexes = new List<int>();
            foreach (var item in currentWave)
            {
                if (laneIndexes.Contains(item.Value)) continue;
                laneIndexes.Add(item.Value);
            }

            int laneInt = laneIndexes[UnityEngine.Random.Range(0, laneIndexes.Count)];

            List<S_EnemyWithCount> enemyList = new List<S_EnemyWithCount>();
            foreach (var ewcPair in currentWave.FindAll(x => x.Value == laneInt)) for (int i = 0; i < ewcPair.Key.Count; i++) enemyList.Add(ewcPair.Key);

            int rando = UnityEngine.Random.Range(0, enemyList.Count);
            S_EnemyWithCount selectedEnemy = enemyList[rando];

            int selectedIndex = currentWave.FindIndex(new Predicate<KeyValuePair<S_EnemyWithCount, int>>(x => x.Key.Equals(selectedEnemy) && x.Value == laneInt));

            //spawn(selectedEnemy.Enemy);

            currentWave[selectedIndex] = new KeyValuePair<S_EnemyWithCount, int>(new S_EnemyWithCount(selectedEnemy.Enemy, selectedEnemy.Count - 1), laneInt);
            if (currentWave[selectedIndex].Key.Count <= 0) currentWave.RemoveAt(selectedIndex);
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
