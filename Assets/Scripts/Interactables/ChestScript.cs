using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour, IInteractable, IContainer<InventoryItem>
{
    public List<InventoryItem> Items => _slots;
    public string InteractDescription { get => "Open Chest"; set { } }

    [SerializeField, Min(1)] int _Capacity = 14;
    [SerializeField] float _ForgetDistance = 1;
    [SerializeField] Animator _Animator;

    List<InventoryItem> _slots;
    bool _isOpen = false;
    CanvasManager _canvasManager;
    Transform _currentInteractor = null;

    void Start()
    {
        _canvasManager = CanvasManager.Instance;

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

    public void OnInteracted(GameObject interactor)
    {
        _currentInteractor = interactor.transform;

        if (_canvasManager.TrySetCurrentChestOfChestUI(this))
        {
            SetOpennes(true);
        }
    }
    public void SetOpennes(bool setTo)
    {
        if (_isOpen == setTo) return;

        _isOpen = setTo;
        _Animator.SetBool("IsOpen", setTo);
        _canvasManager.SetChestUIEnablity(setTo);

        if (setTo)
        {
            StartCoroutine(nameof(CheckForget));
        }
        else
        {
            StopCoroutine(nameof(CheckForget));
            _canvasManager.TrySetCurrentChestOfChestUI(null);
        }
    }

    IEnumerator CheckForget()
    {
        Vector3 pos = transform.position;
        while (true)
        {
            if (Vector3.Distance(_currentInteractor.position, pos) >= _ForgetDistance)
            {
                _currentInteractor = null;
                SetOpennes(false);
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    bool IsNull(InventoryItem item) => GLOBAL.IsNull(item);
}
