using Overworld;
using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefence;
using UnityEngine;
using static SaveSystem.SaveManager;

public class ActiveTurretManager : MonoBehaviour
{
    public static List<GameObject> ActiveTurrets = new();
    public static ActiveTurretManager Instance { get; private set; } = null;

    [SerializeField] Transform _UpgradeParent;
    [SerializeField] TurretUpgradeUIPiece _PiecePrefab;
    [SerializeField] TowerDefenceGridManager _GridManager;

    SaveManager _SaveManager
    {
        get
        {
            if (AUTO_saveManager == null)
                AUTO_saveManager = SaveManager.Instance;

            return AUTO_saveManager;
        }
    }
    SaveManager AUTO_saveManager = null;

    bool _isSaved;//prevents race conditions

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }
    void Start()
    {
        LoadTurrets();
    }
    void OnApplicationQuit()
    {
        SaveTurrets();
        _isSaved = true;
    }
    void OnDestroy()
    {
        if (_isSaved == false) SaveTurrets();
    }

    [ContextMenu("Save Turrets")]
    void SaveTurrets()
    {
        List<TurretSaveData> save = new();

        foreach (var item in ActiveTurrets)
        {
            if (item.TryGetComponent(out TurretUnit tu) == false) continue;

            string id = tu.Data.ID;
            var pos = tu._parentTile.TilePosition;

            save.Add(new TurretSaveData(id, pos.x, pos.y));
        }

        _SaveManager.ReplaceTurrets(save);
    }

    [ContextMenu("Load Turrets")]
    public void LoadTurrets()
    {
        if (_SaveManager == null) return;

        var save = _SaveManager.CurrentSave;
        if (save == null) return;

        List<TurretSaveData> turrets = save.Turrets;
        if (turrets == null) return;
        if (turrets.Count == 0) return;


        var turretDB = GLOBAL.GetTurretDatabase();

        for (int i = 0; i < turrets.Count; i++)
        {
            var item = turrets[i];

            if (item == null) continue;

            if (item.PosX >= 0 && item.PosY >= 0 && item.ID != null)
            {
                var data = turretDB.GetDataByDisplayNameOrID(item.ID);
                var tile = _GridManager.GetTile(item.PosX, item.PosY);
                PlaceTurret(data, tile);
            }
        }
    }

    public void PlaceTurret(TurretData turret, TowerDefenceTileScript tile)
    {
        TurretUnit unit = Instantiate(turret.PrefabObject).GetComponent<TurretUnit>();
        unit.Initialize(turret, tile);

        var piece = Instantiate(_PiecePrefab.gameObject, _UpgradeParent).GetComponent<TurretUpgradeUIPiece>();
        piece.Initialize(unit);
    }
}
