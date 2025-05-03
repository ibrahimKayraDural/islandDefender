using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefence;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

public class TurretCraftingUIController : MonoBehaviour
{
    [SerializeField] Transform _CellParent;
    [SerializeField] GameObject _CellPrefab;
    [SerializeField] CraftingInfoBar _CraftingInfoBar;
    [SerializeField] TowerDefenceGridManager _GridManager;
    GameplayManager _GameplayManager
    {
        get
        {
            if (AUTO_GameplayManager == null)
                AUTO_GameplayManager = GameplayManager.Instance;
            return AUTO_GameplayManager;
        }
    }
    GameplayManager AUTO_GameplayManager = null;

    ActiveTurretManager _ActiveTurretManager
    {
        get
        {
            if (AUTO_ActiveTurretManager == null)
                AUTO_ActiveTurretManager = ActiveTurretManager.Instance;
            return AUTO_ActiveTurretManager;
        }
    }
    ActiveTurretManager AUTO_ActiveTurretManager = null;

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

    void Start()
    {
        Refresh();
    }

    void OnEnable()
    {
        GameplayManager.e_OnUnlockedTurretListChanged += RefreshHandler;
    }
    void OnDisable()
    {
        GameplayManager.e_OnUnlockedTurretListChanged -= RefreshHandler;
    }

    void RefreshHandler(object sender, List<TurretData> e) => Refresh();
    public void Refresh()
    {
        foreach (Transform child in _CellParent) Destroy(child.gameObject);

        var turrets = _GameplayManager.UnlockedTurrets.Where(x => x.ControlType != TurretControlType.Manual).ToList();
        foreach (var turret in turrets)
        {
            var CUCS = Instantiate(_CellPrefab, _CellParent).GetComponent<CraftingUICellScript>();
            CUCS.Initialize(turret, this);
        }
    }
    public void Craft(TurretData turret)
    {
        var tile = _GridManager.GetFirstFreeTile();
        if (tile == null)
        {
            WriteErrorMessage("No place in field");
            return;
        }
        if (turret == null || _BaseResourceController.TryBuyTurret(turret) == false)
        {
            WriteErrorMessage("Insufficent funds");
            return;
        }

        _ActiveTurretManager.PlaceTurret(turret, tile);
    }
    public void SendDataToBar(CraftingUICellScript cell)
    {
        _CraftingInfoBar.Refresh(cell.TurretData, cell.GetMiddlePos().x);
    }
    public void SetBarEnablity(bool setTo) => _CraftingInfoBar.SetEnablity(setTo);

    void WriteErrorMessage(string message)
    {
        _CraftingInfoBar.SetErrorMessage(message);
    }
}
