using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static SaveSystem.SaveManager;

namespace Overworld
{
    public class Inventory : MonoBehaviour, IContainer<InventoryItem>
    {
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

                _CanvasManager.RefreshInventory();
            }
        }

        [SerializeField, Min(0)] int _slotCount = 5;

        ObservableCollection<InventoryItem> _slots;

        CanvasManager _CanvasManager
        {
            get
            {
                if (AUTO_canvasManager == null)
                    AUTO_canvasManager = CanvasManager.Instance;

                return AUTO_canvasManager;
            }
        }
        CanvasManager AUTO_canvasManager = null;

        SaveManager _SaveManager
        {
            get
            {
                if (AUTO_saveManager == null)
                    AUTO_saveManager = SaveManager.Instance;

                return AUTO_saveManager;
            }
        }
        SaveManager AUTO_saveManager = null;

        void Start()
        {
            AUTO_canvasManager = CanvasManager.Instance;
            Clean();
            LoadInventory();
            _slots.CollectionChanged += OnInventoryChanged;
        }
        void Update()
        {
            //Delete this before taking a final build
            Debugmethod();
        }

        bool _isSaved;//prevents race conditions
        void OnApplicationQuit()
        {
            SaveInventory();
            _isSaved = true;
        }
        void OnDestroy()
        {
            if (_isSaved == false) SaveInventory();
        }

        [ContextMenu("Save Inventory")]
        void SaveInventory()
        {
            List<InventoryItemSaveData> inv = Enumerable.Repeat<InventoryItemSaveData>(null, SlotCount).ToList();
            for (int i = 0; i < SlotCount; i++)
            {
                var item = _slots[i];

                if (IsNull(item)) continue;
                else if (item as ResourceItem != null)
                {
                    var temp = (ResourceItem)item;
                    inv[i] = new InventoryItemSaveData(InventoryItem.ResourceItemTypeID, temp.Data.ID, temp.Count);
                }
                else if (item as CoreItem != null)
                {
                    var temp = (CoreItem)item;
                    inv[i] = new InventoryItemSaveData(InventoryItem.CoreItemTypeID, temp.Data.ID, 1);
                }
                else continue;
            }

            _SaveManager.ReplacePlayerInventory(inv);
        }

        [ContextMenu("Load Inventory")]
        public void LoadInventory()
        {
            if (_SaveManager == null) return;

            var save = _SaveManager.CurrentSave;
            if (save == null) return;

            List<InventoryItemSaveData> inv = save.PlayerInventory;
            if (inv == null) return;
            if (inv.Count == 0) return;

            SlotCount = inv.Count;

            var resourceDB = GLOBAL.GetResourceDatabase();
            var coreDB = GLOBAL.GetCoreDatabase();

            for (int i = 0; i < inv.Count; i++)
            {
                var item = inv[i];

                if (item == null) continue;
                else if (item.TypeID == InventoryItem.ResourceItemTypeID)
                {
                    var data = resourceDB.GetDataByDisplayNameOrID(item.ID);
                    _slots[i] = new ResourceItem(data, item.Count);
                }
                else if (item.TypeID == InventoryItem.CoreItemTypeID)
                {
                    var data = coreDB.GetDataByDisplayNameOrID(item.ID);
                    _slots[i] = new CoreItem(data);
                }
                else continue;
            }

            _CanvasManager.RefreshInventory();
        }

        void Debugmethod()
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

            if (Input.GetKeyDown(KeyCode.C))
                Clean();
            if (Input.GetKeyDown(KeyCode.J))
            {
                var datdat = GLOBAL.GetCoreDatabase().GetDataByID("core-jungle");
                InventoryItem itemm = datdat.AsItem();
                TryAddItemFully(itemm);
            }
        }

        public int CheckItemCount(InventoryItem itemToCheck)
        {
            int totalCount = 0;
            foreach (var slot in _slots)
            {
                if (slot == null) continue;
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

                        _CanvasManager.RefreshInventory();

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
                    _CanvasManager.RefreshInventory();

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
            if (dropTheSpill)
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
            List<InventoryItem> returnList = new();
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

            _CanvasManager.RefreshInventory();
        }

        bool IsNull(InventoryItem item) => GLOBAL.IsNull(item);
        void OnInventoryChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _CanvasManager.RefreshInventory();
        }

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
        public bool TryAddItemFully(InventoryItem item)
        {
            if (CheckEmptySpaceFor(item) < item.Count) return false;

            TryAddItemWithSpill(item);
            _CanvasManager?.RefreshInventory();
            return true;
        }
        public void Clean()
        {
            _slots = new ObservableCollection<InventoryItem>(new InventoryItem[SlotCount]);

            _CanvasManager?.RefreshInventory();
        }
        public InventoryItem AddWithSpill(InventoryItem item) => TryAddItemWithSpill(item);
        public void RemoveAtIndex(int index) => SetItemAtIndex(index, null);
        public void SetItemAtIndex(int index, InventoryItem setTo)
        {
            if (index < 0 || index >= _slots.Count) return;

            _slots[index] = setTo;

            _CanvasManager.RefreshInventory();
        }
    }
}
