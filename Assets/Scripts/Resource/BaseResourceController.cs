using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class BaseResourceController : MonoBehaviour
{
    public static BaseResourceController Instance = null;

    [SerializeField] TextMeshProUGUI _resourceText;

    Dictionary<ResourceData, int> _resourceDictionary = new Dictionary<ResourceData, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);

        GLOBAL.GetResourceDatabase().Resources.ForEach(x => _resourceDictionary.TryAdd(x, 0));
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

    public void AddResource(ResourceData data, int amount)
    {
        amount = Mathf.Max(amount, 0);//clamp if negative

        if (_resourceDictionary.TryAdd(data, amount)) return;//add it and quit if value is not present

        _resourceDictionary[data] += amount;//increase amount if value is present

        RefreshText();
    }
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
