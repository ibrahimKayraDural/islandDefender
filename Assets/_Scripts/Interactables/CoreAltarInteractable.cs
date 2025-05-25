using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreAltarInteractable : ProximityInteractable
{
    const string GUIDHEADER = "core-altar-";
    static List<string> AllGUIDs = new();

    public override string InteractDescription { get => "Open Altar"; set { } }

    public CoreData Core => _Core;
    public bool IsTaken => _isTaken;

    [SerializeField] CoreData _Core;
    [SerializeField] GameObject _CoreVisual;
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

    bool _isTaken = false;

    bool _isSaved;//prevents race conditions

    [ContextMenu("Generate GUID")]
    void GenerateGUIDGetter() => _GUID = GUIDHEADER + Guid.NewGuid();

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

        Load();
    }

    void OnApplicationQuit()
    {
        SceneManager.activeSceneChanged -= CheckSave;
        Save();
        _isSaved = true;
    }
    void CheckSave(Scene oldScene, Scene newScene)
    {
        SceneManager.activeSceneChanged -= CheckSave;
        if (_isSaved == false) Save();
    }

    [ContextMenu("Save")]
    void Save()
    {
        if (IsTaken) _SaveManager?.AddOrReplaceSavedInteger(_GUID, 1);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        var i = _SaveManager?.CurrentSave?.SavedIntegers?.Find(x => x.ID == _GUID)?.Value;
        if (i.HasValue) TryTakeCore(true);
    }

    public bool TryTakeCore(bool runSilent = false)
    {
        if (IsTaken) return false;

        _isTaken = true;
        _CoreVisual?.SetActive(false);
        return true;
    }
}
