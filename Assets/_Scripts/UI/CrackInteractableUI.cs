using GameUI;
using Overworld;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CrackInteractableUI : ProximityInteractableUI, IUICellOwner
{
    public UICell OldCell { get; set; }
    public UICell CurrentCell { get; set; }

    [SerializeField] GraphicRaycasterScript _GraphicRaycaster;
    [SerializeField] TextMeshProUGUI _DescriptionTitle;
    [SerializeField] TextMeshProUGUI _DescriptionText;
    [SerializeField] GameObject _VisualParent;
    [SerializeField] ResourceCell _THECELL;
    GraphicRaycasterScript IUICellOwner.GraphicRaycasterS => _GraphicRaycaster;
    TextMeshProUGUI IUICellOwner.DescriptionTitle => _DescriptionTitle;
    TextMeshProUGUI IUICellOwner.DescriptionText => _DescriptionText;

    Inventory _Inventory
    {
        get
        {
            if (AUTO_inventory == null)
                AUTO_inventory = PlayerInstance.Instance.Inventory_Ref;

            return AUTO_inventory;
        }
    }
    Inventory AUTO_inventory = null;

    public void DisableUI() => SetEnablityGetter(false, null);
    public override void OnEnablityChanged(bool changedTo, List<string> optionalParameters = null)
    {
        if (changedTo == true)
        {
            var item = ((CrackInteractable)CurrentPI)?.CurrentReward;

            if (item != null)
                _THECELL.Initialize(item.Resource, item.Count, 0);
            else
                _THECELL.Initialize();

            _VisualParent.SetActive(true);
        }
        else
        {
            _VisualParent.SetActive(false);
        }
    }

    public void OnHoverInteractableCell(UICell currentCell) { }

    internal override void OnPIUpdate_Start()
    {
        (this as IUICellOwner).OnStart();
    }

    internal override void OnPIUpdate_Loop()
    {
        (this as IUICellOwner).OnLoop();
    }

    internal override void OnPIUpdate_End()
    {
        (this as IUICellOwner).OnEnd();
    }
    bool IUICellOwner.CellIsValid(UICell cell)
    {
        var newCell = cell as InventoryCellScript;
        return newCell.ItemData != null;
    }
    void IUICellOwner.OnCellClicked(UICell cell)
    {
        var item = ((ResourceCell)cell)?.ItemData;
        if (item == null) return;

        if (_Inventory.TryAddItemFully(item))
        {
            ((CrackInteractable)CurrentPI).SpendItem();
            _THECELL.Initialize();
            _DescriptionText.text = "";
            _DescriptionTitle.text = "";
        }
        else
        {
            _DescriptionText.text = "";
            _DescriptionTitle.text = "<color=red>NOT ENOUGH ROOM IN INVENTORY</color>";
        }
    }
}
