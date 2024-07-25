using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TowerDefence
{
    public class SpawnManager : MonoBehaviour
    {
        static List<GameObject> ActiveEnemies = new List<GameObject>();
        static event EventHandler<string> e_ActiveEnemiesListIsEmptied;
        static bool _waveEndedOnce;

        [SerializeField] bool _WaitUntillEnemiesAreDead = true;
        [SerializeField] bool _RepeatLastWave;
        [SerializeField] List<Transform> _spawners = new List<Transform>();
        [SerializeField] BaseManager _BaseMngr;
        [SerializeField] WaveDatabase _waveDatabase;
        [SerializeField] TextMeshProUGUI _TimeTM;

        int _currentWaveIndex;
        S_Wave? _currentWave
        {
            get
            {
                if (_waveDatabase == null || _waveDatabase.Waves.Length <= _currentWaveIndex)
                    return null;

                return _waveDatabase.Waves[_currentWaveIndex];
            }
        }
        int _waveCount => _waveDatabase.Waves.Length;
        string spawnerName = "EnemySpawner";
        bool _waveIsActive;

        private void Start()
        {
            e_ActiveEnemiesListIsEmptied += OnWaveEnded;
            _BaseMngr.e_BaseHasDied += _BaseMngr_e_BaseHasDied;
            _TimeTM.text = "";
            StartWave();
        }

        void _BaseMngr_e_BaseHasDied(object sender, EventArgs e)
        {
            StopWave();
        }

        public static bool RemoveFromActiveEnemyList(GameObject target)
        {
            bool didRemove = ActiveEnemies.Remove(target);
            if (ActiveEnemies.Count <= 0 && _waveEndedOnce == false) e_ActiveEnemiesListIsEmptied?.Invoke(typeof(SpawnManager), "listEmpty");
            return didRemove;
        }

        public void StartWave()
        {
            if (_waveIsActive) return;
            if (_currentWave == null)
            {
                Debug.LogError("_currentWave is null");
                return;
            }

            _waveEndedOnce = false;
            StartCoroutine(nameof(WaveCoroutine));
        }
        public void StopWave()
        {
            if (_waveIsActive == false) return;

            StopCoroutine(nameof(WaveCoroutine));
            _waveIsActive = false;
        }

        IEnumerator WaveCoroutine()
        {
            _waveIsActive = true;

            List<KeyValuePair<S_EnemyWithCount, int>> enemiesWithLanes = new List<KeyValuePair<S_EnemyWithCount, int>>();


            //instantiating wave data
            List<int> laneIndexes = new List<int>();
            for (int i = 0; i < _currentWave.Value.Lanes.Count; i++)
            {
                S_LaneGroup lane = _currentWave.Value.Lanes[i];

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
            List<float> cooldownArr = _currentWave.Value.UseCustomCooldowns ? _currentWave.Value.WaveCooldowns : _waveDatabase.DefaultCooldowns;
            int cooldownArrCount = cooldownArr.Count;


            //looping untill either no data is left in wave data or the failsafe cap is reached
            for (int i = 0; enemiesWithLanes.Count > 0 && i <= 10000; i++)
            {
                SpawnNextWave(ref enemiesWithLanes);

                yield return new WaitForSeconds(cooldownArrCount > i ? cooldownArr[i] : cooldownArr[cooldownArrCount-1]);

                if (i == 10000) Debug.LogError("Failsafe cap was reached while spawning waves");
            }

            _waveIsActive = false;
            if ((_waveEndedOnce == false && ActiveEnemies.Count <= 0) || _WaitUntillEnemiesAreDead == false) OnWaveEnded(this, "waveEnd");
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
            prefab = Instantiate(prefab, _spawners[laneInt].position, prefab.transform.rotation);
            ActiveEnemies.Add(prefab);

            currentWave[selectedIndex] = new KeyValuePair<S_EnemyWithCount, int>(new S_EnemyWithCount(selectedEnemy.Enemy, selectedEnemy.Count - 1), laneInt);
            if (currentWave[selectedIndex].Key.Count <= 0) currentWave.RemoveAt(selectedIndex);
        }

        void OnWaveEnded(object sender, string senderID)
        {
            if (_waveIsActive) return;

            if (senderID == "listEmpty" && _WaitUntillEnemiesAreDead == false) return;

            _waveEndedOnce = true;

            if (_currentWaveIndex + 1 >= _waveCount && _RepeatLastWave == false)
            {
                AllWavesEnded();
                return;
            }

            int cooldown = _currentWave.Value.TimeUntillNextWave;
            if (_currentWaveIndex + 1 < _waveCount) _currentWaveIndex++;

            StartCoroutine(nameof(RunWaveCooldown), cooldown);
        }

        IEnumerator RunWaveCooldown(int cooldown)
        {
            while(true)
            {
                if (cooldown <= 0) break;
                _TimeTM.text = $"Untill Next Wave : {cooldown}";
                yield return new WaitForSeconds(1);
                cooldown--;
            }
            _TimeTM.text = "";
            StartWave();
        }

        void AllWavesEnded()
        {
            _TimeTM.text = "All Waves are finished";
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
