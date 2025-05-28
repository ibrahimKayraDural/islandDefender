using Overworld;
using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using TowerDefence;
using UnityEngine;
using UnityEngine.UI;
using UpgradeSystem;

public class PlayerUpgradeManager : MonoBehaviour
{
    const string UNLOCKEDTREESAVEID = "player-upgrade-manager-unlocks";
    const string TREEINDEXSAVEHEADER = "player-upgrade-manager-tree-index-";

    [System.Serializable]
    class UpgradeTree
    {
        public string ID;
        public Button @Button;
        public TextMeshProUGUI CostTM;
        public Image RPImage;
        public GameObject VisualParent;
        public List<UpgradePiece> Upgrades;
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                _currentIndex = value;
                Refresh();
            }
        }

        [SerializeField] int _currentIndex = 0;

        public UpgradePiece? CurrentUpgrade
        {
            get
            {
                if (CurrentIndex < 0 || IsFinished) return null;
                return Upgrades[CurrentIndex];
            }
        }
        public bool IsFinished => CurrentIndex >= Upgrades.Count;


        public void Refresh()
        {
            Button.interactable = !IsFinished;
            SetCostText();
        }
        public void SetCostText()
        {
            CostTM.text = "";
            if (CurrentUpgrade != null)
            {
                var upgrade = CurrentUpgrade.Value;

                for (int i = 0; i < upgrade.BuyCost.Count; i++)
                {
                    Cost buyCost = upgrade.BuyCost[i];
                    CostTM.text += /*buyCost.Resource.DisplayName + */" X " + buyCost.Amount;
                    if (i < upgrade.BuyCost.Count - 1) CostTM.text += "\n";
                }
            }
            else if (IsFinished)
            {
                CostTM.text = "MAXED OUT";
                RPImage.gameObject.SetActive(false);

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
    [SerializeField] List<string> _UnlockedTrees;

    UpgradeTree _selectedTree = null;
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

    void Start()
    {
        LoadDatas();

        foreach (var tree in _Upgrades)
        {
            tree.Button.onClick.AddListener(delegate { OnUpgradeSelected(tree); });
            tree.SetCostText();

            tree.VisualParent.SetActive(_UnlockedTrees.Contains(tree.ID));
        }
    }

    [ContextMenu("Save Datas")]
    public void SaveDatas()
    {
        _SaveManager.AddOrReplaceSavedObject(UNLOCKEDTREESAVEID, _UnlockedTrees);

        foreach (var u in _Upgrades)
        {
            var fullID = TREEINDEXSAVEHEADER + u.ID;
            _SaveManager.AddOrReplaceSavedInteger(fullID, u.CurrentIndex);
        }
    }

    [ContextMenu("Load Datas")]
    public void LoadDatas()
    {
        if (_SaveManager == null) return;

        var save = _SaveManager.CurrentSave;
        if (save == null) return;

        var val = save?.SavedObjects?.Find(x => x.ID == UNLOCKEDTREESAVEID);
        if (val?.Value != null)
            _UnlockedTrees = val.Value as List<string>;

        for (int i = 0; i < _Upgrades.Count; i++)
        {
            var fullID = TREEINDEXSAVEHEADER + _Upgrades[i].ID;
            int? index = save?.SavedIntegers?.Find(x => x.ID == fullID)?.Value;

            if (index.HasValue)
                _Upgrades[i].CurrentIndex = index.Value;
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
        if (_selectedTree.IsFinished) DeselectUpgrade();

        RefreshHelpBox(_selectedTree);
        //DeselectUpgrade();
    }
    public void UnlockUpgradeTree(string id)
    {
        if (_UnlockedTrees.Contains(id)) return;

        _UnlockedTrees.Add(id);
        _Upgrades.Find(x => x.ID == id)?.VisualParent.SetActive(true);
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
            case "Drill-Speed":
                upgradeable = FindObjectOfType<Drill>(true);
                upgradeID = "speed";
                break;
            case "Drill-Strength":
                upgradeable = FindObjectOfType<Drill>(true);
                upgradeID = "strength";
                break;
            case "Walk-Speed":
                upgradeable = PlayerInstance.Instance.PlayerController_Ref;
                upgradeID = "speed";
                break;
        }

        upgradeable?.SetUpgradeData(upgradeID, upgrade);
    }
}
