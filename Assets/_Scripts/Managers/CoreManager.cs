using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreManager : MonoBehaviour
{
    public List<CoreItem> Cores => _cores;

    List<CoreItem> _cores = new();

    public void AddCores(List<CoreItem> cores)
    {
        foreach (var c in cores) AddCore(c);
    }
    public void AddCore(CoreItem core)
    {
        if (_cores.Contains(core)) return;

        _cores.Add(core);
    }
}
