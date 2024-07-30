using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resource/Resource Data")]
public class ResourceData : ScriptableObject
{
    public string DisplayName => _displayName;
    public string ID => _id;
    public int StackSize => _stackSize;

    [SerializeField] string _displayName = GLOBAL.UnassignedString;
    [SerializeField] string _id = GLOBAL.UnassignedString;
    [SerializeField] int _stackSize = 20;

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
