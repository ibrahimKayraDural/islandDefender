using CorePowers;
using Overworld;
using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefence;
using UnityEngine;

public class CoreManager : MonoBehaviour
{
    public List<CoreItem> Cores => _cores;
    public CorePower_Base SelectedCorePower => _selectedCore == null ? null : dic_corePowerControllers[_selectedCore];

    [SerializeField] GenericCell _CellPrefab;
    [SerializeField] CorePowerActivator _CorePowerActivator;
    [SerializeField] Transform _CellParent;
    [SerializeField] Transform _CorePowerControllerParent;

    Dictionary<CoreData, CorePower_Base> dic_corePowerControllers = new();
    List<CoreItem> _cores = new();
    CoreData _selectedCore = null;

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
        GatherCorePowerControllers();
        ResetCorePowers();

        Load();
        RefreshCells();
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

    public void AddCores(List<CoreItem> cores)
    {
        foreach (var c in cores) AddCore(c, false);

        RefreshCells();
    }
    public void AddCore(CoreItem core, bool refresh = true)
    {
        bool isFirst = Cores.Count <= 0;

        //disallow the same cores being added
        foreach (var c in Cores)
            if (c.Compare(core)) return;

        _cores.Add(core);

        if (isFirst) SelectCore(Cores[0].Data);
        if (refresh) RefreshCells();
    }
    public void ResetCorePowers()
    {
        var values = dic_corePowerControllers.Values.ToArray();
        for (int i = 0; i < values.Length; i++)
        {
            values[i].ResetUsages();
        }

        _CorePowerActivator.Refresh(SelectedCorePower);
    }


    [ContextMenu("Save")]
    void Save()
    {
        List<string> coreIDs = Cores?.Where(x => x?.Data?.ID != null)?.Select(x => x.Data.ID)?.ToList();
        if (coreIDs != null)
            _SaveManager.SetCoreState(coreIDs, _selectedCore.ID);
    }

    [ContextMenu("Load")]
    void Load()
    {
        if (_SaveManager == null) return;

        var save = _SaveManager.CurrentSave;
        if (save == null) return;

        var state = save?.CoreState;
        if (state != null)
        {
            var ids = state.CoreIDs;
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    var core = GLOBAL.GetCoreDatabase().GetDataByDisplayNameOrID(id);
                    if (core != null) AddCore(core.AsItem());
                }
            }

            var selectedID = state.SelectedCore;
            if (selectedID != null)
            {
                _selectedCore = Cores.Find(x => x.Data.ID == selectedID)?.Data;
            }
        }
    }

    void RefreshCells()
    {
        foreach (var c in _CellParent.Cast<Transform>())
        {
            Destroy(c.gameObject);
        }

        foreach (var c in Cores)
        {
            var cell = Instantiate(_CellPrefab.gameObject, _CellParent).GetComponent<GenericCell>();
            var data = c.Data;
            cell.Initialize(() => SelectCore(data), data.UISprite);
            cell.SetInteractablity(data != _selectedCore);
        }
    }
    void SelectCore(CoreData core)
    {
        _selectedCore = core;
        RefreshCells();
    }
    void GatherCorePowerControllers()
    {
        var cpcs = _CorePowerControllerParent.GetComponentsInChildren<CorePower_Base>();
        for (int i = 0; i < cpcs.Length; i++)
        {
            var cpc = cpcs[i];
            dic_corePowerControllers.TryAdd(cpc.Data, cpc);
        }
    }
}
