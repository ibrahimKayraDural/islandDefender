using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefence;
using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;

public class ManualTurretUI : MonoBehaviour
{
    public int Index =>_index;

    [SerializeField] Image _Icon;
    [SerializeField] TextMeshProUGUI _NameText;
    [SerializeField] TextMeshProUGUI _DescriptionText;
    [SerializeField] TextMeshProUGUI _CostText;
    [SerializedDictionary("ID", "Menu Game Object"), SerializeField] SerializedDictionary<string,GameObject> _Menus;

    ManualTurretManager _owner;
    int _index = -1;
    bool _isInitialized;

    public void Initialize(TurretData data, int index, ManualTurretManager owner)//@change->manual data
    {
        if (_isInitialized) return;

        _Icon.sprite = data.UISprite;
        _NameText.text = data.DisplayName;
        _DescriptionText.text = data.Description;
        _index = index;
        _owner = owner;

        string cost = "";
        foreach (var c in data.Costs)
        {
            cost += c.Amount + "X " + c.Resource.DisplayName + "\n";
        }
        _CostText.text = cost;

        _isInitialized = true;
    }

    public void SetStatus(string setTo)
    {
        if (_Menus.ContainsKey(setTo) == false) return;

        foreach (var key in _Menus.Keys) _Menus[key].SetActive(key == setTo);
    }

    public void OnClickedBuy()
    {
        if (_owner.TryBuyTurret(_index)) SetStatus("unlocked");
    }
    public void OnClickedPlace()
    {
        _owner.PlaceTurret(_index);
        SetStatus("placed");
    }
    public void OnClickedRemove()
    {
        _owner.RemoveTurret();
        SetStatus("unlocked");
    }
}
