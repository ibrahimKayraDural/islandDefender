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
        public static bool WaveIsActive { get; private set; } = false;

        static List<GameObject> ActiveEnemies = new();

        public WaveValueInfo? PreviousWaveValueInfo { get; private set; } = null;
        public WaveValueInfo? CurrentWaveValueInfo { get; private set; } = null;

        [SerializeField] List<Spawner> _spawners = new();
        [SerializeField] GameObject _SpawnerPrefab = null;
        [SerializeField] GameObject _PrevWaveButton;
        [SerializeField] BaseManager _BaseMngr;
        [SerializeField] TDPlayerController _TDPlayerController;

        TD_WaveValue? _CurrentWave
        {
            get
            {
                if (_waves == null || _waves.Count <= 0) return null;
                if (_currentWaveIndex >= _waves.Count) return _waves[_waves.Count - 1];

                return _waves[_currentWaveIndex];
            }
        }
        TD_WaveValue? _PreviousWave
        {
            get
            {
                int newIndex = _currentWaveIndex - 1;

                if (_waves == null || _waves.Count <= 0) return null;
                if (newIndex < 0) return null;
                if (newIndex >= _waves.Count) newIndex = _waves.Count - 1;

                return _waves[newIndex];
            }
        }

        List<TD_WaveValue> _waves = null;
        int _currentWaveIndex = 0;
        int _lastLaneIndex = -1;

        List<TD_EnemyWithCooldown> _currentEnemies = new();

        void Start()
        {
            //Locking waves assigns values to the wildcards in it.
            LockWaves();

            //Setting wave populates info and add enemies to _currentEnemies
            SetWaveUp();

            _BaseMngr.e_BaseHasDied += _BaseMngr_e_BaseHasDied;

            _PrevWaveButton.SetActive(false);

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

        public void StartPreviousWave()
        {
            _currentWaveIndex = Mathf.Max(_currentWaveIndex - 1, 0);
            StartNextWave();
        }
        public void StartNextWave()
        {
            //SetIndicatorValues(null);

            if (WaveIsActive) return;
            if (_CurrentWave == null)
            {
                Debug.LogError("_currentWave is null");
                return;
            }

            WaveIsActive = true;
            SetWaveUp();
            StartCoroutine(nameof(WaveCoroutine));
        }
        public void StopWave()
        {
            if (WaveIsActive == false) return;

            StopCoroutine(nameof(WaveCoroutine));
            StopCoroutine(nameof(EnemyCooldownCounter));
            StopCoroutine(nameof(CheckNextEnemySpawn));

            WaveIsActive = false;
            _spawnNextEnemy = false;
        }

        IEnumerator WaveCoroutine()
        {
            for (int i = 0; i < _currentEnemies.Count; i++)
            {
                var ewl = _currentEnemies[i];
                SpawnNextEnemy(ewl);

                StartCoroutine(nameof(EnemyCooldownCounter), ewl.Cooldown);
                StartCoroutine(nameof(CheckNextEnemySpawn));

                yield return new WaitUntil(() => _spawnNextEnemy);

                StopCoroutine(nameof(EnemyCooldownCounter));
                StopCoroutine(nameof(CheckNextEnemySpawn));
                _spawnNextEnemy = false;
            }

            yield return new WaitUntil(() => ActiveEnemies.Count <= 0);

            OnWaveEnded();
        }

        bool _spawnNextEnemy = false;
        IEnumerator EnemyCooldownCounter(float cooldown)
        {
            yield return new WaitForSeconds(cooldown);
            _spawnNextEnemy = true;
        }
        IEnumerator CheckNextEnemySpawn()
        {
            yield return new WaitUntil(() => ActiveEnemies.Count <= 0);
            yield return new WaitForSeconds(1);
            _spawnNextEnemy = true;
        }

        void LockWaves()
        {
            var datalist = GLOBAL.GetWaveDatabase()?.DataList;
            if (datalist == null) return;

            _waves = new();
            for (int i = 0; i < datalist.Count; i++)
            {
                var d = datalist[i].AsValue();
                for (int n = 0; n < d.Enemies.Count; n++)
                {
                    d.Enemies[n].LockEnemy();
                }
                _waves.Add(d);
            }
        }
        void SetWaveValues()
        {
            _currentEnemies = _CurrentWave.Value.Enemies;

            PreviousWaveValueInfo = CurrentWaveValueInfo;
            CurrentWaveValueInfo = new WaveValueInfo(_currentEnemies);
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

            _currentWaveIndex++;
            SetWaveUp();

            _PrevWaveButton.SetActive(true);
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
            _TDPlayerController.EvaluateGameplayMode(WaveIsActive);
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

        public struct WaveValueInfo
        {
            public readonly float DifficultyMultiplier;
            public readonly List<EnemyType> EnemyTypes;
            public readonly float RPReward;

            public WaveValueInfo(List<TD_EnemyWithCooldown> enemies)
            {
                var rpGain = GLOBAL.EnemyResearchPointGain;
                var dic_difficulty = GLOBAL.EnemyDifficultyMultipliers;
                List<EnemyType> types = new();
                float totalRP = 0;
                float totalDifficulty = 0;
                float currentEnemyCooldown = -1;

                foreach (var e in enemies)
                {
                    var eData = e.Enemy.Enemy;
                    var cd = e.Cooldown;
                    var difficulty = eData.Difficulty;
                    var difficultyThisCycle = 0f;

                    foreach (var t in eData.EnemyTypes)
                    {
                        if (t != EnemyType.None && types.Contains(t) == false) types.Add(t);
                    }

                    difficultyThisCycle = dic_difficulty[difficulty];
                    difficultyThisCycle *= GLOBAL.EnemyCooldownDifficultyCalculator(cd);
                    totalDifficulty += difficultyThisCycle;

                    totalRP += rpGain[difficulty];
                    currentEnemyCooldown = cd;
                }

                DifficultyMultiplier = totalDifficulty / enemies.Count;
                DifficultyMultiplier = GLOBAL.DecimalSimplifier(DifficultyMultiplier, 3);
                EnemyTypes = types;
                RPReward = totalRP;
            }
        }
    }
}
