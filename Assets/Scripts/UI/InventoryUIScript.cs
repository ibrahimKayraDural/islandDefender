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

    public class InventoryUIScript : MonoBehaviour, UserInterface, GridUI
    {
        public bool IsOpen { get; set; }

        [SerializeField] GameObject _Visuals;
        [SerializeField] Transform _InventoryCellParent;
        [SerializeField] GameObject _CellPrefab;
        [SerializeField] TextMeshProUGUI _DescriptionTitle;
        [SerializeField] TextMeshProUGUI _DescriptionText;
        [SerializeField] GraphicRaycasterScript _GraphicRaycaster;
        [SerializeField] GameObject _QuickMenuPrefab;

        bool _currentCellIsValid => _currentCell != null && _currentCell.IsInitialized;
        InventoryCellScript _currentCell;
        InventoryCellScript _oldCell;

        void Start()
        {
            _GraphicRaycaster.e_OnEventDataGathered += OnRaycastDataGathered;
        }
        void Update()
        {
            if (IsOpen == false) return;

            //Right Click Handle
            if (Input.GetMouseButtonDown(1))
            {
                List<RaycastResult> results = _GraphicRaycaster.Raycast();
                //bool isOnQuickMenu = results.Find(x => x.gameObject.TryGetComponent(out InventoryQuickMenu _)).isValid;
                //if (isOnQuickMenu) goto RightClickHandleEND;
                bool isOnValidCell = CheckCell(results, out _);


                if (isOnValidCell)
                {
                    UpdateCurrentCell(results);

                    InventoryQuickMenu iqm = Instantiate(_QuickMenuPrefab, Input.mousePosition, Quaternion.identity, _Visuals.transform)
                                .GetComponent<InventoryQuickMenu>();

                    iqm.Initialize(_currentCell, _GraphicRaycaster); 
                }
            }
        //RightClickHandleEND:;

        }

        void OnRaycastDataGathered(object sender, List<RaycastResult> e)
        {
            //return if quick menu is open
            if (InventoryQuickMenu.IsOpen) return;

            UpdateCurrentCell(e);
        }

        void UpdateCurrentCell(List<RaycastResult> e)
        {
            bool targetIsValid = CheckCell(e, out InventoryCellScript temp);

            _currentCell = temp;

            if (_oldCell != _currentCell)
            {
                _oldCell?.SetHighlight(false);
                if (targetIsValid) _currentCell?.SetHighlight(true);
            }

            _oldCell = _currentCell;

            _DescriptionTitle.text = targetIsValid ? _currentCell.ItemData.DisplayName : "";
            _DescriptionText.text = targetIsValid ? _currentCell.ItemData.Description : "";
        }

        bool CheckCell(List<RaycastResult> resultList, out InventoryCellScript cell)
        {
            InventoryCellScript temp = null;
            RaycastResult result = resultList.Find(x => x.gameObject.TryGetComponent(out temp));
            cell = temp;

            //done this way to prevent Null Reference Exception
            bool targetIsValid = true;
            if (result.isValid == false || temp.ItemData == null) targetIsValid = false;

            return targetIsValid;
        }

        public void ToggleInventory() => SetEnablityGetter(!IsOpen);
        public void RefreshInventory() => (this as GridUI).RefreshGrid(Inventory.Instance.Items.ToArray(), _InventoryCellParent, _CellPrefab);

        public void OnEnablityChanged(bool changedTo)
        {
            RefreshInventory();
            _Visuals.SetActive(changedTo);
            _DescriptionText.text = "";

            if (changedTo == false)
            {
                InventoryQuickMenu.Instance?.Close();
                _currentCell?.SetHighlight(false);
                _oldCell?.SetHighlight(false);
            }
            else { }
        }

        public void SetEnablityGetter(bool setTo) => (this as UserInterface).SetEnablity(setTo);
    }
}
