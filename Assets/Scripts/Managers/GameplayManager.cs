using Overworld;
using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefence;
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
    }
    public void AddToEnemyPool(string enemyID) => AddToEnemyPool(GLOBAL.GetEnemyDatabase().GetDataByDisplayNameOrID(enemyID));
    public void AddToEnemyPool(EnemyData data)
    {
        if (data == null) return;
        if (_enemyPool.Find(x => x == data) != null) return;
        _enemyPool.Add(data);
    }
    public void RemoveFromEnemyPool(string enemyID) => RemoveFromEnemyPool(GLOBAL.GetEnemyDatabase().GetDataByDisplayNameOrID(enemyID));
    public void RemoveFromEnemyPool(EnemyData data)
    {
        if (data == null) return;
        int i = _enemyPool.FindIndex(x => x == data);
        if (i == -1) return;
        _enemyPool.RemoveAt(i);
    }
}
