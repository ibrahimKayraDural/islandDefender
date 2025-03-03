using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IUICellOwner
{
    internal abstract GraphicRaycasterScript GraphicRaycasterS { get; }
    internal abstract TextMeshProUGUI DescriptionTitle { get; }
    internal abstract TextMeshProUGUI DescriptionText { get; }

    UICell OldCell { get; set; }
    UICell CurrentCell { get; set; }

    internal virtual void OnStart()
    {
        DescriptionTitle.text = "";
        DescriptionText.text = "";

        CurrentCell = null;
    }
    internal virtual void OnLoop()
    {
        DescriptionTitle.text = "";
        DescriptionText.text = "";
        CurrentCell = null;
        UICell tempCell = null;
        RaycastResult result = GraphicRaycasterS.Raycast().Find(x => x.gameObject.TryGetComponent(out tempCell));
        CurrentCell = tempCell;

        //done this way to prevent Null Reference Exception
        bool targetIsValid = true;
        if (result.isValid == false || CellIsValid(CurrentCell) == false) targetIsValid = false;

        if (targetIsValid && CurrentCell.IsInteractable)
        {
            DescriptionTitle.text = CurrentCell.DisplayName;
            DescriptionText.text = CurrentCell.DisplayDescription;
            OnHoverInteractableCell(CurrentCell);
        }

        if (OldCell != CurrentCell)
        {
            if (OldCell != null) OldCell.SetHighlight(false);
            if (targetIsValid) CurrentCell.SetHighlight(true);
        }

        if (targetIsValid && Input.GetMouseButtonDown(0)) OnCellClicked(CurrentCell);

        OldCell = CurrentCell;
    }

    abstract void OnHoverInteractableCell(UICell currentCell);

    internal virtual void OnEnd()
    {
        if (CurrentCell != null) CurrentCell.SetHighlight(false);
        if (OldCell != null) OldCell.SetHighlight(false);

        CurrentCell = null;
    }

    internal abstract void OnCellClicked(UICell cell);
    internal abstract bool CellIsValid(UICell cell);
}
