using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] List<Transform> _spawners = new List<Transform>();
        [SerializeField] WaveDatabase _waveDatabase;

        int _currentWaveIndex;
        S_Wave _currentWave => _waveDatabase.Waves[_currentWaveIndex];
        string spawnerName = "EnemySpawner";

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


            //instantiating wave data
            List<int> laneIndexes = new List<int>();
            for (int i = 0; i < _currentWave.Lanes.Count; i++)
            {
                S_LaneGroup lane = _currentWave.Lanes[i];

                for (int n = 0; n < lane.Enemies.Count; n++)
                {
                    if (lane.Enemies[n].Count <= 0) continue;
                    if (lane.Enemies[n].Enemy == null) continue;
                    enemiesWithLanes.Add(new KeyValuePair<S_EnemyWithCount, int>(lane.Enemies[n], i));
                    if (laneIndexes.Contains(i) == false) laneIndexes.Add(i);
                }
            }


            //removing excess from waveData to fit the actual lane count
            for (int i = 0; i < laneIndexes.Count - _spawners.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, laneIndexes.Count);
                enemiesWithLanes.FindAll(x => x.Value == randomIndex).ForEach(y => enemiesWithLanes.Remove(y));
            }


            //Shuffling lanes (fuck this)
            List<int> spawnerIndexes = new List<int>();
            List<int> oldLaneIndexes = new List<int>();
            List<int> uniqueLaneIndexes = new List<int>();
            for (int i = 0; i < _spawners.Count; i++) spawnerIndexes.Add(i);
            for (int i = 0; i < enemiesWithLanes.Count; i++) oldLaneIndexes.Add(enemiesWithLanes[i].Value);
            for (int i = 0; i < enemiesWithLanes.Count; i++)
            {
                if (uniqueLaneIndexes.Contains(enemiesWithLanes[i].Value) == false)
                {
                    uniqueLaneIndexes.Add(enemiesWithLanes[i].Value);
                }
            }

            for (int i = 0; i < uniqueLaneIndexes.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0,spawnerIndexes.Count);

                for (int n = 0; n < enemiesWithLanes.Count; n++)
                {
                    if (oldLaneIndexes[n] == uniqueLaneIndexes[i])
                    {
                        enemiesWithLanes[n] = new KeyValuePair<S_EnemyWithCount, int>(enemiesWithLanes[n].Key, spawnerIndexes[randomIndex]);
                    }
                }

                spawnerIndexes.RemoveAt(randomIndex);
            }


            //registering cooldown values
            List<float> cooldownArr = _currentWave.UseCustomCooldowns ? _currentWave.WaveCooldowns : _waveDatabase.DefaultCooldowns;
            int cooldownArrCount = cooldownArr.Count;


            //looping untill either no data is left in wave data or the failsafe cap is reached
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

            GameObject prefab = selectedEnemy.Enemy.EnemyPrefab;
            Instantiate(prefab, _spawners[laneInt].position, prefab.transform.rotation);

            currentWave[selectedIndex] = new KeyValuePair<S_EnemyWithCount, int>(new S_EnemyWithCount(selectedEnemy.Enemy, selectedEnemy.Count - 1), laneInt);
            if (currentWave[selectedIndex].Key.Count <= 0) currentWave.RemoveAt(selectedIndex);
        }

        public void SpawnSpawnerAt(Vector3 position, Transform parent = null)
        {
            Transform temp = new GameObject(spawnerName).transform;
            if (parent != null) temp.parent = parent;
            temp.position = position;
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
