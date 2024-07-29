using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    [System.Serializable] public class InventorySlot
    {
        int MAX_HARDCAP = 99;

        public ResourceData Item => _item;
        public int Count
        {
            get => _count;
            set => _count = Mathf.Clamp(value, 0, MaxItemCount);
        }
        public int MaxItemCount
        {
            get => _maxItemCount;
            set => _maxItemCount = Mathf.Clamp(value, 1, MAX_HARDCAP);
        }
        public bool IsInitialized => _isInitialized;

        ResourceData _item;
        int _count;
        int _maxItemCount;
        bool _isInitialized = false;

        public InventorySlot(ResourceData item, int count, int maxCount)
        {
            _item = item;
            MaxItemCount = maxCount;
            Count = count;
            _isInitialized = true;
        }
    }
}
