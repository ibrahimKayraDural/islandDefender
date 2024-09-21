using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequirementUI : ProximityInteractableUI
{
    [SerializeField] GameObject _VisualParent;

    public override void OnEnablityChanged(bool changedTo)
    {
        _VisualParent.SetActive(changedTo);
    }

    internal override void OnPIUpdate_End()
    {

    }

    internal override void OnPIUpdate_Loop()
    {

    }

    internal override void OnPIUpdate_Start()
    {

    }
}
