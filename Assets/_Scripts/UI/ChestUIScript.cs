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
    using UnityEngine.UI;

    public class ChestUIScript : ProximityInteractableUI, IInventoryCellGrid, IUICellOwner
    {
        const string INVENTORY_ID = "inventory-cell";
        const string CHEST_ID = "chest-cell";

        [SerializeField] GameObject _VisualParent;
        [SerializeField] Transform _ChestCellParent;
        [SerializeField] Transform _InventoryCellParent;
        [SerializeField] GameObject _CellPrefab;

        [SerializeField] GraphicRaycasterScript _GraphicRaycaster;
        [SerializeField] TextMeshProUGUI _DescriptionTitle;
        [SerializeField] TextMeshProUGUI _DescriptionText;
        [SerializeField] Image _DescriptionIcon;

        Inventory _Inventory
        {
            get
            {
                if (AUTO_inventory == null)
                    AUTO_inventory = PlayerInstance.Instance.Inventory_Ref;

                return AUTO_inventory;
            }
        }
        Inventory AUTO_inventory = null;

        ChestScript _currentChest => CurrentPI as ChestScript;

        public UICell OldCell { get; set; }
        public UICell CurrentCell { get; set; }

        GraphicRaycasterScript IUICellOwner.GraphicRaycasterS => _GraphicRaycaster;

        TextMeshProUGUI IUICellOwner.DescriptionTitle => _DescriptionTitle;

        TextMeshProUGUI IUICellOwner.DescriptionText => _DescriptionText;

        void Awake()
        {
            _GraphicRaycaster.RunOnUpdate = false;
        }
        void SwapCell(InventoryCellScript cell)
        {
            if (cell == null || cell.IsInitialized == false) return;
            if (_currentChest == null) return;
            if (_Inventory == null) return;

            bool isChest = cell.OwnerID == CHEST_ID;
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
            else (this as IInventoryCellGrid).RefreshGrid(_currentChest.Items.ToArray(), _ChestCellParent, _CellPrefab, CHEST_ID);

            InventoryItem[] items = PlayerInstance.Instance.Inventory_Ref.Items.ToArray();
            if (items == null) Debug.LogError("Can not refresh Inventory UI (ChestUIScript)");
            else (this as IInventoryCellGrid).RefreshGrid(items, _InventoryCellParent, _CellPrefab, INVENTORY_ID);
        }

        void HandleDescriptionSprite(Sprite setTo)
        {
            _DescriptionIcon.sprite = setTo;
            _DescriptionIcon.color = setTo != null ? Color.white : Color.clear;
        }

        internal override void OnPIUpdate_Start()
        {
            HandleDescriptionSprite(null);
            (this as IUICellOwner).OnStart();
        }

        internal override void OnPIUpdate_Loop()
        {
            HandleDescriptionSprite(null);
            (this as IUICellOwner).OnLoop();
        }

        internal override void OnPIUpdate_End()
        {
            HandleDescriptionSprite(null);
            (this as IUICellOwner).OnEnd();
        }

        void IUICellOwner.OnCellClicked(UICell cell)
        {
            SwapCell(cell as InventoryCellScript);
        }

        bool IUICellOwner.CellIsValid(UICell cell)
        {
            var newCell = cell as InventoryCellScript;
            if (newCell == null) return false;
            return newCell.ItemData != null;
        }

        public void OnHoverInteractableCell(UICell currentCell)
        {
            HandleDescriptionSprite(currentCell.UISprite);
        }
    }
}
