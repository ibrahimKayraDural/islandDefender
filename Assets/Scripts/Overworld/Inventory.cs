using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Overworld
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance = null;

        public List<InventoryItem> Items => _slots.ToList();

        int SlotCount
        {
            get => _slotCount;
            set
            {
                value = Mathf.Max(0, value);
                _slotCount = value;

                ObservableCollection<InventoryItem> temp = _slots;
                _slots = new ObservableCollection<InventoryItem>(new InventoryItem[_slotCount]);

                for (int i = 0; i < temp.Count; i++)
                {
                    if (i < _slots.Count)
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

                _canvasManager.RefreshInventory();
            }
        }

        [SerializeField, Min(0)] int _slotCount = 5;

        ObservableCollection<InventoryItem> _slots;

        CanvasManager _canvasManager;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(this);
        }
        void Start()
        {
            _canvasManager = CanvasManager.Instance;
            _slots = new ObservableCollection<InventoryItem>(new InventoryItem[SlotCount]);
            _slots.CollectionChanged += OnInventoryChanged;
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
                _slots[_slots.Count - 1] = itemm;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                ResourceData datdat = GLOBAL.GetResourceDatabase().GetDataByID("resource-iron");
                InventoryItem itemm = datdat.AsItem(15);
                TryAddItemWithSpill(itemm);
            }
            if (Input.GetKeyDown(KeyCode.K))
                _slots[_slots.Count - 1] = null;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                _slots[2] = null;
        }

        public int CheckItemCount(InventoryItem itemToCheck)
        {
            int totalCount = 0;
            foreach (var slot in _slots)
            {
                if (slot.Compare(itemToCheck)) totalCount += slot.Count;
            }
            return totalCount;
        }

        /// <summary>
        /// Try to remove items from Inventory, if they fully exist
        /// </summary>
        /// <param name="itemsToUse">What items to use with what count</param>
        /// <returns>If items existed and therefore removed</returns>
        public bool TryUseItems(InventoryItem[] itemsToUse)
        {
            foreach (var item in itemsToUse)
            {
                if (CheckItemCount(item) < item.Count) return false;
            }

            foreach (var itemToUse in itemsToUse)
            {
                for (int i = 0; i < _slots.Count; i++)
                {
                    InventoryItem slot = _slots[i];
                    if (IsNull(slot)) continue;

                    if (slot.Compare(itemToUse))
                    {
                        int itemCount = itemToUse.Count;
                        itemToUse.Count -= slot.Count;
                        slot.Count -= itemCount;

                        _canvasManager.RefreshInventory();

                        if (slot.Count <= 0) _slots[i] = null;
                        if (itemToUse.Count <= 0) break;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Try to remove item from Inventory, if it fully exists
        /// </summary>
        /// <param name="itemToUse">What item to use with what count</param>
        /// <returns>If item existed and therefore removed</returns>
        public bool TryUseItems(InventoryItem itemToUse) => TryUseItems(new InventoryItem[] { itemToUse });

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
                    _canvasManager.RefreshInventory();

                    if (itemToAdd.Count == 0)
                    {
                        return null;
                    }
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
                for (int i = 0; i < _slots.Count; i++)
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

        /// <summary>
        /// <para>Tries to add the items to the inventory</para>
        /// <para>Returns the leftover items (or null if there is no letftovers)</para>
        /// </summary>
        public List<InventoryItem> TryAddItemWithSpill(InventoryItem[] itemsToAdd, bool dropTheSpill = false)
        {
            List<InventoryItem> returnList = new List<InventoryItem>();
            foreach (var item in itemsToAdd)
            {
                InventoryItem temp = TryAddItemWithSpill(item, dropTheSpill);
                if (temp != null) returnList.Add(temp);
            }
            return returnList;
        }

        public void DropItemAtIndex(int index, Vector3? dropPosition = null)
        {
            if (index < 0 || index >= _slots.Count) return;

            InventoryItem item = _slots[index];
            _slots[index] = null;

            Vector3 targetPos = dropPosition == null ? transform.position : dropPosition.Value;
            item.Drop(targetPos);

            _canvasManager.RefreshInventory();
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
        void OnInventoryChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _canvasManager.RefreshInventory();
        }
    }
}
