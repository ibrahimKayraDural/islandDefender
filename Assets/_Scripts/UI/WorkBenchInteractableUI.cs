using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorkBenchInteractableUI : ProximityInteractableUI
{
    [SerializeField] GameObject _VisualParent;
    [SerializeField, SerializedDictionary("ID", "Object")]
    SerializedDictionary<string, GameObject> _WorkBenches;

    public override void OnEnablityChanged(bool changedTo, List<string> optionalParameters = null)
    {
        _VisualParent.SetActive(changedTo);

        if (changedTo == false || optionalParameters == null || optionalParameters.Count <= 0)
            optionalParameters = new List<string> { GLOBAL.UnassignedString };

        SelectBench(optionalParameters[0]);
    }
    public void DisableUI() => SetEnablityGetter(false, null);

    void SelectBench(string id)
    {
        int i = _WorkBenches.ToList().FindIndex(x => x.Key == id);
        if (id == GLOBAL.UnassignedString) i = -1;

        for (int n = 0; n < _WorkBenches.Count; n++)
        {
            var str = _WorkBenches.Keys.ToArray()[n];
            _WorkBenches[str].SetActive(n == i);
        }
    }

    internal override void OnPIUpdate_End() { }

    internal override void OnPIUpdate_Loop() { }

    internal override void OnPIUpdate_Start() { }
}
