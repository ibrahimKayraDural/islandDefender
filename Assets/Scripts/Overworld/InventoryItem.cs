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

        internal Sprite _UISprite;
        internal int _maxHardcap = 99;
        internal int _count = 0;
        internal int _maxItemCount = 1;
        internal bool _isInitialized = false;

        public InventoryItem(int count, int maxCount, Sprite uiSprite)
        {
            _UISprite = uiSprite;
            Count = count;
            MaxItemCount = maxCount;

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
    }
}
