using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefence;
using UnityEngine;
using UnityEngine.UI;

public class TurretCraftingUIController : MonoBehaviour
{
    [SerializeField] Transform _CellParent;
    [SerializeField] GameObject _CellPrefab;
    [SerializeField] OwnedTurretController _OwnedTurretController;
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

        var turrets = _GameplayManager.UnlockedTurrets.Where(x => x.Type != TurretControlType.Manual).ToList();
        foreach (var turret in turrets)
        {
            var CUCS = Instantiate(_CellPrefab, _CellParent).GetComponent<CraftingUICellScript>();
            CUCS.Initialize(turret, this);
        }
    }
    public void Craft(TurretData turret)
    {
        _OwnedTurretController.TryBuyTurret(turret);
    }
}
