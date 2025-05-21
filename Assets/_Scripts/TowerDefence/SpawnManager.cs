using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Overworld;
using SaveSystem;
using System.Linq;

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

        const string SAVE_ID = "spawn-manager-current-wave-index";

        public static bool WaveIsActive { get; private set; } = false;

        static List<GameObject> ActiveEnemies = new();

        public WaveValueInfo? PreviousWaveValueInfo { get; private set; } = null;
        public WaveValueInfo? CurrentWaveValueInfo { get; private set; } = null;

        [SerializeField] List<Spawner> _spawners = new();
        [SerializeField] GameObject _SpawnerPrefab = null;
        [SerializeField] GameObject _PrevWaveButton;
        [SerializeField] BaseManager _BaseMngr;
        [SerializeField] TDPlayerController _TDPlayerController;
        [SerializeField] ResourceData _RPData;
        [SerializeField] EnemyHealthbarManager _EnemyHBManager;
        [SerializeField] CoreManager _CoreManager;

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
        SaveManager _SaveManager
        {
            get
            {
                if (AUTO_saveManager == null)
                    AUTO_saveManager = SaveManager.Instance;

                return AUTO_saveManager;
            }
        }
        SaveManager AUTO_saveManager = null;

        bool _isSaved;//prevents race conditions

        void Start()
        {
            //Locking waves assigns values to the wildcards in it.
            var datalist = GLOBAL.GetWaveDatabase()?.DataList;
            if (datalist != null) _waves = LockWaves(datalist);

            LoadWaveIndex();

            //Setting wave populates info and add enemies to _currentEnemies
            SetWaveUp();

            _BaseMngr.e_BaseHasDied += _BaseMngr_e_BaseHasDied;

            _PrevWaveButton.SetActive(_currentWaveIndex > 0);

            //Use this method when you are ready. Usually hooked up to a button.
            //StartWave();
        }

        void OnApplicationQuit()
        {
            SaveWaveIndex();
            _isSaved = true;
        }
        void OnDestroy()
        {
            if (_isSaved == false) SaveWaveIndex();
        }
        void SaveWaveIndex()
        {
            _SaveManager?.AddOrReplaceSavedInteger(SAVE_ID, _currentWaveIndex);
        }

        public void LoadWaveIndex()
        {
            var temp = _SaveManager?.CurrentSave?.SavedIntegers?.Find(x => x.ID == SAVE_ID)?.Value ?? 0;
            _currentWaveIndex = Mathf.Min(Mathf.Max(_waves.Count - 1, 0), temp);
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

        /// <summary>
        /// Adds waves to current waves at current index
        /// </summary>
        /// <param name="addition">Waves to add</param>
        /// <param name="waveDelay">Delay of the waves. 0 means waves will play as next waves</param>
        public void AddWaves(List<TD_Wave> addition, int waveDelay = 0)
        {
            //Wave shouldn't be active to begin with
            if (WaveIsActive) return;

            int addIndex = _currentWaveIndex + Mathf.Max(waveDelay, 0);
            addIndex = Mathf.Min(addIndex, _waves.Count);

            _waves.InsertRange(addIndex, LockWaves(addition));
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
                yield return SpawnNextEnemy(ewl);

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

        List<TD_WaveValue> LockWaves(List<TD_Wave> allWaves)
        {
            var waves = new List<TD_WaveValue>();
            for (int i = 0; i < allWaves.Count; i++)
            {
                var d = allWaves[i].AsValue();
                for (int n = 0; n < d.Enemies.Count; n++)
                {
                    d.Enemies[n].LockEnemy();
                }
                waves.Add(d);
            }
            return waves;
        }
        void SetWaveValues()
        {
            var currentWave = _CurrentWave.Value;
            _currentEnemies = currentWave.Enemies;

            if (_PreviousWave.HasValue)
                PreviousWaveValueInfo = new(_PreviousWave.Value);

            CurrentWaveValueInfo = new(currentWave);
        }

        IEnumerator SpawnNextEnemy(TD_EnemyWithCooldown enemy)
        {
            var data = enemy.Enemy.Enemy;
            int laneMax = _spawners.Count;
            int i = 0;

            for (int n = 0; n < 3; n++)
            {
                i = UnityEngine.Random.Range(0, laneMax);
                if (i != _lastLaneIndex) break;
            }
            _lastLaneIndex = i;

            GameObject prefab = data.EnemyPrefab;
            int swarmCount = data.SwarmCount;

            for (int n = 0; n < swarmCount; n++)
            {
                var spawnedGO = Instantiate(prefab, _spawners[i].Position, prefab.transform.rotation);
                ActiveEnemies.Add(spawnedGO);
                _EnemyHBManager.SpawnBar(spawnedGO, data.Difficulty);

                if (n < swarmCount - 1)
                    yield return new WaitForSeconds
                        (data.SwarmCooldownRange.GetRandomInclusive());
            }

            yield return null;
        }

        void OnWaveEnded()
        {
            if (WaveIsActive == false) return;
            WaveIsActive = false;

            GiveRewards();
            //_CoreManager?.ResetCorePowers();

            if (_currentWaveIndex < _waves.Count - 1) _currentWaveIndex++;

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
            _CoreManager?.ResetCorePowers();
            //SetIndicatorValues();
            _TDPlayerController.EvaluateGameplayMode(WaveIsActive);
        }
        void GiveRewards()
        {
            //Gather wave rewards and add them to the base
            var rewards = _CurrentWave.Value.Rewards;
            foreach (var reward in rewards)
            {
                BaseResourceController.Instance.AddResource(reward.Resource, reward.Count);
            }

            //Send data to unlock manager (gameplay manager)
            _GameplayManager.UnlockDatas(_CurrentWave.Value.Unlocks);

            //Wipe crack save so they can reroll
            _SaveManager.DeleteCrackIndexes();
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
            public readonly List<ResourceWithCount> Rewards;
            public readonly List<string> UnlockIDs;

            public WaveValueInfo(TD_WaveValue wave)
            {
                var rpGain = GLOBAL.EnemyResearchPointGain;
                var dic_difficulty = GLOBAL.EnemyDifficultyMultipliers;
                List<EnemyType> types = new();
                float totalDifficulty = 0;
                float currentEnemyCooldown = -1;

                foreach (var e in wave.Enemies)
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

                    currentEnemyCooldown = cd;
                }

                DifficultyMultiplier = totalDifficulty / wave.Enemies.Count;
                DifficultyMultiplier = GLOBAL.DecimalSimplifier(DifficultyMultiplier, 3);
                EnemyTypes = types;
                Rewards = wave.Rewards;
                UnlockIDs = wave.Unlocks;
            }
        }
    }
}
