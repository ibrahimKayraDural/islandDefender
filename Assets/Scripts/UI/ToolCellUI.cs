using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolCellUI : MonoBehaviour
{
    public bool IsInitialized => _isInitialized;
    public string OwnerID => _ownerID;
    public ToolData CellData => _data;
    public bool IsInteractable
    {
        get => _isInteractable;
        set
        {
            if (_isEmpty) return;

            _isInteractable = value;
            OnIsInteractableChanged();
        }
    }

    [SerializeField] Image _BackgroundImage;
    [SerializeField] Image _ToolImage;

    [Header("Image Colors")]
    [SerializeField] Color DefaultItemImage;
    [SerializeField] Color DisabledItemImage;
    [Space(15)]
    [SerializeField] Color DefaultBG;
    [SerializeField] Color HighlightedBG;
    [SerializeField] Color DisabledBG;

    ToolData _data;

    bool _isInteractable;
    bool _isEmpty = false;
    bool _isHighlighted = false;
    bool _isInitialized = false;
    string _ownerID = GLOBAL.UnassignedString;

    public void Initialize(ToolData data, bool isInteractable = true, string ownerID = null)
    {
        _data = data;
        _ToolImage.sprite = _data.UISprite;
        IsInteractable = isInteractable;
        if (ownerID != null) _ownerID = ownerID;

        _isInitialized = true;
    }
    public void Initialize()
    {
        _data = null;
        _isEmpty = true;
        IsInteractable = false;

        _ToolImage.color = new Color(1, 1, 1, 0);
        _BackgroundImage.color = DefaultBG;

        _isInitialized = true;
    }

    public void SetHighlight(bool setTo)
    {
        if (_forceSetHighlight == false)
        {
            if (_isHighlighted == setTo) return;
            if (_isInteractable == false) return;
            if (_isEmpty) return;
        }

        _BackgroundImage.color = setTo ? HighlightedBG : DefaultBG;

        _isHighlighted = setTo;

        _forceSetHighlight = false;
    }

    bool _forceSetHighlight = false;
    void OnIsInteractableChanged()
    {
        if (_isEmpty) return;

        if(IsInteractable == false)
        {
            _forceSetHighlight = true;
            SetHighlight(false);
        }

        _ToolImage.color = IsInteractable ? DefaultItemImage : DisabledItemImage;
        _BackgroundImage.color = IsInteractable ? DefaultBG : DisabledBG;
    }
}
