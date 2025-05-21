using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GenericCell : MonoBehaviour
{
    [SerializeField] Button _Button;
    [SerializeField] Image _Icon;
    [SerializeField] TextMeshProUGUI _Name;
    [SerializeField] TextMeshProUGUI _Description;

    bool _isInitialized;

    public void Initialize(UnityAction OnButtonClicked, Sprite icon = null, string name = null, string description = null)
    {
        if (_isInitialized) return;

        if (icon && _Icon) _Icon.sprite = icon;
        if (name != null && _Name) _Name.text = name;
        if (description != null && _Description) _Description.text = description;

        _Button?.onClick.AddListener(OnButtonClicked);

        _isInitialized = true;
    }
    public void SetInteractablity(bool setTo)
    {
        _Button.interactable = setTo;
    }
}
