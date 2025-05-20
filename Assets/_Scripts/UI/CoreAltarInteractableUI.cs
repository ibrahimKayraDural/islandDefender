using GameUI;
using Overworld;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoreAltarInteractableUI : ProximityInteractableUI, IUICellOwner
{
    public UICell OldCell { get; set; }
    public UICell CurrentCell { get; set; }

    [SerializeField] GraphicRaycasterScript _GraphicRaycaster;
    [SerializeField] TextMeshProUGUI _DescriptionTitle;
    [SerializeField] TextMeshProUGUI _DescriptionText;
    [SerializeField] GameObject _VisualParent;
    [SerializeField] CoreCell _THECELL;
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
            var core = ((CoreAltarInteractable)CurrentPI);

            if (core?.Core != null && core.IsTaken == false)
            {
                _THECELL.gameObject.SetActive(true);
                _THECELL.Initialize(core.Core);
            }
            else
            {
                _THECELL.Initialize();
                _THECELL.gameObject.SetActive(false);
            }

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
        var newCell = cell as CoreCell;
        return newCell.ItemData != null;
    }
    void IUICellOwner.OnCellClicked(UICell cell)
    {
        var item = ((CoreCell)cell)?.ItemData;
        if (item == null) return;

        if (((CoreAltarInteractable)CurrentPI).TryTakeCore() && _Inventory.TryAddItemFully(item))
        {
            _THECELL.Initialize();
            _THECELL.gameObject.SetActive(false);
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
