using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefence;
using UnityEngine;

public class OwnedTurretUIScript : UICell
{
    public TurretData TData => _turretData;

    [SerializeField] TextMeshProUGUI _CountText;

    TurretData _turretData;

    public void Initialize(TurretData data, int count, string ownerID = null)
    {
        _turretData = data;
        _CountText.text = count.ToString();
        base.Initialize(_turretData.UISprite, _turretData.DisplayName, _turretData.Description, true, ownerID);
    }
}
