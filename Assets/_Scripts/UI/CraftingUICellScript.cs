using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefence;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUICellScript : MonoBehaviour
{
    public TurretData @TurretData => _turretData;

    [SerializeField] TextMeshProUGUI _TurretNameTM;
    [SerializeField] TextMeshProUGUI _TurretCosts;
    [SerializeField] Image TurretIcon;

    TurretCraftingUIController _owner;
    TurretData _turretData;
    bool _isInitialized;

    public void Initialize(TurretData turret, TurretCraftingUIController owner)
    {
        if (_isInitialized) return;

        _owner = owner;

        _turretData = turret;
        _TurretNameTM.text = turret.DisplayName;
        TurretIcon.sprite = turret.UISprite;
        _TurretCosts.text = "";
        foreach (var c in turret.Costs)
        {
            _TurretCosts.text += $"{c.Amount} {c.Resource.DisplayName}";
        }

        _isInitialized = true;
    }

    public void OnCraft()
    {
        _owner.Craft(TurretData);
    }
}
