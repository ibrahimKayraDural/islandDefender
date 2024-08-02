using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Data<T> : ScriptableObject
{
    public string DisplayName => _displayName;
    public string ID => _id;

    [SerializeField] internal string _displayName = GLOBAL.UnassignedString;
    [SerializeField] internal string _id = GLOBAL.UnassignedString;
}
