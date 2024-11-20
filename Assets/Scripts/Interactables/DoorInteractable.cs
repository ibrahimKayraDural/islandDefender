using AYellowpaper.SerializedCollections;
using GameUI;
using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : ProximityInteractable
{
    public override string InteractDescription { get => "Check possible descent point"; set { } }

    public InventoryItem[] Items
    {
        get
        {
            List<InventoryItem> items = new List<InventoryItem>();
            foreach (var i in _items) items.Add(new ResourceItem(i.Key, i.Value));
            return items.ToArray();
        }
    }

    [SerializeField, SerializedDictionary("Data","Amount")] SerializedDictionary<ResourceData, int> _items;
}
