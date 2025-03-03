namespace GameUI
{
    using Overworld;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public interface IInventoryCellGrid
    {
        public void RefreshGrid(InventoryItem[] items, Transform cellParent, GameObject cellPrefab, string ownerID = "")
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

                if (item == null) cell.Initialize();
                else cell.Initialize(item, i, true, ownerID);
            }
        }
    }

}