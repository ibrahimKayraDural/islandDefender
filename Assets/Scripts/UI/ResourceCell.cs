using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCell : InventoryCellScript
{
    public ResourceData Data => _data;
    public int Amount => _amount;

    ResourceData _data = null;
    int _amount = 0;

    public void Initialize(ResourceData data, int amount = 1)
    {
        _data = data;
        _amount = amount;
    }
}
