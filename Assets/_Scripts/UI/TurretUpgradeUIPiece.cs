using Overworld;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefence;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurretUpgradeUIPiece : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button _UpgradeButton;
    [SerializeField] Image _IconFirst;
    [SerializeField] Image _IconSecond;
    [SerializeField] Image _Arrow;
    [SerializeField] TextMeshProUGUI _TitleFirstTM;
    [SerializeField] TextMeshProUGUI _TitleSecondTM;
    [SerializeField] TextMeshProUGUI _CostTM;

    TurretUnit _unit;
    bool _isInitialized;

    BaseResourceController ResourceController
    {
        get
        {
            if (AUTO_ResourceController == null)
                AUTO_ResourceController = BaseResourceController.Instance;
            return AUTO_ResourceController;
        }
    }
    BaseResourceController AUTO_ResourceController = null;

    TDPlayerController TDPC
    {
        get
        {
            if (AUTO_TDPC == null)
                AUTO_TDPC = TDPlayerController.Instance;
            return AUTO_TDPC;
        }
    }
    TDPlayerController AUTO_TDPC = null;

    void OnDestroy()
    {
        if (_unit != null) _unit.e_OnDeath -= OnUnitDeath;
    }
    public void Initialize(TurretUnit unit)
    {
        if (_isInitialized) return;

        _unit = unit;
        var data = unit.Data;

        _IconFirst.sprite = data.UISprite;
        _TitleFirstTM.text = data.DisplayName;

        TurretUpgradeTree.TurretUpgradePiece nextTurret = null;
        var tree = GLOBAL.GetTurretUpgradeTree().FindTree(data, out int turretIDX);
        if (tree != null && tree.Upgrades.Count > turretIDX + 1 && turretIDX != -1)
        {
            nextTurret = tree.Upgrades[turretIDX + 1];
            _IconSecond.sprite = nextTurret.Turret.UISprite;
            _TitleSecondTM.text = nextTurret.Turret.DisplayName;
        }
        else
        {
            _TitleSecondTM.gameObject.SetActive(false);
            _IconSecond.gameObject.SetActive(false);
            _Arrow.gameObject.SetActive(false);
            _CostTM.text = "TURRET ALREADY AT MAX LEVEL";
            _UpgradeButton.interactable = false;
        }

        if (nextTurret != null)
        {
            _CostTM.text = "";
            for (int i = 0; i < nextTurret.Costs.Count; i++)
            {
                var c = nextTurret.Costs[i];
                _CostTM.text += c.Resource.DisplayName + " X " + c.Amount;
                if (i + 1 < nextTurret.Costs.Count) _CostTM.text += "\n";
            }
        }
        _UpgradeButton.onClick.AddListener(() => TryUpgrade(nextTurret));
        _unit.e_OnDeath += OnUnitDeath;
    }

    bool TryUpgrade(TurretUpgradeTree.TurretUpgradePiece nextTurret)
    {
        if (nextTurret == null) return false;
        if (ResourceController.TrySpendResource(nextTurret.Costs.ToArray()) == false) return false;

        var tile = _unit._parentTile;
        _unit.KillSelf(false);
        ActiveTurretManager.Instance.PlaceTurret(nextTurret.Turret, tile);

        return true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _unit.SetHighlight(false, this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TDPC?.DeselectTurretToSwap();
        _unit.SetHighlight(true, this);
    }

    void OnUnitDeath(object sender, System.EventArgs e)
    {
        if (gameObject == null) return;

        Destroy(gameObject);
    }
}
