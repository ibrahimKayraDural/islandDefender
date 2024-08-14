using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overworld;
using UnityEngine.UI;

[System.Serializable]
public class ResourceItem : InventoryItem
{
    public ResourceData Data => _data;

    ResourceData _data;

    public ResourceItem(ResourceData data, int count = 0)
    {
        _data = data;

        MaxItemCount = Data.StackSize;
        Count = count;
        _UISprite = Data.UISprite;
        _droppedItem = data.DroppedItem;

        _isInitialized = true;
    }

    public override bool Compare(InventoryItem otherItem)
    {
        if ((otherItem is ResourceItem) == false) return false;

        ResourceItem casting = (ResourceItem)otherItem;
        return casting.Data == Data;
    }
}
