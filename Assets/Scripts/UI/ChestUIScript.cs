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

    public class ChestUIScript : ProximityInteractableUI, IInventoryCellGrid
    {
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

        Inventory _Inventory
        {
            get
            {
                if(AUTO_inventory == null)
                    AUTO_inventory = PlayerInstance.Instance.Inventory_Ref;

                return AUTO_inventory;
            }
        }
        Inventory AUTO_inventory = null;

        ChestScript _currentChest => CurrentPI as ChestScript;

        void Awake()
        {
            _GraphicRaycaster.RunOnUpdate = false;
        }
        void SwapCell(InventoryCellScript cell)
        {
            if (cell == null || cell.IsInitialized == false) return;
            if (_currentChest == null) return;
            if (_Inventory == null) return;

            bool isChest = cell.ID == "chest-cell";
            IContainer<InventoryItem> from = isChest ? _currentChest : _Inventory;
            IContainer<InventoryItem> to = isChest ? _Inventory : _currentChest;

            InventoryItem item = cell.ItemData;

            InventoryItem remainingItem = to.AddWithSpill(item);
            from.SetItemAtIndex(cell.CellIndex, remainingItem);
            
            RefreshGrids();
        }

        public override void OnEnablityChanged(bool changedTo)
        {
            _VisualParent.SetActive(changedTo);

            if (changedTo == true)
            {
                RefreshGrids();
            }
        }

        void RefreshGrids()
        {
            if (_currentChest == null) Debug.LogError("Can not refresh Chest UI");
            else (this as IInventoryCellGrid).RefreshGrid(_currentChest.Items.ToArray(), _ChestCellParent, _CellPrefab, "chest-cell");

            InventoryItem[] items = PlayerInstance.Instance.Inventory_Ref.Items.ToArray();
            if (items == null) Debug.LogError("Can not refresh Inventory UI (ChestUIScript)");
            else (this as IInventoryCellGrid).RefreshGrid(items, _InventoryCellParent, _CellPrefab, "inventory-cell");
        }


        InventoryCellScript _currentCell = null;
        internal override void OnPIUpdate_Start()
        {
            _DescriptionTitle.text = "";
            _DescriptionText.text = "";

            _currentCell = null;
        }

        internal override void OnPIUpdate_Loop()
        {
            //Input start
            foreach (var key in ChestCloseKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    CurrentPI.SetOpennes(false);
                    goto InputEND;
                }
            }
            if (Input.GetButtonDown("Interact") || Input.GetButtonDown("Exit"))
            {
                CurrentPI.SetOpennes(false);
                goto InputEND;
            }
        InputEND:;//input end

            _currentCell = null;
            RaycastResult result = _GraphicRaycaster.Raycast().Find(x => x.gameObject.TryGetComponent(out _currentCell));

            //done this way to prevent Null Reference Exception
            bool targetIsValid = true;
            if (result.isValid == false || _currentCell.ItemData == null) targetIsValid = false;

            _DescriptionTitle.text = targetIsValid ? _currentCell.ItemData.DisplayName : "";
            _DescriptionText.text = targetIsValid ? _currentCell.ItemData.Description : "";

            if (_oldCell != _currentCell)
            {
                if (_oldCell != null) _oldCell.SetHighlight(false);
                if (targetIsValid) _currentCell.SetHighlight(true);
            }

            if (targetIsValid && Input.GetMouseButtonDown(0)) SwapCell(_currentCell);

            _oldCell = _currentCell;
        }

        internal override void OnPIUpdate_End()
        {
            if (_currentCell != null) _currentCell.SetHighlight(false);
            if (_oldCell != null) _oldCell.SetHighlight(false);

            _currentCell = null;
        }
    }
}
