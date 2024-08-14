namespace Overworld
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class DroppedInventoryItem : MonoBehaviour
    {
        InventoryItem _item;
        bool isInstantiated;
        int debugCount = 0;

        public void Instantiate(InventoryItem item)
        {
            if (isInstantiated) return;

            _item = item;
            debugCount = _item.Count;

            gameObject.tag = "DroppedItem";
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.radius = 1;
            col.isTrigger = true;

            isInstantiated = true;
        }

        public bool TryAddItem(InventoryItem addition, out int spill)
        {
            spill = addition.Count;

            if (addition.Compare(_item) == false) return false;

            spill = _item.AddWithSpill(addition.Count);
            debugCount = _item.Count;

            return spill == 0;
        }
    }

}