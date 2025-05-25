using Overworld;
using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefence;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrackInteractable : ProximityInteractable
{
    static List<string> AllGUIDs = new();
    public override string InteractDescription { get => "Search the crack"; set { } }
    public ResourceWithCount CurrentReward => _currentReward;

    [SerializeField] List<ResourceWithCount> _RewardList;
    [SerializeField] string _GUID;

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

    ResourceWithCount _currentReward = null;
    int _rewardIndex;
    bool _isSaved;//prevents race conditions

    [ContextMenu("Generate GUID")]
    void GenerateGUIDGetter() => _GUID = Guid.NewGuid().ToString();

    void Awake()
    {
        AllGUIDs = new();
    }

    void Start()
    {
        SceneManager.activeSceneChanged += CheckSave;

        if (AllGUIDs.Contains(_GUID))
            Debug.LogError("Can not have two objects with the same GUID. " +
                "Regenerate this GUID or this will cause weird errors!\nSincerely, " + gameObject.name);
        else
            AllGUIDs.Add(_GUID);

        if (_RewardList.Count == 0)
        {
            enabled = false;
            return;
        }

        LoadIndex();

        if (_rewardIndex >= 0) _currentReward = _RewardList[_rewardIndex];
    }

    void OnApplicationQuit()
    {
        SceneManager.activeSceneChanged -= CheckSave;
        SaveIndex();
        _isSaved = true;
    }
    void CheckSave(Scene oldScene, Scene newScene)
    {
        SceneManager.activeSceneChanged -= CheckSave;
        if (_isSaved == false) SaveIndex();
    }

    [ContextMenu("Save Index")]
    void SaveIndex()
    {
        _SaveManager?.AddOrReplaceCrackIndex(_GUID, _rewardIndex);
    }

    [ContextMenu("Load Index")]
    public void LoadIndex()
    {
        var temp = _SaveManager?.CurrentSave?.CrackIndexes?.Find(x => x.ID == _GUID)?.Value;

        if (temp.HasValue) _rewardIndex = temp.Value;
        else _rewardIndex = UnityEngine.Random.Range(0, _RewardList.Count);
    }

    public void SpendItem()
    {
        _currentReward = null;
        _rewardIndex = -1;
    }
}
