using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _resourceText;

    Dictionary<ResourceData, int> _resourceList = new Dictionary<ResourceData, int>();

    void Awake()
    {
        GLOBAL.GetResourceDatabase().Resources.ForEach(x => _resourceList.TryAdd(x, 0));
        RefreshText();
    }

    void RefreshText()
    {
        _resourceText.text = "";
        foreach (KeyValuePair<ResourceData, int> pair in _resourceList)
        {
            _resourceText.text += $"<color=red>{pair.Value}<color=white>    {pair.Key.ID}\n";
        }
    }
}
