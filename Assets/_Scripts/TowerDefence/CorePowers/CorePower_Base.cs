using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CorePower_Base : MonoBehaviour
{
    public int UsagesLeft => _usagesLeft;

    [SerializeField] internal CoreData Data;

    internal int _usagesLeft = 0;

    internal virtual void Start()
    {
        ResetUsages();
    }

    public virtual void ResetUsages()
    {
        _usagesLeft = Data.UsePerDay;
    }
    public virtual bool TryActivate()
    {
        if (_usagesLeft <= 0) return false;

        _usagesLeft--;
        OnActivated();
        return true;
    }

    internal abstract void OnActivated();
}
