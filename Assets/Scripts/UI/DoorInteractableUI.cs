using GameUI;
using Overworld;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorInteractableUI : ProximityInteractableUI, IInventoryCellGrid, IUICellOwner
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
            RefreshGrids();
        }
    }

    void RefreshGrids()
    {
        (this as IInventoryCellGrid).RefreshGrid(_currentDoor.Items, _CellParent, _CellPrefab);
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

    void IUICellOwner.OnCellClicked(UICell cell)
    {
        //SwapCell(cell as InventoryCellScript);
    }

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
