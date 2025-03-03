using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resource/Resource Data")]
public class ResourceData : GameplayElementData<ResourceData>
{
    public int StackSize => _stackSize;
    public GameObject DroppedItem => _DroppedItem;

    [SerializeField] int _stackSize = 20;
    [SerializeField] GameObject _DroppedItem;

    public ResourceItem AsItem(int count = 0) => new ResourceItem(this, count);
}

[System.Serializable] public struct Cost
{
    public ResourceData Resource => _resource;
    public int Amount => _amount;

    [SerializeField] ResourceData _resource;
    [SerializeField] int _amount;

    public Cost(ResourceData resource, int amount)
    {
        _resource = resource;
        _amount = amount;
    }
}
