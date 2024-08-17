using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Overworld;

public class InventoryCellScript : MonoBehaviour
{
    public bool IsInitialized => _isInitialized;
    public int CellIndex = -1;
    public InventoryItem ItemData => _item;

    [SerializeField] Image _Image;
    [SerializeField] TextMeshProUGUI _CountTM;

    InventoryItem _item = null;
    bool _isInitialized = false;

    public void Initialize(InventoryItem item, int i)
    {
        _item = item;
        _Image.sprite = item.UISprite;
        _CountTM.text = item.Count.ToString();
        CellIndex = i;
        _isInitialized = true;
    }
}
