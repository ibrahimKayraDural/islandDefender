using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICell : MonoBehaviour
{
    public string DisplayName => _displayName;
    public string DisplayDescription => _displayDescription;
    public bool IsInitialized => _isInitialized;
    public string OwnerID => _ownerID;
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

    [SerializeField] internal Image _BackgroundImage;
    [SerializeField] internal Image _UIImage;

    [Header("Image Colors")]
    [SerializeField] internal Color DefaultItemImage;
    [SerializeField] internal Color DisabledItemImage;
    [Space(15)]
    [SerializeField] internal Color DefaultBG;
    [SerializeField] internal Color HighlightedBG;
    [SerializeField] internal Color DisabledBG;

    internal bool _isInteractable;
    internal bool _isEmpty = false;
    internal bool _isHighlighted = false;
    internal bool _isInitialized = false;
    internal string _ownerID = GLOBAL.UnassignedString;
    internal string _displayName = GLOBAL.UnassignedString;
    internal string _displayDescription = GLOBAL.UnassignedString;

    public virtual void Initialize(Sprite uiSprite, string displayName, string displayDescription, bool isInteractable = true, string ownerID = null)
    {
        _UIImage.sprite = uiSprite;
        _displayName = displayName;
        _displayDescription = displayDescription;
        IsInteractable = isInteractable;
        if (ownerID != null) _ownerID = ownerID;

        _isInitialized = true;
    }
    public virtual void Initialize()
    {
        _isEmpty = true;
        IsInteractable = false;

        _UIImage.color = new Color(1, 1, 1, 0);
        _BackgroundImage.color = DefaultBG;

        _isInitialized = true;
    }

    public virtual void SetHighlight(bool setTo)
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

    internal bool _forceSetHighlight = false;
    internal virtual void OnIsInteractableChanged()
    {
        if (_isEmpty) return;

        if (IsInteractable == false)
        {
            _forceSetHighlight = true;
            SetHighlight(false);
        }

        _UIImage.color = IsInteractable ? DefaultItemImage : DisabledItemImage;
        _BackgroundImage.color = IsInteractable ? DefaultBG : DisabledBG;
    }
}
