using GameUI;
using Overworld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorInteractableUI : ProximityInteractableUI, IUICellOwner
{
    public UICell OldCell { get; set; }
    public UICell CurrentCell { get; set; }

    [SerializeField] GameObject _VisualParent;
    [SerializeField] Transform _CellParent;
    [SerializeField] GameObject _CellPrefab;

    [SerializeField] GraphicRaycasterScript _GraphicRaycaster;
    [SerializeField] TextMeshProUGUI _DescriptionTitle;
    [SerializeField] TextMeshProUGUI _DescriptionText;
    [SerializeField] Image _DescriptionIcon;

    DoorInteractable _currentDoor => CurrentPI as DoorInteractable;
    Inventory _InventoryInstance
    {
        get
        {
            if (AUTO_InventoryInstance == null) AUTO_InventoryInstance = PlayerInstance.Instance.Inventory_Ref;
            return AUTO_InventoryInstance;
        }
    }
    Inventory AUTO_InventoryInstance = null;

    BaseResourceController _BaseResourceController
    {
        get
        {
            if (AUTO_BaseResourceController == null) AUTO_BaseResourceController = BaseResourceController.Instance;
            return AUTO_BaseResourceController;
        }
    }
    BaseResourceController AUTO_BaseResourceController = null;

    GraphicRaycasterScript IUICellOwner.GraphicRaycasterS => _GraphicRaycaster;
    TextMeshProUGUI IUICellOwner.DescriptionTitle => _DescriptionTitle;
    TextMeshProUGUI IUICellOwner.DescriptionText => _DescriptionText;

    Dictionary<ResourceData, Vector2Int> _itemsInInventoryAndBase = new Dictionary<ResourceData, Vector2Int>();

    void Awake()
    {
        _GraphicRaycaster.RunOnUpdate = false;
    }

    public override void OnEnablityChanged(bool changedTo)
    {
        _VisualParent.SetActive(changedTo);

        if (changedTo == true)
        {
            RefreshGrid();
        }
    }

    void RefreshGrid()
    {
        foreach (var item in _CellParent.Cast<Transform>()) MonoBehaviour.Destroy(item.gameObject);

        ResourceItem[] requiredItems = _currentDoor.Items;
        //ResourceItem[] inventoryItems = _currentDoor.Items;
        _itemsInInventoryAndBase = new Dictionary<ResourceData, Vector2Int>();

        for (int i = 0; i < requiredItems.Length; i++)
        {
            ResourceItem item = requiredItems[i];
            int currentAmountInventory = _InventoryInstance.CheckItemCount(item);
            int currentAmountBase = _BaseResourceController.CheckItemCount(item);

            _itemsInInventoryAndBase.Add(item.Data, new Vector2Int(currentAmountInventory, currentAmountBase));

            //Create empty cell
            DoorIngredientCell cell = Instantiate(_CellPrefab, _CellParent).GetComponent<DoorIngredientCell>();

            if (item == null) cell.Initialize();
            else cell.Initialize(item.Data, item.Count, currentAmountInventory + currentAmountBase, i, "DoorInteractable");
        }
    }

    void HandleDescriptionSprite(Sprite setTo)
    {
        _DescriptionIcon.sprite = setTo;
        _DescriptionIcon.color = setTo != null ? Color.white : Color.clear;
    }

    internal override void OnPIUpdate_Start()
    {
        HandleDescriptionSprite(null);
        (this as IUICellOwner).OnStart();
    }

    internal override void OnPIUpdate_Loop()
    {
        HandleDescriptionSprite(null);
        (this as IUICellOwner).OnLoop();
    }

    internal override void OnPIUpdate_End()
    {
        HandleDescriptionSprite(null);
        (this as IUICellOwner).OnEnd();
    }

    void IUICellOwner.OnCellClicked(UICell cell) { }

    bool IUICellOwner.CellIsValid(UICell cell)
    {
        var newCell = cell as InventoryCellScript;
        return newCell.ItemData != null;
    }

    public void OnHoverInteractableCell(UICell currentCell)
    {
        HandleDescriptionSprite(currentCell.UISprite);
    }

    public void OnFinished()
    {
        ResourceItem[] requiredItems = _currentDoor.Items;

        foreach (var item in requiredItems)
        {
            Vector2Int inventoryAndBaseCount = _itemsInInventoryAndBase[item.Data];
            if (inventoryAndBaseCount.x + inventoryAndBaseCount.y < item.Count) return;
        }

        foreach (var item in requiredItems)
        {
            Vector2Int inventoryAndBaseCount = _itemsInInventoryAndBase[item.Data];
            if (_InventoryInstance.TryUseItems(item.Data.AsItem(inventoryAndBaseCount.x)) == false) return;
            _BaseResourceController.TrySpendResource(item.Data, inventoryAndBaseCount.y);
        }

        _currentDoor.OnBought();
        SetEnablityGetter(false);
    }
}
