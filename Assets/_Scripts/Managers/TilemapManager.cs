using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);
    }

    public bool DeleteTile(Vector3Int tilePos, string ID = null)
    {
        if(CheckTilemap(ID, out Tilemap map) == false) return false;

        map.SetTile(tilePos, null);
        return true;
    }
    public bool DeleteTile(Vector3 worldPos, string ID = null)
    {
        if (CheckTilemap(ID, out Tilemap map) == false) return false;

        map.SetTile(map.WorldToCell(worldPos), null);
        return true;
    }

    bool CheckTilemap(string ID, out Tilemap map)
    {
        map = null;

        if (_Tilemaps.Length <= 0) return false;

        if (ID == null) map = _Tilemaps[0].Tilemap;
        else map = GetTilemap(ID).Tilemap;

        return map != null;
    }

    public TilemapWithID GetTilemap(string id) => _Tilemaps.ToList().Find(x => x.ID.ToLower() == id.ToLower());
}
