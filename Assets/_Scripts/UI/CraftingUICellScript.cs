using CustomPointerEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefence;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingUICellScript : MonoBehaviour
{
    public TurretData @TurretData => _turretData;

    [SerializeField] Image _TurretIcon;
    [SerializeField] Image _Background;
    [SerializeField] Transform _MiddlePosTransform;
    [SerializeField] Color _NormalColor = new Color(.8f, .8f, .8f, 1);
    [SerializeField] Color _HighlightedColor = new Color(1, 1, 1, 1);
    [SerializeField] OnHoverOver _OnHoverOver;
    [SerializeField] EventTrigger _EventTrigger;
    [SerializeField] GameObject _LockImage;

    TurretCraftingUIController _owner;
    TurretData _turretData;
    bool _isInitialized;

    public void Initialize(TurretData turret, TurretCraftingUIController owner, bool isLocked)
    {
        if (_isInitialized) return;

        _owner = owner;
        _turretData = turret;
        _TurretIcon.sprite = turret.UISprite;
        _Background.color = _NormalColor;

        _OnHoverOver.enabled = !isLocked;
        _EventTrigger.enabled = !isLocked;
        _LockImage.SetActive(isLocked);

        _isInitialized = true;
    }

    public void OnCraft()
    {
        _owner.Craft(TurretData);
    }

    public void SendDataToBar() => _owner.SendDataToBar(this);

    public void SetBarEnablity(bool setTo) => _owner.SetBarEnablity(setTo);
    public Vector3 GetMiddlePos() => _MiddlePosTransform.transform.position;
    public void SetHighlight(bool setTo) => _Background.color = setTo ? _HighlightedColor : _NormalColor;
}
