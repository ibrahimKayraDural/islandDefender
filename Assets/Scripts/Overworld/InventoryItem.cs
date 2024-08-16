using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    public abstract class InventoryItem
    {
        public int Count
        {
            get => Mathf.Clamp(_count, 0, MaxItemCount);
            set => _count = Mathf.Clamp(value, 0, MaxItemCount);
        }
        public int MaxItemCount
        {
            get => Mathf.Clamp(_maxItemCount, 1, _maxHardcap);
            set => _maxItemCount = Mathf.Clamp(value, 1, _maxHardcap);
        }
        public int RemainingSpace => _maxItemCount - _count;
        public bool IsInitialized => _isInitialized;
        public Sprite UISprite => _UISprite;
        public GameObject DroppedItem => _droppedItem;
        public string DisplayName => _displayName;
        public string Description => _description;

        internal string _description = GLOBAL.UnassignedString;
        internal string _displayName = GLOBAL.UnassignedString;
        internal GameObject _droppedItem;
        internal Sprite _UISprite;
        internal int _maxHardcap = 99;
        internal int _count = 0;
        internal int _maxItemCount = 1;
        internal bool _isInitialized = false;

        public InventoryItem(int count, int maxCount, Sprite uiSprite, GameObject droppedItem, string displayName, string description)
        {
            _UISprite = uiSprite;
            Count = count;
            MaxItemCount = maxCount;
            _droppedItem = droppedItem;
            _displayName = displayName;
            _description = description;

            _isInitialized = true;
        }
        public InventoryItem()
        {
            _isInitialized = true;
        }

        /// <summary>
        /// <para>Adds the amount to the slot and returns the remaining amount</para>
        /// </summary>
        public int AddWithSpill(int amount)
        {
            if (amount <= RemainingSpace)
            {
                Count += amount;
                return 0;
            }

            amount -= RemainingSpace;
            Count += RemainingSpace;
            return amount;
        }
        public abstract bool Compare(InventoryItem otherItem);
        public void Drop(Vector3 dropAt)
        {
            dropAt.y = 100;
            Ray ray = new Ray(dropAt, -Vector3.up);
            if (Physics.Raycast(ray, out RaycastHit hit, 200, 1 << 11))
            {
                dropAt = hit.point;

                RaycastHit[] hits = Physics.RaycastAll(new Vector3(dropAt.x, -100, dropAt.z), Vector3.up, 200, ~0, QueryTriggerInteraction.Collide);
                foreach (var h in hits)
                {
                    GameObject collidedGo = h.collider.gameObject;
                    if (collidedGo.TryGetComponent(out DroppedInventoryItem droppedII) == false) continue;

                    droppedII.TryAddItem(this, out int spill);
                    Count = spill;
                    if (Count == 0) break;
                }

                if (Count > 0)
                {
                    GameObject go = GameObject.Instantiate(DroppedItem, dropAt, Quaternion.identity) as GameObject;

                    if (go.TryGetComponent(out DroppedInventoryItem dit) == false)
                    { dit = go.AddComponent<DroppedInventoryItem>(); }

                    dit.Instantiate(this); 
                }
            }
        }
    }
}
