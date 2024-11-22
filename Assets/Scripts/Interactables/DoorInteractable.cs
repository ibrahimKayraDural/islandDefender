using AYellowpaper.SerializedCollections;
using GameUI;
using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : ProximityInteractable
{
    public override string InteractDescription { get => "Check possible descent point"; set { } }

    public ResourceItem[] Items
    {
        get
        {
            List<ResourceItem> items = new List<ResourceItem>();
            foreach (var i in _items) items.Add(i.Key.AsItem(i.Value));
            return items.ToArray();
        }
    }

    [SerializeField, SerializedDictionary("Data","Amount")] SerializedDictionary<ResourceData, int> _items;
}
