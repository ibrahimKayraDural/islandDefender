using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Overworld
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance = null;

        public InventoryItem[] Items => _slots;

        int SlotCount
        {
            get => _slotCount;
            set
            {
                value = Mathf.Max(0, value);
                _slotCount = value;

                InventoryItem[] temp = _slots;
                _slots = new InventoryItem[_slotCount];

                for (int i = 0; i < temp.Length; i++)
                {
                    if (i < _slots.Length)
                    {
                        if (IsNull(temp[i]) == false)
                        {
                            _slots[i] = temp[i];
                        }
                    }
                    else
                    {
                        TryAddItemWithSpill(temp[i]);
                    }
                }
            }
        }

        [SerializeField, Min(0)] int _slotCount = 5;

        InventoryItem[] _slots;
        CanvasManager CMInstance;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(this);

            _slots = new InventoryItem[5];
        }

        void Start()
        {
            CMInstance = CanvasManager.Instance;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
                SlotCount++;
            if (Input.GetKeyDown(KeyCode.O))
                SlotCount--;
            if (Input.GetKeyDown(KeyCode.L))
            {
                ResourceData datdat = GLOBAL.GetResourceDatabase().GetDataByID("resource-iron");
                InventoryItem itemm = datdat.AsItem(15);
                _slots[_slots.Length - 1] = itemm;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                ResourceData datdat = GLOBAL.GetResourceDatabase().GetDataByID("resource-iron");
                InventoryItem itemm = datdat.AsItem(15);
                TryAddItemWithSpill(itemm);
            }
            if (Input.GetKeyDown(KeyCode.K))
                _slots[_slots.Length - 1] = null;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                _slots[2] = null;

            if (Input.anyKeyDown) CanvasManager.Instance.RefreshInventory();
        }

        /// <summary>
        /// <para>Tries to add the item to the inventory.</para>
        /// <para>Returns the leftover item (or null if there is no letftover).</para>
        /// </summary>
        public InventoryItem TryAddItemWithSpill(InventoryItem itemToAdd, bool dropTheSpill = false)
        {
            //return if slotToAdd is invalid
            if (IsNull(itemToAdd)) return itemToAdd;

            int nullIndex = -1;

            //try to divide the amount to existing items of the same type
            for (int i = 0; i < _slots.Length; i++)
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
                    if (itemToAdd.Count == 0) return null;
                }
            }

            //if an empty index was encountered before, register the item there (for optimization reasons)
            //if not, find an empty slot register the item there
            if (nullIndex >= 0)
            {
                _slots[nullIndex] = itemToAdd;
                return null;
            }
            else
            {
                for (int i = 0; i < _slots.Length; i++)
                {
                    if (IsNull(_slots[i]))
                    {
                        _slots[i] = itemToAdd;
                        return null;
                    }
                }
            }

            //drop the rest if it was asked for
            if(dropTheSpill)
            {
                itemToAdd.Drop(transform.position);
                return null;
            }

            //return the leftover if there are still more items
            return itemToAdd;
        }

        public List<InventoryItem> TryAddItemWithSpill(InventoryItem[] itemsToAdd)
        {
            List<InventoryItem> returnList = new List<InventoryItem>();
            foreach (var item in itemsToAdd)
            {
                InventoryItem temp  = TryAddItemWithSpill(item);
                if (temp != null) returnList.Add(temp);
            }
            return returnList;
        }

        bool IsNull(InventoryItem item)
        {
            //since c# is a dumbass it initializes the item as null, but
            //changes it to an unitialized version (which is not null) a frame later.
            //This causes massive issues but a work-around I found is to check both anyways.
            //This method does that.
            if (item == null) return true;
            else if (item.IsInitialized == false) return true;
            return false;
        }
    }
}
