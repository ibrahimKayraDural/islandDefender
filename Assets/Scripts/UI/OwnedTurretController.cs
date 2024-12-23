using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using UnityEngine;

public class OwnedTurretController : MonoBehaviour
{
    const string OWNERID = "owned-turret-controller";

    [SerializeField] Transform _CellParent;
    [SerializeField] GameObject _CellPrefab;
    [SerializeField] int _MaxCap = 999;

    Dictionary<TurretData, int> _ownedTurrets = new Dictionary<TurretData, int>();

    BaseResourceController _BaseResCont
    {
        get
        {
            if (AUTO_baseResCont == null)
                AUTO_baseResCont = BaseResourceController.Instance;
            return AUTO_baseResCont;
        }
    }
    BaseResourceController AUTO_baseResCont = null;

    public bool TryBuyTurret(TurretData turret)
    {
        if (_BaseResCont.TryBuyTurret(turret) == false) return false;
        AddTurret(turret);
        return true;
    }
    public void AddTurret(TurretData turret)
    {
        if (turret == null)
        {
            Debug.LogError("Turret can not be null");
            return;
        }

        if (_ownedTurrets.ContainsKey(turret) == false) _ownedTurrets.Add(turret, 0);

        _ownedTurrets[turret] = Mathf.Min(_ownedTurrets[turret] + 1, _MaxCap);

        Refresh();
    }
    public void RemoveTurret(TurretData turret, int count = 1)
    {
        if (count < 1) return;
        if (turret == null) return;
        if (_ownedTurrets.ContainsKey(turret) == false) return;

        _ownedTurrets[turret] = _ownedTurrets[turret] - count;

        if (_ownedTurrets[turret] <= 0) _ownedTurrets.Remove(turret);
        Refresh();
    }
    void Refresh()
    {
        foreach (Transform child in _CellParent) Destroy(child.gameObject);
        foreach (var item in _ownedTurrets)
        {
            var otus = Instantiate(_CellPrefab, _CellParent).GetComponent<OwnedTurretUIScript>();
            otus.Initialize(item.Key, item.Value, OWNERID);
        }
    }
}
