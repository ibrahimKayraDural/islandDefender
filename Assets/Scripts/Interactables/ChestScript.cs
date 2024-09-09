using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : ProximityInteractable, IContainer<InventoryItem>
{
    public List<InventoryItem> Items => _slots;
    public override string InteractDescription { get => "Open Chest"; set { } }

    [SerializeField, Min(1)] int _Capacity = 14;

    List<InventoryItem> _slots;
    CanvasManager _CanvasManager
    {
        get
        {
            if(AUTO_canvasManager == null)
             AUTO_canvasManager = CanvasManager.Instance;

            return AUTO_canvasManager;
        }
    }
    CanvasManager AUTO_canvasManager = null;

    void Start()
    {
        Clean();
    }

    public InventoryItem AddWithSpill(InventoryItem item) => TryAddItemWithSpill(item);
    public int CheckEmptySpaceFor(InventoryItem item)
    {
        int count = 0;
        foreach (var slot in _slots)
        {
            if (IsNull(slot))
            {
                count += item.MaxItemCount;
            }
            else if (slot.Compare(item))
            {
                count += slot.MaxItemCount - slot.Count;
            }
        }
        return count;
    }
    public InventoryItem TryAddItemWithSpill(InventoryItem itemToAdd)
    {
        //return if slotToAdd is invalid
        if (IsNull(itemToAdd)) return itemToAdd;

        int nullIndex = -1;

        //try to divide the amount to existing items of the same type
        for (int i = 0; i < _slots.Count; i++)
        {
            if (IsNull(_slots[i]))
            {
                if (nullIndex == -1) nullIndex = i;

                continue;
            }

            InventoryItem slot = _slots[i];
            if (slot.Compare(itemToAdd))
            {
                //and return if all of the item is divided
                itemToAdd.Count = slot.AddWithSpill(itemToAdd.Count);

                if (itemToAdd.Count == 0)
                {
                    return null;
                }
            }
        }

        //if an empty index was encountered before, register the item there
        //if not, find an empty slot and register the item there
        if (nullIndex >= 0)
        {
            _slots[nullIndex] = itemToAdd;
            return null;
        }
        else
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (IsNull(_slots[i]))
                {
                    _slots[i] = itemToAdd;
                    return null;
                }
            }
        }

        //return the leftover if there are still more items
        return itemToAdd;
    }
    public bool TryAddItemFully(InventoryItem item)
    {
        if (CheckEmptySpaceFor(item) < item.Count) return false;

        TryAddItemWithSpill(item);
        return true;
    }
    public void RemoveAtIndex(int index) => SetItemAtIndex(index, null);
    public void SetItemAtIndex(int index, InventoryItem setTo)
    {
        if (index < 0 || index >= _slots.Count) return;

        _slots[index] = setTo;
    }
    public void Clean()
    {
        _slots = new List<InventoryItem>(new InventoryItem[_Capacity]);
    }

    public override void OnInteracted(GameObject interactor)
    {
        if (_CanvasManager.TrySetCurrentChestOfChestUI(this))
        {
            base.OnInteracted(interactor);
        }
    }
    public override void SetOpennes(bool setTo)
    {
        if (setTo == b_isOpen) return;

        _CanvasManager.SetChestUIEnablity(setTo);

        base.SetOpennes(setTo);
    }
    internal override void OnClosed()
    {
        base.OnClosed();
        _CanvasManager.TrySetCurrentChestOfChestUI(null);
    }
    bool IsNull(InventoryItem item) => GLOBAL.IsNull(item);
}
