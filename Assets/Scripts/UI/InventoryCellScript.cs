using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Overworld;
using GameUI;

public class InventoryCellScript : MonoBehaviour
{
    public string ID => _id;
    public IGridUI Owner => _owner;
    public bool IsInitialized => _isInitialized;
    public int CellIndex = -1;
    public InventoryItem ItemData => _item;

    [SerializeField] Image _ItemImage;
    [SerializeField] Image _BackgroundImage;
    [SerializeField] TextMeshProUGUI _CountTM;
    [SerializeField, Range(0, 1)] float _highlightAmount = .2f;

    InventoryItem _item = null;
    bool _isHighlighted = false;
    bool _isInitialized = false;
    float _defaultBGAlpha = 0;
    IGridUI _owner = null;
    string _id = GLOBAL.UnassignedString;

    public void Initialize(InventoryItem item, int i, IGridUI owner, string id = "")
    {
        _item = item;
        _ItemImage.sprite = item.UISprite;
        _CountTM.text = item.Count.ToString();
        CellIndex = i;
        _defaultBGAlpha = _BackgroundImage.color.a;
        _owner = owner;
        if (id != "") _id = id;
        _isInitialized = true;
    }

    public void SetHighlight(bool setTo)
    {
        if (_isHighlighted == setTo) return;

        Color color = _BackgroundImage.color;
        color.a = setTo ? _defaultBGAlpha + _highlightAmount : _defaultBGAlpha;
        _BackgroundImage.color = color;

        _isHighlighted = setTo;
    }
}
