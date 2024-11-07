using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Overworld;

public class BaseResourceController : MonoBehaviour
{
    public static BaseResourceController Instance { get; private set; } = null;

    [SerializeField] TextMeshProUGUI _resourceText;

    Dictionary<ResourceData, int> _resourceDictionary = new Dictionary<ResourceData, int>();
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

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);

        GLOBAL.GetResourceDatabase().DataList.ForEach(x => _resourceDictionary.TryAdd(x, 0));
        RefreshText();
    }
    int debug1 = 0;
    private void Update()
    {

        //DEBUG

        if (Input.GetKeyDown(KeyCode.Keypad1)) debug1 = 1;
        if (Input.GetKeyDown(KeyCode.Keypad0)) debug1 = 10;
        if(debug1 != 0)
        {
            List<ResourceData> temp = _resourceDictionary.Keys.ToList();
            foreach (var data in temp)
            {
                AddResource(data, debug1);
            }

            debug1 = 0;
        }
        //DEBUG
    }

    public void TransferInventory()
    {
        List<ResourceItem> rItems = new List<ResourceItem>();
        List<InventoryItem> items = playerInstance.Inventory_Ref.Items;

        foreach (var item in items)
        {
            if(item is ResourceItem)
            {
                rItems.Add(item as ResourceItem);
            }
        }

        playerInstance.Inventory_Ref.Clean();
        AddResource(rItems.ToArray());
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

    void RefreshText()
    {
        _resourceText.text = "";
        foreach (KeyValuePair<ResourceData, int> pair in _resourceDictionary)
        {
            _resourceText.text += $"<color=red>{pair.Value}<color=white>    {pair.Key.ID}\n";
        }
    }
}
