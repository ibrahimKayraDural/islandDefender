using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUI
{
    public class ResourceCell : InventoryCellScript
    {
        public ResourceData Data => _data;
        public int Amount => _amount;

        ResourceData _data = null;
        int _amount = 0;

        public virtual void Initialize(ResourceData data, int amount, int i, bool isInteractable = true, string ownerID = null)
        {
            _data = data;
            _amount = amount;

            Initialize(_data.AsItem(), i, IsInteractable, ownerID);
        }
    } 
}
