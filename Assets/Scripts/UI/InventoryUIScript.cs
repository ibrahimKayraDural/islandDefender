namespace GameUI
{
    using Overworld;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class InventoryUIScript : MonoBehaviour
    {
        public bool IsOpen { get; private set; }

        [SerializeField] GameObject _Visuals;
        [SerializeField] Transform _InventoryCellParent;
        [SerializeField] GameObject _CellPrefab;

        public void ToggleInventory() => SetInventoryEnablity(!IsOpen);

        public void SetInventoryEnablity(bool setTo)
        {
            if (IsOpen == setTo) return;

            RefreshInventory();
            _Visuals.SetActive(setTo);

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

                GameObject cell = Instantiate(_CellPrefab, _InventoryCellParent);

                if (item == null) continue;

                cell.transform.Find("CountText").GetComponent<TextMeshProUGUI>().text = item.Count.ToString();
                cell.transform.Find("UIImage").GetComponent<Image>().sprite = item.UISprite;
            }
        }
    }
}
