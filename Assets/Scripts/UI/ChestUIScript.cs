namespace GameUI
{
    using Overworld;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class ChestUIScript : MonoBehaviour, IUserInterface, IGridUI
    {
        public ChestScript CurrentChest { get; private set; } = null;
        public bool IsOpen { get; set; }

        [SerializeField] GameObject _VisualParent;
        [SerializeField] Transform _ChestCellParent;
        [SerializeField] Transform _InventoryCellParent;
        [SerializeField] GameObject _CellPrefab;
        [SerializeField] GraphicRaycasterScript _GraphicRaycaster;
        [SerializeField] TextMeshProUGUI _DescriptionTitle;
        [SerializeField] TextMeshProUGUI _DescriptionText;

        List<KeyCode> ChestCloseKeys = new List<KeyCode>() {
            KeyCode.I
        };
        InventoryCellScript _oldCell;
        Inventory _inventory;

        void Awake()
        {
            _GraphicRaycaster.RunOnUpdate = false;
        }
        void Start()
        {
            _inventory = Inventory.Instance;
        }

        public bool TrySetCurrentChest(ChestScript setTo)
        {
            if (CurrentChest == null)//chest is opened
            {
                CurrentChest = setTo;
                StartCoroutine(nameof(ChestUpdate));
            }
            else if (setTo == null)//chest is closed
            {
                CurrentChest = null;
                _breakChestUpdate = true;
            }
            else
            {
                return false;
            }

            return true;
        }

        bool _breakChestUpdate = false;
        IEnumerator ChestUpdate()
        {
            yield return new WaitForSeconds(.1f);

            _DescriptionTitle.text = "";
            _DescriptionText.text = "";

            InventoryCellScript currentCell = null;

            //Update is the inside of this loop.
            //Code above will run once before the update loop.
            while (_breakChestUpdate == false)
            {
                //Input start
                foreach (var key in ChestCloseKeys)
                {
                    if (Input.GetKeyDown(key))
                    {
                        CurrentChest.SetOpennes(false);
                        goto InputEND;
                    }
                }
                if (Input.GetButtonDown("Interact") || Input.GetButtonDown("Exit"))
                {
                    CurrentChest.SetOpennes(false);
                    goto InputEND;
                }
            InputEND:;//input end

                currentCell = null;
                RaycastResult result = _GraphicRaycaster.Raycast().Find(x => x.gameObject.TryGetComponent(out currentCell));

                //done this way to prevent Null Reference Exception
                bool targetIsValid = true;
                if (result.isValid == false || currentCell.ItemData == null) targetIsValid = false;

                _DescriptionTitle.text = targetIsValid ? currentCell.ItemData.DisplayName : "";
                _DescriptionText.text = targetIsValid ? currentCell.ItemData.Description : "";

                if(_oldCell != currentCell)
                {
                    if (_oldCell != null) _oldCell.SetHighlight(false);
                    if (targetIsValid) currentCell.SetHighlight(true);
                }

                if (targetIsValid && Input.GetMouseButtonDown(0)) SwapCell(currentCell);

                _oldCell = currentCell;

                yield return null;
            }
            //Update is the inside of the loop above.
            //Code below will run once after the update loop has ended.

            if (currentCell != null) currentCell.SetHighlight(false);
            if (_oldCell != null) _oldCell.SetHighlight(false);

            _breakChestUpdate = false;
        }

        void SwapCell(InventoryCellScript cell)
        {
            if (cell == null || cell.IsInitialized == false) return;
            if (CurrentChest == null) return;
            if (_inventory == null) return;

            bool isChest = cell.ID == "chest-cell";
            IContainer<InventoryItem> from = isChest ? CurrentChest : _inventory;
            IContainer<InventoryItem> to = isChest ? _inventory : CurrentChest;

            InventoryItem item = cell.ItemData;

            InventoryItem remainingItem = to.AddWithSpill(item);
            from.SetItemAtIndex(cell.CellIndex, remainingItem);
            
            RefreshGrids();
        }

        public void OnEnablityChanged(bool changedTo)
        {
            _VisualParent.SetActive(changedTo);

            if (changedTo == true)
            {
                RefreshGrids();
            }
        }

        void RefreshGrids()
        {
            if (CurrentChest == null) Debug.LogError("Can not refresh Chest UI");
            else (this as IGridUI).RefreshGrid(CurrentChest.Items.ToArray(), _ChestCellParent, _CellPrefab, "chest-cell");

            InventoryItem[] items = Inventory.Instance.Items.ToArray();
            if (items == null) Debug.LogError("Can not refresh Inventory UI (ChestUIScript)");
            else (this as IGridUI).RefreshGrid(items, _InventoryCellParent, _CellPrefab, "inventory-cell");
        }

        public void SetEnablityGetter(bool setTo) => (this as IUserInterface).SetEnablity(setTo);

    }
}
