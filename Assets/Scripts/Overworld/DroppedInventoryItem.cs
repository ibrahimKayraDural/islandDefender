namespace Overworld
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class DroppedInventoryItem : MonoBehaviour, IInteractable
    {
        public string InteractDescription
        {
            get => _item == null ? "Pick Up" : "Pick up " + _item.DisplayName + " X " + _item.Count;
            set { }
        }

        InventoryItem _item;
        bool isInstantiated;

        public void Instantiate(InventoryItem item)
        {
            if (isInstantiated) return;

            _item = item;

            gameObject.tag = "DroppedItem";
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.radius = 1;
            col.isTrigger = true;

            isInstantiated = true;
        }

        public void OnInteracted()
        {
            _item = Inventory.Instance.TryAddItemWithSpill(_item);
            if (_item == null) Destroy(gameObject);
        }

        public bool TryAddItem(InventoryItem addition, out int spill)
        {
            spill = addition.Count;

            if (addition.Compare(_item) == false) return false;

            spill = _item.AddWithSpill(addition.Count);

            return spill == 0;
        }
    }

}