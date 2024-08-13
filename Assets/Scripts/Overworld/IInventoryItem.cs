using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Overworld
{
    public interface IInventoryItem
    {
        public int Count
        {
            get => Mathf.Clamp(AUTO_count, 0, MaxItemCount);
            set => AUTO_count = Mathf.Clamp(value, 0, MaxItemCount);
        }
        public int MaxItemCount
        {
            get => Mathf.Clamp(AUTO_maxItemCount, 1, AUTO_MAX_HARDCAP);
            set => AUTO_maxItemCount = Mathf.Clamp(value, 1, AUTO_MAX_HARDCAP);
        }
        public int RemainingSpace => AUTO_maxItemCount - AUTO_count;
        public bool IsInitialized => AUTO_isInitialized;
        public Image UIImage => _UIImage;

        Image _UIImage { get; set; }
        int AUTO_MAX_HARDCAP { get; }
        int AUTO_count { get; set; }
        int AUTO_maxItemCount { get; set; }
        bool AUTO_isInitialized { get; set; }

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
        public bool Compare(IInventoryItem otherItem);
    }
}
