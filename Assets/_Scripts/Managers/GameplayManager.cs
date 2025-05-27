using Overworld;
using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefence;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; } = null;
    public static event EventHandler<List<TurretData>> e_OnUnlockedTurretListChanged;
    public static event EventHandler<List<ToolData>> e_OnUnlockedToolListChanged;

    public List<ToolData> UnlockedTools => _unlockedTools;
    public List<TurretData> UnlockedTurrets => _unlockedTurrets;
    public List<EnemyData> EnemyPool => _enemyPool;

    [SerializeField] List<ToolData> _unlockedTools;
    [SerializeField] List<TurretData> _unlockedTurrets;
    [SerializeField] List<EnemyData> _enemyPool;

    ToolDatabase _toolDatabase;
    TurretDatabase _turretDatabase;
    EnemyDatabase _enemyDatabase;

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
    PlayerUpgradeManager _PlayerUpgradeManager
    {
        get
        {
            if (AUTO_playerUpgradeManager == null)
                AUTO_playerUpgradeManager = FindObjectOfType<PlayerUpgradeManager>(true);

            return AUTO_playerUpgradeManager;
        }
    }
    PlayerUpgradeManager AUTO_playerUpgradeManager = null;
    ScannerManager _ScannerManager
    {
        get
        {
            if (AUTO_scannerManager == null)
                AUTO_scannerManager = FindObjectOfType<ScannerManager>(true);

            return AUTO_scannerManager;
        }
    }
    ScannerManager AUTO_scannerManager = null;

    bool _isSaved;//prevents race conditions

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _toolDatabase = GLOBAL.GetToolDatabase();
        _turretDatabase = GLOBAL.GetTurretDatabase();
        _enemyDatabase = GLOBAL.GetEnemyDatabase();

        LoadAllData();
    }
    void Start()
    {
        SceneManager.activeSceneChanged += CheckSave;
    }
    void OnApplicationQuit()
    {
        SceneManager.activeSceneChanged -= CheckSave;
        SaveAllData();
        _isSaved = true;
    }
    void CheckSave(Scene oldScene, Scene newScene)
    {
        SceneManager.activeSceneChanged -= CheckSave;
        if (_isSaved == false) SaveAllData();
    }

    public void UnlockDatas(List<string> ids)
    {
        foreach (var fullID in ids)
        {
            var sep = fullID.Split('=');
            if (sep.Length != 2) continue;

            var type = sep[0];
            var val = sep[1];

            if (type == GLOBAL.TurretUnlockID) UnlockTurret(val);
            else if (type == GLOBAL.ToolUnlockID) UnlockTool(val);
            else if (type == GLOBAL.EnemyUnlockID) AddToEnemyPool(val);
            else if (type == GLOBAL.UpgradeUnlockID)
            {
                if (_PlayerUpgradeManager != null)
                    _PlayerUpgradeManager.UnlockUpgradeTree(val);
            }
            else if (type == GLOBAL.ScannerUnlockID)
            {
                if (_ScannerManager != null && int.TryParse(val, out int lvl))
                    _ScannerManager.IncreasinglySetAllowedLevel(lvl);
            }
            else if (type == GLOBAL.SpecialUnlockID)
            {

            }
        }
    }
    public void UnlockTool(string nameOrID)
    {
        Tool tool = _toolDatabase.GetToolByNameOrID(nameOrID);
        if (tool == null) return;

        if (_unlockedTools.Contains(tool.Data) == false)
        {
            _unlockedTools.Add(tool.Data);
            e_OnUnlockedToolListChanged?.Invoke(this, UnlockedTools);
        }

        SaveUnlockedTools();
    }
    public void LockTool(string nameOrID)
    {
        Tool tool = _toolDatabase.GetToolByNameOrID(nameOrID);
        if (tool == null) return;

        if (_unlockedTools.Contains(tool.Data))
        {
            _unlockedTools.Remove(tool.Data);
            e_OnUnlockedToolListChanged?.Invoke(this, UnlockedTools);
        }

        SaveUnlockedTools();
    }
    public void UnlockTurret(string nameOrID)
    {
        TurretData data = _turretDatabase.GetDataByDisplayNameOrID(nameOrID);
        if (data == null) return;

        if (_unlockedTurrets.Contains(data) == false)
        {
            _unlockedTurrets.Add(data);
            e_OnUnlockedTurretListChanged?.Invoke(this, UnlockedTurrets);
        }

        SaveUnlockedTurrets();
    }
    public void LockTurret(string nameOrID)
    {
        TurretData data = _turretDatabase.GetDataByDisplayNameOrID(nameOrID);
        if (data == null) return;

        if (_unlockedTurrets.Contains(data))
        {
            _unlockedTurrets.Remove(data);
            e_OnUnlockedToolListChanged?.Invoke(this, UnlockedTools);
        }

        SaveUnlockedTurrets();
    }
    public void AddToEnemyPool(string enemyID) => AddToEnemyPool(_enemyDatabase.GetDataByDisplayNameOrID(enemyID));
    public void AddToEnemyPool(EnemyData data)
    {
        if (data == null) return;
        if (_enemyPool.Find(x => x == data) != null) return;
        _enemyPool.Add(data);

        SaveUnlockedEnemies();
    }
    public void RemoveFromEnemyPool(string enemyID) => RemoveFromEnemyPool(_enemyDatabase.GetDataByDisplayNameOrID(enemyID));
    public void RemoveFromEnemyPool(EnemyData data)
    {
        if (data == null) return;
        int i = _enemyPool.FindIndex(x => x == data);
        if (i == -1) return;
        _enemyPool.RemoveAt(i);

        SaveUnlockedEnemies();
    }

    #region Save
    void SaveAllData()
    {
        SaveUnlockedEnemies();
        SaveUnlockedTools();
        SaveUnlockedTurrets();
    }
    void LoadAllData()
    {
        LoadUnlockedEnemies();
        LoadUnlockedTools();
        LoadUnlockedTurrets();
    }
    void SaveUnlockedEnemies()
    {
        _SaveManager.ReplaceUnlockedEnemies(EnemyPool);
    }
    void LoadUnlockedEnemies()
    {
        var ids = _SaveManager?.CurrentSave?.UnlockedEnemyIDs;
        var temp = ids?.Select(x => _enemyDatabase?.GetDataByID(x))?.Where(y => y != null)?.ToList();
        if (temp == null || temp.Count <= 0) return;

        _enemyPool.AddRange(temp);
    }
    void SaveUnlockedTools()
    {
        _SaveManager.ReplaceUnlockedTools(UnlockedTools);
    }
    void LoadUnlockedTools()
    {
        var ids = _SaveManager?.CurrentSave?.UnlockedToolIDs;
        if (ids == null) return;

        _unlockedTools = ids.Select(x => _toolDatabase?.GetToolByID(x)?.Data)?.Where(y => y != null).ToList();
    }
    void SaveUnlockedTurrets()
    {
        _SaveManager.ReplaceUnlockedTurrets(UnlockedTurrets);
    }
    void LoadUnlockedTurrets()
    {
        var ids = _SaveManager?.CurrentSave?.UnlockedTurretIDs;
        if (ids == null) return;

        _unlockedTurrets = ids.Select(x => _turretDatabase?.GetDataByDisplayNameOrID(x))?.Where(y => y != null).ToList();
    }
    #endregion
}
