using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UICostItem : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _amountText;

    public void Initialize(Sprite icon, int amount)
    {
        if (_icon != null)
            _icon.sprite = icon;

        if (_amountText != null)
            _amountText.text = $"x {amount}";
    }
}