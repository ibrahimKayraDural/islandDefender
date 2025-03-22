using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using UnityEngine;

public class ActiveTurretManager : MonoBehaviour
{
    public static List<GameObject> ActiveTurrets = new();
    public static ActiveTurretManager Instance { get; private set; } = null;

    [SerializeField] Transform _UpgradeParent;
    [SerializeField] TurretUpgradeUIPiece _PiecePrefab;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    public void PlaceTurret(TurretData turret, TowerDefenceTileScript tile)
    {
        TurretUnit unit = Instantiate(turret.PrefabObject).GetComponent<TurretUnit>();
        unit.Initialize(turret, tile);

        var piece = Instantiate(_PiecePrefab.gameObject, _UpgradeParent).GetComponent<TurretUpgradeUIPiece>();
        piece.Initialize(unit);
    }
}
