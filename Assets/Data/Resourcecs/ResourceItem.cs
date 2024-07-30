using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overworld;

[System.Serializable]
public class ResourceItem : IInventoryItem
{
    public ResourceData Data => _data;

    public ResourceItem(ResourceData data, int count = 0)
    {
        _data = data;
        AUTO_MAX_HARDCAP = 99;

        IInventoryItem thisObj = (IInventoryItem)this;
        thisObj.MaxItemCount = Data.StackSize;
        thisObj.Count = count;

        AUTO_isInitialized = true;
    }

    ResourceData _data;

    //dont touch values below
    //I hate interfaces
    public int AUTO_count { get; set; }
    public int AUTO_maxItemCount { get; set; }
    public bool AUTO_isInitialized { get; set; } = false;
    public int AUTO_MAX_HARDCAP { get; }

    public bool Compare(IInventoryItem otherItem)
    {
        if ((otherItem is ResourceItem) == false) return false;

        ResourceItem casting = (ResourceItem)otherItem;
        return casting.Data == Data;
    }
}
