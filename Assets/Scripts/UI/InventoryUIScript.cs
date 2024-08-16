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

    public class InventoryUIScript : MonoBehaviour
    {
        public bool IsOpen { get; private set; }

        [SerializeField] GameObject _Visuals;
        [SerializeField] Transform _InventoryCellParent;
        [SerializeField] GameObject _CellPrefab;
        [SerializeField] TextMeshProUGUI _DescriptionTitle;
        [SerializeField] TextMeshProUGUI _DescriptionText;
        [SerializeField] GraphicRaycasterScript _GraphicRaycaster;

        private void Start()
        {
            _GraphicRaycaster.e_OnEventDataGathered += OnRaycastDataGathered;
        }

        private void OnRaycastDataGathered(object sender, List<RaycastResult> e)
        {
            InventoryCellScript ics = null;
            RaycastResult result = e.Find(x => x.gameObject.TryGetComponent(out ics));

            //done this way to prevent Null Reference Exception
            bool targetIsValid = true;
            if (result.isValid == false || ics.ItemData == null) targetIsValid = false;

            _DescriptionTitle.text = targetIsValid ? ics.ItemData.DisplayName : "";
            _DescriptionText.text = targetIsValid ? ics.ItemData.Description : "";
        }

        public void ToggleInventory() => SetInventoryEnablity(!IsOpen);

        public void SetInventoryEnablity(bool setTo)
        {
            if (IsOpen == setTo) return;

            RefreshInventory();
            _Visuals.SetActive(setTo);
            _DescriptionText.text = "";

            IsOpen = setTo;
        }

        public void RefreshInventory()
        {
            InventoryItem[] items = Inventory.Instance.Items;

            List<Transform> temp = _InventoryCellParent.Cast<Transform>().ToList();
            foreach (var item in temp)
            {
                Destroy(item.gameObject);
            }

            for (int i = 0; i < items.Length; i++)
            {
                InventoryItem item = items[i];

                //Create empty cell
                InventoryCellScript cell = Instantiate(_CellPrefab, _InventoryCellParent).GetComponent<InventoryCellScript>();

                //Return if there is no data to fill the cell
                if (item == null) continue;

                cell.Initialize(item);
            }
        }
    }
}
