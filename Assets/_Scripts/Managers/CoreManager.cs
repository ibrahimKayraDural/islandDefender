using Overworld;
using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefence;
using UnityEngine;

public class CoreManager : MonoBehaviour
{
    const string SAVEID = "core-manager-cores";

    public List<CoreItem> Cores => _cores;

    List<CoreItem> _cores = new();

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
        Load();
    }
    void OnApplicationQuit()
    {
        Save();
        _isSaved = true;
    }
    void OnDestroy()
    {
        if (_isSaved == false) Save();
    }

    [ContextMenu("Save")]
    void Save()
    {
        List<string> coreIDs = Cores.Where(x => x?.Data?.ID != null).Select(x => x.Data.ID).ToList();
        _SaveManager.AddOrReplaceSavedObject(SAVEID, coreIDs);
    }

    [ContextMenu("Load")]
    void Load()
    {
        if (_SaveManager == null) return;

        var save = _SaveManager.CurrentSave;
        if (save == null) return;

        var val = save?.SavedObjects?.Find(x => x.ID == SAVEID);
        if (val?.Value != null)
        {
            var ids = val.Value as List<string>;
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    var core = GLOBAL.GetCoreDatabase().GetDataByDisplayNameOrID(id);
                    if (core != null) AddCore(core.AsItem());
                }
            }
        }
    }

    public void AddCores(List<CoreItem> cores)
    {
        foreach (var c in cores) AddCore(c);
    }
    public void AddCore(CoreItem core)
    {
        //disallow the same cores being added
        foreach (var c in Cores)
            if (c.Compare(core)) return;

        _cores.Add(core);
    }
}
