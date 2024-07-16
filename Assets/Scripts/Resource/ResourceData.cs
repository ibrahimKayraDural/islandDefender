using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resource/Resource Data")]
public class ResourceData : ScriptableObject
{
    public string DisplayName => _displayName;
    public string ID => _id;

    [SerializeField] string _displayName = GLOBAL.UnassignedString;
    [SerializeField] string _id = GLOBAL.UnassignedString;
}
