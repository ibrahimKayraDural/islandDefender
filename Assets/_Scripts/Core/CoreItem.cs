using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CoreItem : InventoryItem
{
    public CoreData Data => _data;

    CoreData _data;

    public CoreItem(CoreData data)
    {
        _data = data;

        MaxItemCount = 1;
        Count = 1;
        _UISprite = Data.UISprite;
        _droppedItem = null;
        _displayName = data.DisplayName;
        _description = data.Description;

        _isDroppable = false;

        _isInitialized = true;
    }

    public override bool Compare(InventoryItem otherItem)
    {
        if ((otherItem is CoreItem) == false) return false;

        CoreItem casting = (CoreItem)otherItem;
        return casting.Data == Data;
    }
}
