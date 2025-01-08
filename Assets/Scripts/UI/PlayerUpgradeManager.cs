using AYellowpaper.SerializedCollections;
using Overworld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeManager : MonoBehaviour
{
    [System.Serializable]
    struct UpgradeUIPiece
    {
        public Button @Button;
        public UpgradeData Upgrade;
        public List<Cost> BuyCost;
        public float SliderAfterValue;
        public int Level;
        public string ID;
    }

    [SerializeField] List<UpgradeUIPiece> _Upgrades;
    [SerializeField] List<SerializedKeyValuePair<string, Slider>> _SlidersWithID;
    [SerializeField] TextMeshProUGUI _Title;
    [SerializeField] TextMeshProUGUI _Description;
    [SerializeField] TextMeshProUGUI _Cost;
    [SerializeField] GameObject BuyButton;
    [SerializeField] Color _BoughtUpgradeColor;

    BaseResourceController _BaseResourceController
    {
        get
        {
            if (AUTO_BaseResourceController == null)
                AUTO_BaseResourceController = BaseResourceController.Instance;
            return AUTO_BaseResourceController;
        }
    }
    BaseResourceController AUTO_BaseResourceController = null;

    UpgradeUIPiece? _selectedUpgrade;
    Dictionary<string, List<UpgradeUIPiece>> _dataDictionary;

    void Awake()
    {
        foreach (var item in _Upgrades)
        {
            item.Button.onClick.AddListener(delegate { OnUpgradeSelected(item); });
        }

        _dataDictionary = new Dictionary<string, List<UpgradeUIPiece>>();
        foreach (var item in _Upgrades)
        {
            if (_dataDictionary.ContainsKey(item.ID)) _dataDictionary[item.ID].Add(item);
            else _dataDictionary.Add(item.ID, new List<UpgradeUIPiece>() { item });
        }

        foreach (var dataList in _dataDictionary.Values)
        {
            dataList.ForEach(x => x.Button.interactable = false);
            dataList.OrderBy(x => x.Level).First().Button.interactable = true;
        }
    }

    public void BuyUpgrade()
    {
        if (_selectedUpgrade == null) return;
        if (_BaseResourceController.TrySpendResource(_selectedUpgrade.Value.BuyCost.ToArray()) == false)
        {
            DeselectUpgrade();
            _Title.text = "<color=red>INSUFFICENT FUNDS<color=white>";
            return;
        }

        HandleUpgradeEffect(_selectedUpgrade.Value);

        _selectedUpgrade.Value.Button.interactable = false;
        _selectedUpgrade.Value.Button.GetComponent<Image>().color = _BoughtUpgradeColor;
        var list = _dataDictionary[_selectedUpgrade.Value.ID];
        list.Remove(_selectedUpgrade.Value);

        if (list.Count <= 0) _dataDictionary.Remove(_selectedUpgrade.Value.ID);
        else list.OrderBy(x => x.Level).First().Button.interactable = true;

        var slider = _SlidersWithID.Find(x => x.Key == _selectedUpgrade.Value.ID).Value;
        slider.value = _selectedUpgrade.Value.SliderAfterValue;

        DeselectUpgrade();
    }

    void HandleUpgradeEffect(UpgradeUIPiece upgrade)
    {
        string id = upgrade.ID;

        if (id == "player" || id == "drillSpeed")
        {
            ISpeedUpgradable isu = null;

            switch (id)
            {
                case "drillSpeed": isu = FindObjectOfType<Drill>(true); break;
                case "player": isu = PlayerInstance.Instance.PlayerController_Ref; break;
            }
            isu?.SetSpeedUpgrade(upgrade.Upgrade);
        }
    }
    void OnUpgradeSelected(UpgradeUIPiece UIPiece)
    {
        _selectedUpgrade = UIPiece;
        _Title.text = _selectedUpgrade.Value.Upgrade.DisplayName;
        _Description.text = _selectedUpgrade.Value.Upgrade.Description;
        _Cost.text = "";
        for (int i = 0; i < UIPiece.BuyCost.Count; i++)
        {
            Cost buyCost = UIPiece.BuyCost[i];
            _Cost.text += buyCost.Resource.DisplayName + " X " + buyCost.Amount;
            if (i < UIPiece.BuyCost.Count - 1) _Cost.text += "\n";
        }
        UIPiece.Button.Select();
        BuyButton.SetActive(true);
    }
    void DeselectUpgrade()
    {
        _selectedUpgrade = null;
        _Title.text = "";
        _Description.text = "";
        _Cost.text = "";
        BuyButton.SetActive(false);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }
}
