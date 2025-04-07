using Overworld;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UpgradeSystem;

public class PlayerUpgradeManager : MonoBehaviour
{
    [System.Serializable]
    class UpgradeTree
    {
        public string ID;
        public Button @Button;
        public TextMeshProUGUI CostTM;
        public List<UpgradePiece> Upgrades;
        public int CurrentIndex = 0;

        public UpgradePiece? CurrentUpgrade
        {
            get
            {
                if (CurrentIndex < 0 || IsFinished) return null;
                return Upgrades[CurrentIndex];
            }
        }
        public bool IsFinished => CurrentIndex >= Upgrades.Count;
        public void SetCostText()
        {
            CostTM.text = "";
            if (CurrentUpgrade != null)
            {
                var upgrade = CurrentUpgrade.Value;

                for (int i = 0; i < upgrade.BuyCost.Count; i++)
                {
                    Cost buyCost = upgrade.BuyCost[i];
                    CostTM.text += buyCost.Resource.DisplayName + " X " + buyCost.Amount;
                    if (i < upgrade.BuyCost.Count - 1) CostTM.text += "\n";
                }
            }
        }
    }
    [System.Serializable]
    struct UpgradePiece
    {
        public UpgradeData Data;
        public List<Cost> BuyCost;
    }
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

    [SerializeField] List<UpgradeTree> _Upgrades;
    [SerializeField] TextMeshProUGUI _Title;
    [SerializeField] TextMeshProUGUI _Description;
    [SerializeField] GameObject BuyButton;

    UpgradeTree _selectedTree = null;

    void Awake()
    {
        foreach (var tree in _Upgrades)
        {
            tree.Button.onClick.AddListener(delegate { OnUpgradeSelected(tree); });
            tree.SetCostText();
        }
    }

    void OnUpgradeSelected(UpgradeTree tree)
    {
        if (tree.CurrentUpgrade == null) return;
        var currentUpgrade = tree.CurrentUpgrade.Value;
        var upgradeData = currentUpgrade.Data;
        var upgradeCost = currentUpgrade.BuyCost;
        _selectedTree = tree;
        RefreshHelpBox(tree);
        BuyButton.SetActive(true);
        tree.Button.Select();
    }

    private void RefreshHelpBox(UpgradeTree tree)
    {
        if (tree == null || tree.CurrentUpgrade == null) return;
        var upgradeData = tree.CurrentUpgrade.Value.Data;
        _Title.text = upgradeData.DisplayName;
        _Description.text = upgradeData.Description;
    }

    public void BuyUpgrade()
    {
        if (_selectedTree == null) return;
        if (_selectedTree.CurrentUpgrade == null) return;
        if (_BaseResourceController.TrySpendResource(_selectedTree.CurrentUpgrade.Value.BuyCost.ToArray()) == false)
        {
            DeselectUpgrade();
            _Title.text = "<color=red>INSUFFICENT FUNDS<color=white>";
            return;
        }

        HandleUpgradeEffect(_selectedTree);

        _selectedTree.CurrentIndex++;
        _selectedTree.SetCostText();
        if (_selectedTree.IsFinished)
        {
            _selectedTree.Button.interactable = false;
            _selectedTree.CostTM.text = "MAX";
            DeselectUpgrade();
        }

        RefreshHelpBox(_selectedTree);
        //DeselectUpgrade();
    }
    void DeselectUpgrade()
    {
        _selectedTree = null;
        _Title.text = "";
        _Description.text = "";
        BuyButton.SetActive(false);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }
    void HandleUpgradeEffect(UpgradeTree tree)
    {
        string id = tree.ID;
        var upgrade = tree?.CurrentUpgrade?.Data;
        if (upgrade == null) return;

        IUpgradeable upgradeable = null;
        string upgradeID = GLOBAL.UnassignedString;
        switch (id)
        {
            case "drillSpeed":
                upgradeable = FindObjectOfType<Drill>(true);
                upgradeID = "speed";
                break;
            case "player":
                upgradeable = PlayerInstance.Instance.PlayerController_Ref;
                upgradeID = "speed";
                break;
        }

        upgradeable?.SetUpgradeData(upgradeID, upgrade);
    }
}
