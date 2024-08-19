namespace GameUI
{
    using Overworld;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class InventoryUIScript : MonoBehaviour, UserInterface
    {
        public bool IsOpen { get; set; }

        [SerializeField] GameObject _Visuals;
        [SerializeField] Transform _InventoryCellParent;
        [SerializeField] GameObject _CellPrefab;
        [SerializeField] TextMeshProUGUI _DescriptionTitle;
        [SerializeField] TextMeshProUGUI _DescriptionText;
        [SerializeField] GraphicRaycasterScript _GraphicRaycaster;
        [SerializeField] GameObject _QuickMenuPrefab;

        InventoryCellScript _currentCell;

        void Start()
        {
            _GraphicRaycaster.e_OnEventDataGathered += OnRaycastDataGathered;
        }
        void Update()
        {
            if (IsOpen == false) return;

            if(Input.GetMouseButtonDown(1) && _currentCell != null && _currentCell.IsInitialized)
            {
                InventoryQuickMenu iqm = Instantiate(_QuickMenuPrefab, Input.mousePosition, Quaternion.identity, _Visuals.transform)
                    .GetComponent<InventoryQuickMenu>();

                iqm.Initialize(_currentCell, _GraphicRaycaster);
            }
        }

        void OnRaycastDataGathered(object sender, List<RaycastResult> e)
        {
            _currentCell = null;
            RaycastResult result = e.Find(x => x.gameObject.TryGetComponent(out _currentCell));

            //done this way to prevent Null Reference Exception
            bool targetIsValid = true;
            if (result.isValid == false || _currentCell.ItemData == null) targetIsValid = false;

            _DescriptionTitle.text = targetIsValid ? _currentCell.ItemData.DisplayName : "";
            _DescriptionText.text = targetIsValid ? _currentCell.ItemData.Description : "";
        }

        public void ToggleInventory() => SetEnablityGetter(!IsOpen);

        public void OnEnablityChanged(bool changedTo)
        {
            RefreshInventory();
            _Visuals.SetActive(changedTo);
            _DescriptionText.text = "";

            if (changedTo == false)
            {
                InventoryQuickMenu.Instance?.Close();
            }
            else { }
        }

        public void RefreshInventory()
        {
            List<InventoryItem> items = Inventory.Instance.Items;

            List<Transform> temp = _InventoryCellParent.Cast<Transform>().ToList();
            foreach (var item in temp)
            {
                Destroy(item.gameObject);
            }

            for (int i = 0; i < items.Count; i++)
            {
                InventoryItem item = items[i];

                //Create empty cell
                InventoryCellScript cell = Instantiate(_CellPrefab, _InventoryCellParent).GetComponent<InventoryCellScript>();

                //Return if there is no data to fill the cell
                if (item == null) continue;

                cell.Initialize(item, i);
            }
        }

        public void SetEnablityGetter(bool setTo)
        {
            UserInterface ui = this as UserInterface;
            ui.SetEnablity(setTo);
        }
    }
}
