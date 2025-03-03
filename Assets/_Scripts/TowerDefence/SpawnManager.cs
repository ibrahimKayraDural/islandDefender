using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TowerDefence
{
    public class SpawnManager : MonoBehaviour
    {
        /*
        public static bool WaveIsActive { get; private set; } = false; 
        public void SetCooldownIsPaused(bool a){}
        public void SpawnSpawnerAt(Vector3 a, Transform b) { }
        public static void RemoveFromActiveEnemyList(GameObject a) { }
        public void DeleteSpawners() { }
        */
        static List<GameObject> ActiveEnemies = new();
        public static bool WaveIsActive { get; private set; }

        [SerializeField] List<Spawner> _spawners = new();
        [SerializeField] GameObject _SpawnerPrefab = null;
        [SerializeField] BaseManager _BaseMngr;
        [SerializeField] TDPlayerController _TDPlayerController;

        TD_WaveValue? _currentWave = null;
        int currentWaveIndex = 0;
        int _lastLaneIndex = -1;

        List<TD_EnemyWithCooldown> _currentEnemies = new();

        void Start()
        {
            //Get wave database
            var waveDB = GLOBAL.GetWaveDatabase();

            _BaseMngr.e_BaseHasDied += _BaseMngr_e_BaseHasDied;

            //Get current wave index here
            _currentWave = waveDB.DataList[currentWaveIndex].AsValue();

            SetWaveUp();

            //Use this method when you are ready. Usually hooked up to a button.
            //StartWave();
        }

        public static void RemoveFromActiveEnemyList(GameObject target)
        {
            ActiveEnemies.Remove(target);
        }

        void _BaseMngr_e_BaseHasDied(object sender, EventArgs e)
        {
            StopWave();

            foreach (var enemy in ActiveEnemies)
            {
                if (enemy.TryGetComponent(out EnemyBase eb))
                {
                    eb.Win();
                }
            }

            _BaseMngr.e_BaseHasDied -= _BaseMngr_e_BaseHasDied;
        }

        public void StartWave()
        {
            //SetIndicatorValues(null);

            if (WaveIsActive) return;
            if (_currentWave == null)
            {
                Debug.LogError("_currentWave is null");
                return;
            }

            WaveIsActive = true;
            _TDPlayerController.EvaluateGameplayMode(true);
            StartCoroutine(nameof(WaveCoroutine));
        }
        public void StopWave()
        {
            if (WaveIsActive == false) return;

            StopCoroutine(nameof(WaveCoroutine));
            WaveIsActive = false;
        }

        IEnumerator WaveCoroutine()
        {
            //looping untill either no data is left in wave data or the failsafe cap is reached
            for (int i = 0; i < _currentEnemies.Count; i++)
            {
                var ewl = _currentEnemies[i];
                SpawnNextEnemy(ewl);

                yield return new WaitForSeconds(ewl.Cooldown);
            }

            yield return new WaitUntil(() => ActiveEnemies.Count <= 0);

            OnWaveEnded();
        }
        void SetWaveValues()
        {
            _currentEnemies = new();

            for (int i = 0; i < _currentWave.Value.Enemies.Count; i++)
            {
                var item = _currentWave.Value.Enemies[i];
                item.LockEnemy();
                _currentEnemies.Add(item);
            }

            ////instantiating wave data
            //List<int> laneIndexes = new();
            //for (int i = 0; i < _currentWave.Value.Enemies.Count; i++)
            //{
            //    S_LaneGroup lane = _currentWave.Value.Enemies[i];

            //    for (int n = 0; n < lane.Enemies.Count; n++)
            //    {

            //        //locking enemies

            //        lane.Enemies[n] = new S_EnemyWithCount(lane.Enemies[n].Enemy, lane.Enemies[n].Count);

            //        //locking enemies end


            //        if (lane.Enemies[n].Count <= 0) continue;
            //        if (lane.Enemies[n].Enemy == null) continue;

            //        _enemiesWithLanes.Add(new KeyValuePair<S_EnemyWithCount, int>(lane.Enemies[n], i));
            //        if (laneIndexes.Contains(i) == false) laneIndexes.Add(i);
            //    }
            //}


            ////removing excess from waveData to fit the actual lane count
            //for (int i = 0; i < laneIndexes.Count - _spawners.Count; i++)
            //{
            //    int randomIndex = UnityEngine.Random.Range(0, laneIndexes.Count);
            //    _enemiesWithLanes.FindAll(x => x.Value == randomIndex).ForEach(y => _enemiesWithLanes.Remove(y));
            //}


            ////Shuffling lanes
            //List<int> spawnerIndexes = new();
            //List<int> oldLaneIndexes = new();
            //List<int> uniqueLaneIndexes = new();
            //for (int i = 0; i < _spawners.Count; i++) spawnerIndexes.Add(i);
            //for (int i = 0; i < _enemiesWithLanes.Count; i++) oldLaneIndexes.Add(_enemiesWithLanes[i].Value);
            //for (int i = 0; i < _enemiesWithLanes.Count; i++)
            //{
            //    if (uniqueLaneIndexes.Contains(_enemiesWithLanes[i].Value) == false)
            //    {
            //        uniqueLaneIndexes.Add(_enemiesWithLanes[i].Value);
            //    }
            //}

            //for (int i = 0; i < uniqueLaneIndexes.Count; i++)
            //{
            //    int randomIndex = UnityEngine.Random.Range(0, spawnerIndexes.Count);

            //    for (int n = 0; n < _enemiesWithLanes.Count; n++)
            //    {
            //        if (oldLaneIndexes[n] == uniqueLaneIndexes[i])
            //        {
            //            _enemiesWithLanes[n] = new KeyValuePair<S_EnemyWithCount, int>(_enemiesWithLanes[n].Key, spawnerIndexes[randomIndex]);
            //        }
            //    }

            //    spawnerIndexes.RemoveAt(randomIndex);
            //}

            ////registering cooldown values
            //_enemyCooldownArr = CurrentSwarm.DefaultEnemyCooldowns;
            //if (_enemyCooldownArr == null || _enemyCooldownArr.Count <= 0) _enemyCooldownArr = GLOBAL.FailsafeEnemyCooldowns;
            //_enemyCooldownArrCount = _enemyCooldownArr.Count;
        }

        void SpawnNextEnemy(TD_EnemyWithCooldown enemy)
        {
            int laneMax = _spawners.Count;
            int i = 0;

            for (int n = 0; n < 3; n++)
            {
                i = UnityEngine.Random.Range(0, laneMax);
                if (i != _lastLaneIndex) break;
            }
            _lastLaneIndex = i;

            GameObject prefab = enemy.Enemy.Enemy.EnemyPrefab;
            prefab = Instantiate(prefab, _spawners[i].Position, prefab.transform.rotation);
            ActiveEnemies.Add(prefab);
        }

        void OnWaveEnded()
        {
            if (WaveIsActive == false) return;
            WaveIsActive = false;

            Debug.Log("Wave has ended");
        }

        //void SetIndicatorValues(bool? toNull = false)
        //{
        //    if (toNull == null || toNull.Value)
        //    {
        //        foreach (var item in _spawners) item.SetEnemyIndicators(null);

        //        return;
        //    }

        //    //First is lane second is enemies in it
        //    List<List<EnemyData>> enemies = new();

        //    for (int i = 0; i < _spawners.Count; i++)
        //    {
        //        enemies.Add(null);
        //    }

        //    foreach (var item in _currentEnemies)
        //    {
        //        int laneInt = item.Value;
        //        if (laneInt >= enemies.Count) continue;

        //        EnemyData data = item.Key.Enemy.Enemy;

        //        if (enemies[laneInt] == null)
        //        {
        //            enemies[laneInt] = new List<EnemyData>();
        //        }

        //        if (enemies[laneInt].Contains(data) == false)
        //        {
        //            enemies[laneInt].Add(data);
        //        }
        //    }

        //    for (int i = 0; i < _spawners.Count; i++)
        //    {
        //        _spawners[i].SetEnemyIndicators(enemies[i]?.ToArray());
        //    }
        //}

        void SetWaveUp()
        {
            SetWaveValues();
            //SetIndicatorValues();
            _TDPlayerController.EvaluateGameplayMode(false);
        }

        public void SpawnSpawnerAt(Vector3 position, Transform parent = null)
        {
            Transform temp = Instantiate(_SpawnerPrefab).transform;
            if (parent != null) temp.parent = parent;
            temp.position = position;

            _spawners.Add(temp.GetComponent<Spawner>());
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
            _spawners = new List<Spawner>();
        }
    }
}
