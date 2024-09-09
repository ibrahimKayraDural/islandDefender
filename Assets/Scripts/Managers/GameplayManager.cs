using Overworld;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; } = null;

    public List<ToolData> UnlockedTools => _unlockedTools;

    [SerializeField] List<ToolData> _unlockedTools;

    ToolDatabase _toolDatabase;

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
    }

    public void UnlockTool(string nameOrID)
    {
        Tool tool = _toolDatabase.GetToolByNameOrID(nameOrID);
        if (tool == null) return;

        if (_unlockedTools.Contains(tool.Data) == false)
        {
            _unlockedTools.Add(tool.Data);
        }
    }
    public void LockTool(string nameOrID)
    {
        Tool tool = _toolDatabase.GetToolByNameOrID(nameOrID);
        if (tool == null) return;

        _unlockedTools.Remove(tool.Data);
    }
}
