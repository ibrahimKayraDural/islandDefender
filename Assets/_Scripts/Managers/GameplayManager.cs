using Overworld;
using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefence;
using Unity.VisualScripting;
using UnityEngine;

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
    }

    void Start()
    {
        LoadAllData();
    }
    void OnApplicationQuit()
    {
        SaveAllData();
        _isSaved = true;
    }
    void OnDestroy()
    {
        if (_isSaved == false) SaveAllData();
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
        TurretData data = _turretDatabase.GetDataByDisplayName(nameOrID);
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
        TurretData data = _turretDatabase.GetDataByDisplayName(nameOrID);
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
        var ids = _SaveManager.CurrentSave.UnlockedToolIDs;
        _unlockedTools = ids.Select(x => _toolDatabase?.GetToolByID(x)?.Data)?.Where(y => y != null).ToList();
    }
    void SaveUnlockedTurrets()
    {
        _SaveManager.ReplaceUnlockedTurrets(UnlockedTurrets);
    }
    void LoadUnlockedTurrets()
    {
        var ids = _SaveManager.CurrentSave.UnlockedTurretIDs;
        _unlockedTurrets = ids.Select(x => _turretDatabase?.GetDataByDisplayNameOrID(x))?.Where(y => y != null).ToList();
    }
    #endregion
}
