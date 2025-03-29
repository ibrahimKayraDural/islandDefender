using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using SaveSystem;
using TowerDefence;

[System.Serializable]
public struct TilemapWithID
{
    public Tilemap @Tilemap => _Tilemap;
    public string ID => _ID;

    [SerializeField] Tilemap _Tilemap;
    [SerializeField] string _ID;
}

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager Instance { get; private set; } = null;

    [SerializeField] TilemapWithID[] _Tilemaps;
    [SerializeField] string _GUID = GLOBAL.UnassignedString;

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

    List<DeletedTileData> _deletedTiles = new();
    bool _isSaved;//prevents race conditions

    void Start()
    {
        LoadGrid();
    }
    void OnApplicationQuit()
    {
        SaveGrid();
        _isSaved = true;
    }
    void OnDestroy()
    {
        if (_isSaved == false) SaveGrid();
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }
    void Reset()
    {
        GenerateGUID();
    }

    public bool TryDeleteTile(Vector3Int tilePos, string ID = null)
    {
        if (CheckTilemap(ID, out Tilemap map) == false) return false;

        DeleteTile(map, ID, tilePos);
        return true;
    }
    public bool TryDeleteTile(Vector3 worldPos, string ID = null)
    {
        if (CheckTilemap(ID, out Tilemap map) == false) return false;

        DeleteTile(map, ID, map.WorldToCell(worldPos));
        return true;
    }
    public TilemapWithID GetTilemap(string id) => _Tilemaps.ToList().Find(x => x.ID.ToLower() == id.ToLower());

    void DeleteTile(Tilemap map, string id, Vector3Int tilePos)
    {
        map.SetTile(tilePos, null);
        _deletedTiles.Add(new(id, tilePos));
    }
    bool CheckTilemap(string ID, out Tilemap map)
    {
        map = null;

        if (_Tilemaps.Length <= 0) return false;

        if (ID == null) map = _Tilemaps[0].Tilemap;
        else map = GetTilemap(ID).Tilemap;

        return map != null;
    }

    [ContextMenu("Generate GUID")]
    void GenerateGUID()
    {
        _GUID = Guid.NewGuid().ToString();
    }

    [ContextMenu("Save Grid")]
    void SaveGrid()
    {
        if (_deletedTiles == null || _deletedTiles.Count == 0) return;
        if(_GUID == GLOBAL.UnassignedString)
        {
            Debug.LogError($"GUID of {gameObject.name} is not assigned. " +
                $"Assign it by right clicking and selecting ''Generate GUID'' on TilemapManager script.");
            return;
        }

        _SaveManager.AddOrReplaceGrid(_GUID, _deletedTiles);
    }

    [ContextMenu("Load Grid")]
    public void LoadGrid()
    {
        if (_SaveManager == null) return;

        var save = _SaveManager.CurrentSave;
        if (save == null) return;

        var grid = save.Grids;
        if (grid == null) return;
        if (grid.Count == 0) return;

        var gridSD = grid.Find(x => x.TileManagerID == _GUID);
        if (gridSD == null) return;
        if (gridSD.DeletedTiles == null) return;
        if (gridSD.DeletedTiles.Count == 0) return;

        foreach (var tileToDelete in gridSD.DeletedTiles)
        {
            TryDeleteTile(tileToDelete.TileCellPosition, tileToDelete.TilemapID);
        }
    }
}
