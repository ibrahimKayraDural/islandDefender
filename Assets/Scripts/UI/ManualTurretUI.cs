using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefence;
using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;

public class ManualTurretUI : MonoBehaviour
{
    [SerializeField] Image _Icon;
    [SerializeField] TextMeshProUGUI _NameText;
    [SerializeField] TextMeshProUGUI _DescriptionText;
    [SerializeField] TextMeshProUGUI _CostText;
    [SerializedDictionary("ID", "Menu Game Object"), SerializeField] SerializedDictionary<string,GameObject> _Menus;

    int _index = -1;
    bool _isInitialized;

    public void Initialize(TurretData data, int index)//@change->manual data
    {
        if (_isInitialized) return;

        _Icon.sprite = data.UISprite;
        _NameText.text = data.DisplayName;
        _DescriptionText.text = data.Description;
        _index = index;

        string cost = "";
        foreach (var c in data.Costs)
        {
            cost += c.Amount + "X " + c.Resource.DisplayName + "\n";
        }

        _isInitialized = true;
    }

    public void SetStatus(string setTo)
    {
        if (_Menus.ContainsKey(setTo) == false) return;

        foreach (var key in _Menus.Keys) _Menus[key].SetActive(key == setTo);
    }
}
