namespace GameUI
{
    using Overworld;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public interface GridUI
    {
        public void RefreshInventory(InventoryItem[] items, Transform cellParent, GameObject cellPrefab)
        {

            List<Transform> temp = cellParent.Cast<Transform>().ToList();
            foreach (var item in temp)
            {
                MonoBehaviour.Destroy(item.gameObject);
            }

            for (int i = 0; i < items.Length; i++)
            {
                InventoryItem item = items[i];

                //Create empty cell
                InventoryCellScript cell = MonoBehaviour.Instantiate(cellPrefab, cellParent).GetComponent<InventoryCellScript>();

                //Return if there is no data to fill the cell
                if (item == null) continue;

                cell.Initialize(item, i);
            }
        }
    }

}