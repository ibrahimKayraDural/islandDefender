using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Overworld;

public class InventoryCellScript : MonoBehaviour
{
    public InventoryItem ItemData => _item;

    [SerializeField] Image _Image;
    [SerializeField] TextMeshProUGUI _CountTM;

    InventoryItem _item = null;

    public void Initialize(InventoryItem item)
    {
        _item = item;
        _Image.sprite = item.UISprite;
        _CountTM.text = item.Count.ToString();
    }
}
