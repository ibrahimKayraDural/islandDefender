using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Overworld;
using SaveSystem;
using static SaveSystem.SaveManager;
using TowerDefence;

public class BaseResourceController : MonoBehaviour
{
    public static BaseResourceController Instance { get; private set; } = null;

    [SerializeField] GameObject _resourceUIRoot;
    [SerializeField] CoreManager _CoreManager;

    Dictionary<ResourceData, int> _resourceDictionary = new();

    PlayerInstance playerInstance
    {
        get
        {
            if (AUTO_playerInstance == null)
                AUTO_playerInstance = PlayerInstance.Instance;
            return AUTO_playerInstance;
        }
    }
    PlayerInstance AUTO_playerInstance = null;
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
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);

        GLOBAL.GetResourceDatabase().DataList.ForEach(x => _resourceDictionary.TryAdd(x, 0));
        RefreshText();
    }
    void Start()
    {
        LoadInventory();
    }
    void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            List<ResourceData> temp = _resourceDictionary.Keys.ToList();
            foreach (var data in temp)
            {
                AddResource(data, 100);
            }
        }
        //DEBUG
    }
    void OnApplicationQuit()
    {
        SaveInventory();
        _isSaved = true;
    }
    void OnDestroy()
    {
        if (_isSaved == false) SaveInventory();
    }

    [ContextMenu("Save Inventory")]
    void SaveInventory()
    {
        List<ItemWithCount> inv = new();
        var dic = _resourceDictionary.ToArray();

        for (int i = 0; i < dic.Length; i++)
        {
            var data = dic[i].Key;
            var count = dic[i].Value;

            if (data == null) continue;
            else
            {
                inv.Add(new ItemWithCount(data.ID, count));
            }
        }

        _SaveManager.ReplaceBaseInventory(inv);
    }

    [ContextMenu("Load Inventory")]
    public void LoadInventory()
    {
        if (_SaveManager == null) return;

        var save = _SaveManager.CurrentSave;
        if (save == null) return;

        var inv = save.BaseInventory;
        if (inv == null) return;
        if (inv.Count == 0) return;

        _resourceDictionary = new Dictionary<ResourceData, int>();
        GLOBAL.GetResourceDatabase().DataList.ForEach(x => _resourceDictionary.TryAdd(x, 0));
        var resourceDB = GLOBAL.GetResourceDatabase();

        for (int i = 0; i < inv.Count; i++)
        {
            var item = inv[i];

            if (item == null) continue;
            else
            {
                var data = resourceDB.GetDataByDisplayNameOrID(item.ID);
                if (data == null) continue;

                if (_resourceDictionary.TryAdd(data, item.Count) == false)
                    _resourceDictionary[data] = item.Count;
            }
        }

        RefreshText();
    }

    public void TransferInventory()
    {
        List<ResourceItem> rItems = new();
        List<CoreItem> cores = new();
        List<InventoryItem> items = playerInstance.Inventory_Ref.Items;

        foreach (var item in items)
        {
            if (item is ResourceItem)
            {
                rItems.Add(item as ResourceItem);
            }
            else if (item is CoreItem)
            {
                cores.Add(item as CoreItem);
            }
        }

        playerInstance.Inventory_Ref.Clean();
        AddResource(rItems.ToArray());
        _CoreManager.AddCores(cores);
    }
    public void AddResource(ResourceItem[] items)
    {
        foreach (var item in items) AddResource(item);
    }
    public void AddResource(ResourceData data, int amount)
    {
        amount = Mathf.Max(amount, 0);//clamp if negative

        InfoLogUI.Instance?.AddToLog($"{data.DisplayName} X {amount} has been transfered to the base.", Color.yellow);

        if (_resourceDictionary.TryAdd(data, amount)) return;//add it and quit if value is not present

        _resourceDictionary[data] += amount;//increase amount if value is present

        RefreshText();
    }
    public void AddResource(ResourceItem item) => AddResource(item.Data, item.Count);
    public bool TryBuyTurret(TowerDefence.TurretData data) => TrySpendResource(data.Costs);
    public bool TrySpendResource(ResourceData data, int amount)
    {
        if (_resourceDictionary.ContainsKey(data) == false) return false;
        if (_resourceDictionary[data] < amount) return false;

        _resourceDictionary[data] -= amount;
        RefreshText();
        return true;
    }
    public bool TrySpendResource(Cost cost)
    {
        if (_resourceDictionary.ContainsKey(cost.Resource) == false) return false;
        if (_resourceDictionary[cost.Resource] < cost.Amount) return false;

        _resourceDictionary[cost.Resource] -= cost.Amount;
        RefreshText();
        return true;
    }
    public bool TrySpendResource(Cost[] costArr)
    {
        foreach (var cost in costArr)
        {
            if (_resourceDictionary.ContainsKey(cost.Resource) == false) return false;
            if (_resourceDictionary[cost.Resource] < cost.Amount) return false;
        }

        foreach (var cost in costArr)
        {
            _resourceDictionary[cost.Resource] -= cost.Amount;
        }
        RefreshText();
        return true;
    }
    public void ResetAllResources()
    {
        List<ResourceData> temp = _resourceDictionary.Keys.ToList();
        foreach (var data in temp)
        {
            _resourceDictionary[data] = 0;
        }
        RefreshText();
    }
    public int CheckItemCount(ResourceItem itemToCheck)
    {
        if (itemToCheck == null) return 0;

        ResourceData data = itemToCheck.Data;
        if (data == null) return 0;
        if (_resourceDictionary.ContainsKey(data) == false) return 0;

        return _resourceDictionary[data];
    }

    void RefreshText()
    {
        if (_resourceUIRoot != null && _resourceUIRoot.activeSelf == false)
            _resourceUIRoot.SetActive(true);

        FindObjectOfType<UIResourceDisplay>()?.RefreshAll();
    }
}
