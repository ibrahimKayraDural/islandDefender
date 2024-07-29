using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    public class Inventory : MonoBehaviour
    {
        int SlotCount
        {
            get => _slotCount; 
            set
            {
                value = Mathf.Max(0, value);
                _slotCount = value;

                InventorySlot[] temp = _slots;
                _slots = new InventorySlot[_slotCount];

                for (int i = 0; i < temp.Length; i++)
                {
                    if(i < _slots.Length)
                    {
                        if (temp[i] != null || temp[i].IsInitialized)
                        {
                            _slots[i] = temp[i];
                        }
                    }
                    else
                    {
                        for (int n = 0; n < _slots.Length; n++)
                        {
                            if(_slots[n] == null || _slots[n].IsInitialized == false)
                            {
                                _slots[n] = temp[i];
                                goto Checkpoint1;
                            }
                        }
                        break;
                    }
                Checkpoint1:;
                }
            }
        }

        [SerializeField, Min(0)] int _MaxItemPerSlot = 20;
        [SerializeField,Min(0)] int _slotCount = 5;

        InventorySlot[] _slots;

        void Awake()
        {
            _slots = new InventorySlot[SlotCount];
        }

        //public bool TryAddResource(ResourceData resource)
        //{
        //    //check if resource exists
        //    //increase amount if it does, try to add it otherwise
        //    //return false if cant add it
        //}
    } 
}
