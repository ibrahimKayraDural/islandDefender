using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolRack : ProximityInteractable
{
    public override string InteractDescription { get => "Change Your Tools"; set { } }

    CanvasManager _canvasManager;

    void Start()
    {
        _canvasManager = CanvasManager.Instance;
    }

    public override void OnInteracted(GameObject interactor)
    {
        if (_canvasManager.TrySetCurrentRackOfToolRackUI(this))
        {
            base.OnInteracted(interactor);
        }
    }

    public override void SetOpennes(bool setTo)
    {
        if (setTo == b_isOpen) return;

        _canvasManager.SetToolRackUIEnablity(setTo);

        base.SetOpennes(setTo);
    }

    internal override void OnClosed()
    {
        base.OnClosed();
        _canvasManager.TrySetCurrentRackOfToolRackUI(null);
    }
}
