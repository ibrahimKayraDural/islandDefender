using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Overworld
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance = null;

        int SlotCount
        {
            get => _slotCount;
            set
            {
                value = Mathf.Max(0, value);
                _slotCount = value;

                IInventoryItem[] temp = _slots;
                _slots = new IInventoryItem[_slotCount];

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
        [SerializeField] TextMeshProUGUI _DebugText;

        IInventoryItem[] _slots;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(this);

            _slots = new IInventoryItem[5];
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
                IInventoryItem itemm = datdat.AsItem(15);
                _slots[_slots.Length - 1] = itemm;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                ResourceData datdat = GLOBAL.GetResourceDatabase().GetDataByID("resource-iron");
                IInventoryItem itemm = datdat.AsItem(15);
                TryAddItemWithSpill(itemm);
            }
            if (Input.GetKeyDown(KeyCode.K))
                _slots[_slots.Length - 1] = null;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                _slots[2] = null;

            _DebugText.text = "";
            foreach (var item in _slots)
            {
                if (IsNull(item))
                {
                    _DebugText.text += "NULL<br>";
                    _DebugText.text += "-------------------------------<br>";
                    continue;
                }

                ResourceItem dataaa = (ResourceItem)item;
                _DebugText.text += item.Count + "<br>";
                _DebugText.text += dataaa.Data.DisplayName + "<br>";
                _DebugText.text += item.IsInitialized + "<br>";
                _DebugText.text += "--------------------------<br>";
            }
        }

        /// <summary>
        /// <para>Tries to add the item to the inventory.</para>
        /// <para>Returns the leftover item (or null if there is no letftover).</para>
        /// </summary>
        public IInventoryItem TryAddItemWithSpill(IInventoryItem itemToAdd)
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

                IInventoryItem slot = _slots[i];
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

            //return the leftover if there are still more items
            return itemToAdd;
        }

        public List<IInventoryItem> TryAddItemWithSpill(IInventoryItem[] itemsToAdd)
        {
            List<IInventoryItem> returnList = new List<IInventoryItem>();
            foreach (var item in itemsToAdd)
            {
                IInventoryItem temp  = TryAddItemWithSpill(item);
                if (temp != null) returnList.Add(temp);
            }
            return returnList;
        }

        bool IsNull(IInventoryItem item)
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
