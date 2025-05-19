using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Core/Core Data")]
public class CoreData : GameplayElementData<CoreData>
{
    public int UsePerDay => _UsePerDay;

    [SerializeField] int _UsePerDay = 1;

    public CoreItem AsItem() => new CoreItem(this);
}
