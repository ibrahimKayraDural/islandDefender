using CustomPointerEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefence;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUICellScript : MonoBehaviour
{
    public TurretData @TurretData => _turretData;

    [SerializeField] Image _TurretIcon;
    [SerializeField] Transform _MiddlePosTransform;

    TurretCraftingUIController _owner;
    TurretData _turretData;
    bool _isInitialized;

    public void Initialize(TurretData turret, TurretCraftingUIController owner)
    {
        if (_isInitialized) return;

        _owner = owner;
        _turretData = turret;
        _TurretIcon.sprite = turret.UISprite;

        _isInitialized = true;
    }

    public void OnCraft()
    {
        _owner.Craft(TurretData);
    }

    public void SendDataToBar() => _owner.SendDataToBar(this);

    public void SetBarEnablity(bool setTo) => _owner.SetBarEnablity(setTo);
    public Vector3 GetMiddlePos() => _MiddlePosTransform.transform.position;
}
