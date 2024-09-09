using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolRack : ProximityInteractable
{
    public override string InteractDescription { get => "Change Your Tools"; set { } }

    CanvasManager CanvasManagerGetter
    {
        get
        {
            if (AUTO_canvasManager == null)
                AUTO_canvasManager = CanvasManager.Instance;

            return AUTO_canvasManager;
        }
    }
    CanvasManager AUTO_canvasManager = null;

    public override void OnInteracted(GameObject interactor)
    {
        if (CanvasManagerGetter.TrySetCurrentRackOfToolRackUI(this))
        {
            base.OnInteracted(interactor);
        }
    }

    public override void SetOpennes(bool setTo)
    {
        if (setTo == b_isOpen) return;

        CanvasManagerGetter.SetToolRackUIEnablity(setTo);

        base.SetOpennes(setTo);
    }

    internal override void OnClosed()
    {
        base.OnClosed();
        CanvasManagerGetter.TrySetCurrentRackOfToolRackUI(null);
    }
}
