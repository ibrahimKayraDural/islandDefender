using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolCellUI : MonoBehaviour
{
    public bool IsInitialized => _isInitialized;
    public ToolData CellData => _data;

    [SerializeField] Image _BackgroundImage;
    [SerializeField] Image _Image;
    [SerializeField, Range(0, 1)] float _highlightAmount = .2f;

    ToolData _data;

    bool _isHighlighted = false;
    bool _isInitialized = false;
    float _defaultBGAlpha = 0;

    public void Initialize(ToolData data)
    {
        _data = data;
        _Image.sprite = _data.UISprite;
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
