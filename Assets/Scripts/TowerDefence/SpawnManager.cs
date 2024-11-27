using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TowerDefence
{
    public class SpawnManager : MonoBehaviour
    {
        public static SwarmDataValueContainer CurrentSwarm;

        static List<GameObject> ActiveEnemies = new List<GameObject>();
        static event EventHandler<string> e_ActiveEnemiesListIsEmptied;
        public static bool WaveIsActive { get; private set; }

        [SerializeField, Min(0)] int _StartCooldown = 2;
        [SerializeField] bool _WaitUntillEnemiesAreDead = true;
        [SerializeField] bool _RepeatLastWave;
        [SerializeField] bool _StartPaused;
        [SerializeField] List<Spawner> _spawners = new List<Spawner>();
        [SerializeField] GameObject _SpawnerPrefab = null;
        [SerializeField] BaseManager _BaseMngr;
        [SerializeField] TextMeshProUGUI _TimeTM;
        [SerializeField] TDPlayerController _TDPlayerController;

        bool _isPaused => _cooldownSpeedMultiplier == 0;
        int _waveCount => CurrentSwarm.Waves.Count;
        int _currentWaveCooldown
        {
            get
            {
                return _currentWaveIndex < _waveCooldownArr.Count && _currentWaveIndex >= 0 ? _waveCooldownArr[_currentWaveIndex] : _waveCooldownArr[_waveCooldownArr.Count - 1];
            }
        }
        S_Wave? _currentWave
        {
            get
            {
                if (CurrentSwarm == null || CurrentSwarm.Waves.Count <= _currentWaveIndex)
                    return null;

                return CurrentSwarm.Waves[_currentWaveIndex];
            }
        }

        List<KeyValuePair<S_EnemyWithCount, int>> _enemiesWithLanes = new List<KeyValuePair<S_EnemyWithCount, int>>();
        List<float> _enemyCooldownArr = new List<float>();
        int _enemyCooldownArrCount;

        List<int> _waveCooldownArr = GLOBAL.FailsafeWaveCooldowns;
        SwarmDataValueContainer _changedSwarm = null;
        int _currentWaveIndex;
        float _cooldownSpeedMultiplier = 1;

        void Awake()
        {
            foreach (var item in _spawners)
            {
                item.SetEnemyIndicators(null);
            }
        }

        private void Start()
        {
            SwarmDatabase sdb = GLOBAL.GetSwarmDatabase();

            SwarmData sd = sdb.DataList[0] as SwarmData;
            SwarmData temp = ScriptableObject.CreateInstance<SwarmData>();
            temp.SetSwarmValues(sd.AsValue, false);

            CurrentSwarm = temp.AsValue;
            CurrentSwarm.e_ValuesHaveChanged += OnCurrentSwarmValuesHaveChanged;

            e_ActiveEnemiesListIsEmptied += OnWaveEnded;
            _BaseMngr.e_BaseHasDied += _BaseMngr_e_BaseHasDied;
            _TimeTM.text = "";

            StartCoroutine(RunWaveCooldown(_StartCooldown));
            SetCooldownIsPaused(_StartPaused);
        }

        void OnCurrentSwarmValuesHaveChanged(object sender, SwarmDataValueContainer e)
        {
            Debug.Log($"{sender.ToString()} has changed values.");
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

        public static bool RemoveFromActiveEnemyList(GameObject target)
        {
            bool didRemove = ActiveEnemies.Remove(target);
            if (ActiveEnemies.Count <= 0)
            {
                e_ActiveEnemiesListIsEmptied?.Invoke(typeof(SpawnManager), "listEmpty");
            }
            return didRemove;
        }

        public void StartWave()
        {
            if (WaveIsActive) return;
            if (_currentWave == null)
            {
                Debug.LogError("_currentWave is null");
                return;
            }

            _waveCooldownArr = CurrentSwarm.DefaultWaveCooldowns;
            if (_waveCooldownArr == null || _waveCooldownArr.Count <= 0) _waveCooldownArr = GLOBAL.FailsafeWaveCooldowns;

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
        public void SetCooldownIsPaused(bool setTo) => _cooldownSpeedMultiplier = setTo ? 0 : 1;


        IEnumerator WaveCoroutine()
        {
            //looping untill either no data is left in wave data or the failsafe cap is reached
            for (int i = 0; _enemiesWithLanes.Count > 0 && i <= 10000; i++)
            {
                SpawnNextWave(ref _enemiesWithLanes);

                yield return new WaitForSeconds(_enemyCooldownArrCount > i ? _enemyCooldownArr[i] : _enemyCooldownArr[_enemyCooldownArrCount - 1]);

                if (i == 10000) Debug.LogError("Failsafe cap was reached while spawning waves");
            }

            if (ActiveEnemies.Count <= 0 || _WaitUntillEnemiesAreDead == false) OnWaveEnded(this, "waveEnd");
        }
        void SetWaveUp()
        {
            _enemiesWithLanes = new List<KeyValuePair<S_EnemyWithCount, int>>();

            //instantiating wave data
            List<int> laneIndexes = new List<int>();
            for (int i = 0; i < _currentWave.Value.Lanes.Count; i++)
            {
                S_LaneGroup lane = _currentWave.Value.Lanes[i];

                for (int n = 0; n < lane.Enemies.Count; n++)
                {

                    //locking enemies

                    lane.Enemies[n] = new S_EnemyWithCount(lane.Enemies[n].Enemy, lane.Enemies[n].Count);

                    //locking enemies end


                    if (lane.Enemies[n].Count <= 0) continue;
                    if (lane.Enemies[n].Enemy == null) continue;

                    _enemiesWithLanes.Add(new KeyValuePair<S_EnemyWithCount, int>(lane.Enemies[n], i));
                    if (laneIndexes.Contains(i) == false) laneIndexes.Add(i);
                }
            }


            //removing excess from waveData to fit the actual lane count
            for (int i = 0; i < laneIndexes.Count - _spawners.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, laneIndexes.Count);
                _enemiesWithLanes.FindAll(x => x.Value == randomIndex).ForEach(y => _enemiesWithLanes.Remove(y));
            }


            //Shuffling lanes
            List<int> spawnerIndexes = new List<int>();
            List<int> oldLaneIndexes = new List<int>();
            List<int> uniqueLaneIndexes = new List<int>();
            for (int i = 0; i < _spawners.Count; i++) spawnerIndexes.Add(i);
            for (int i = 0; i < _enemiesWithLanes.Count; i++) oldLaneIndexes.Add(_enemiesWithLanes[i].Value);
            for (int i = 0; i < _enemiesWithLanes.Count; i++)
            {
                if (uniqueLaneIndexes.Contains(_enemiesWithLanes[i].Value) == false)
                {
                    uniqueLaneIndexes.Add(_enemiesWithLanes[i].Value);
                }
            }

            for (int i = 0; i < uniqueLaneIndexes.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, spawnerIndexes.Count);

                for (int n = 0; n < _enemiesWithLanes.Count; n++)
                {
                    if (oldLaneIndexes[n] == uniqueLaneIndexes[i])
                    {
                        _enemiesWithLanes[n] = new KeyValuePair<S_EnemyWithCount, int>(_enemiesWithLanes[n].Key, spawnerIndexes[randomIndex]);
                    }
                }

                spawnerIndexes.RemoveAt(randomIndex);
            }

            //registering cooldown values
            _enemyCooldownArr = CurrentSwarm.DefaultEnemyCooldowns;
            if (_enemyCooldownArr == null || _enemyCooldownArr.Count <= 0) _enemyCooldownArr = GLOBAL.FailsafeEnemyCooldowns;
            _enemyCooldownArrCount = _enemyCooldownArr.Count;
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
            prefab = Instantiate(prefab, _spawners[laneInt].Position, prefab.transform.rotation);
            ActiveEnemies.Add(prefab);

            currentWave[selectedIndex] = new KeyValuePair<S_EnemyWithCount, int>(new S_EnemyWithCount(selectedEnemy.Enemy, selectedEnemy.Count - 1), laneInt);
            if (currentWave[selectedIndex].Key.Count <= 0) currentWave.RemoveAt(selectedIndex);
        }

        void OnWaveEnded(object sender, string senderID)
        {
            if (WaveIsActive == false) return;

            if (senderID == "listEmpty" && _WaitUntillEnemiesAreDead == false) return;

            WaveIsActive = false;

            if (_changedSwarm != null)
            {
                ChangeCurrentSwarm();
                return;
            }

            if (_currentWaveIndex + 1 >= _waveCount && _RepeatLastWave == false)
            {
                AllWavesEnded();
                return;
            }

            int cooldown = _currentWaveCooldown;
            if (_currentWaveIndex + 1 < _waveCount) _currentWaveIndex++;

            StartCoroutine(nameof(RunWaveCooldown), cooldown);
        }

        void SetIndicatorValues(bool? toNull = false)
        {
            if(toNull == null || toNull.Value)
            {
                foreach (var item in _spawners) item.SetEnemyIndicators(null);

                return;
            }

            //First is lane second is enemies in it
            List<List<EnemyData>> enemies = new List<List<EnemyData>>();

            for (int i = 0; i < _spawners.Count; i++)
            {
                enemies.Add(null);
            }

            foreach (var item in _enemiesWithLanes)
            {
                int laneInt = item.Value;
                if (laneInt >= enemies.Count) continue;

                EnemyData data = item.Key.Enemy;

                if(enemies[laneInt] == null)
                {
                    enemies[laneInt] = new List<EnemyData>();
                }

                if(enemies[laneInt].Contains(data) == false)
                {
                    enemies[laneInt].Add(data);
                }
            }

            for (int i = 0; i < _spawners.Count; i++)
            {
                _spawners[i].SetEnemyIndicators(enemies[i]?.ToArray());
            }
        }

        float _currentCooldown = -1;
        IEnumerator RunWaveCooldown(int cooldown)
        {
            SetWaveUp();
            SetIndicatorValues();
            _TDPlayerController.EvaluateGameplayMode(false);

            _currentCooldown = cooldown;

            while (true)
            {
                if (_currentCooldown <= 0) break;

                string timerStr = $"Untill Next Wave : {Mathf.CeilToInt(_currentCooldown)}";
                _TimeTM.text = _isPaused ? "PAUSED" : timerStr;

                yield return new WaitForFixedUpdate();
                _currentCooldown -= Time.fixedDeltaTime * _cooldownSpeedMultiplier;
            }
            _currentCooldown = -1;
            _TimeTM.text = "";

            SetIndicatorValues(null);
            StartWave();
        }
        public void SkipWaveCooldown() => _currentCooldown = -1;

        void AllWavesEnded()
        {
            _TimeTM.text = "All Waves are finished";
        }

        void ChangeCurrentSwarm()
        {
            CurrentSwarm = _changedSwarm;
            _changedSwarm = null;
            _currentWaveIndex = 0;

            RunWaveCooldown(_currentWaveCooldown);
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
