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

    GraphicRaycasterScript IUICellOwner.GraphicRaycasterS => _GraphicRaycaster;
    TextMeshProUGUI IUICellOwner.DescriptionTitle => _DescriptionTitle;
    TextMeshProUGUI IUICellOwner.DescriptionText => _DescriptionText;

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
        ResourceItem[] inventoryItems = _currentDoor.Items;

        for (int i = 0; i < requiredItems.Length; i++)
        {
            ResourceItem item = requiredItems[i];
            int currentAmount = _InventoryInstance.CheckItemCount(item);

            //Create empty cell
            DoorIngredientCell cell = Instantiate(_CellPrefab, _CellParent).GetComponent<DoorIngredientCell>();

            if (item == null) cell.Initialize();
            else cell.Initialize(item.Data, item.Count, currentAmount, i, "DoorInteractable");
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
}
