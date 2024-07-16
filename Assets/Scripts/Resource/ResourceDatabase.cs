using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Resource/Resource Database")]
public class ResourceDatabase : ScriptableObject
{
    public List<ResourceData> Resources => _Resources;

    [SerializeField] List<ResourceData> _Resources = new List<ResourceData>();

    public ResourceData GetResourceByID(string id) => Resources.Find(x => x.ID == id);
    public ResourceData GetResourceByDisplayName(string displayName) => Resources.Find(x => x.DisplayName == displayName);
}
