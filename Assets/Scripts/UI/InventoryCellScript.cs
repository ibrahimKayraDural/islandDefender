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

    [SerializeField] Image _ItemImage;
    [SerializeField] Image _BackgroundImage;
    [SerializeField] TextMeshProUGUI _CountTM;
    [SerializeField, Range(0, 1)] float _highlightAmount = .2f;

    InventoryItem _item = null;
    bool _isHighlighted = false;
    bool _isInitialized = false;
    float _defaultBGAlpha = 0;

    public void Initialize(InventoryItem item, int i)
    {
        _item = item;
        _ItemImage.sprite = item.UISprite;
        _CountTM.text = item.Count.ToString();
        CellIndex = i;
        _defaultBGAlpha = _BackgroundImage.color.a;
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
