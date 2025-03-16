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
    struct UpgradeTree
    {
        public List<UpgradeUIPiece> Upgrades;
        public Slider Slider;
        public string ID;
    }
    [System.Serializable]
    struct UpgradeUIPiece
    {
        public Button @Button;
        public UpgradeData Upgrade;
        public List<Cost> BuyCost;
        public float SliderAfterValue;
        [HideInInspector] public UpgradeTree Owner;
    }

    [SerializeField] List<UpgradeTree> _Upgrades;
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

    void Awake()
    {
        foreach (var tree in _Upgrades)
        {
            for (int i = 0; i < tree.Upgrades.Count; i++)
            {
                var item = tree.Upgrades[i];
                var i2 = i;
                var tree2 = tree;
                item.Button.onClick.AddListener(delegate { OnUpgradeSelected(tree, i2); });
                item.Button.interactable = i == 0;
                item.Owner = tree;
            }
        }
    }

    public void BuyUpgrade()
    {
        if (_selectedUpgrade == null) return;
        var su = _selectedUpgrade.Value;
        if (_BaseResourceController.TrySpendResource(su.BuyCost.ToArray()) == false)
        {
            DeselectUpgrade();
            _Title.text = "<color=red>INSUFFICENT FUNDS<color=white>";
            return;
        }

        HandleUpgradeEffect(su);

        su.Button.interactable = false;
        su.Button.GetComponent<Image>().color = _BoughtUpgradeColor;

        su.Owner.Slider.value = su.SliderAfterValue;

        DeselectUpgrade();
    }

    void HandleUpgradeEffect(UpgradeUIPiece upgrade)
    {
        string id = upgrade.Owner.ID;

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
    void OnUpgradeSelected(UpgradeTree tree, int index)
    {
        UpgradeUIPiece uiPiece = tree.Upgrades[index];
        _selectedUpgrade = uiPiece;
        _Title.text = _selectedUpgrade.Value.Upgrade.DisplayName;
        _Description.text = _selectedUpgrade.Value.Upgrade.Description;
        _Cost.text = "";
        for (int i = 0; i < uiPiece.BuyCost.Count; i++)
        {
            Cost buyCost = uiPiece.BuyCost[i];
            _Cost.text += buyCost.Resource.DisplayName + " X " + buyCost.Amount;
            if (i < uiPiece.BuyCost.Count - 1) _Cost.text += "\n";
        }
        uiPiece.Button.Select();
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
