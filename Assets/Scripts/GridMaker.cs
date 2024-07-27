using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridMaker : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField, Min(1)] int _Width = 1;
    [SerializeField, Min(1)] int _Height = 1;

    [Header("Reference")]
    [SerializeField, Tooltip("The plane must have 1 in scale to work properly.")] GameObject _TilePrefab;
    [SerializeField] LayerMask _TileLayer;
    [SerializeField] Transform _Parent;

    public void Generate()
    {
        if (_TilePrefab.TryGetComponent(out MeshFilter mf) == false) Debug.LogError($"Can not generate. There are no mesh filters in {_TilePrefab.name}");

        Transform trans = _Parent == null ? transform : _Parent;

        var tempList = trans.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            DestroyImmediate(child.gameObject);
        }

        Bounds bounds = mf.sharedMesh.bounds;

        Transform tempParent = new GameObject().transform;

        for (int y = 0; y < _Height; y++)
        {
            for (int x = 0; x < _Width; x++)
            {
                GameObject instGo = Instantiate(_TilePrefab, tempParent);
                Vector3 targetPos = new Vector3(((x + 1) * 2 - 1) * bounds.extents.x, -bounds.extents.y, ((y + 1) * 2 - 1) * bounds.extents.z);
                instGo.transform.position = targetPos;

                instGo.layer = (int)Mathf.Log(_TileLayer.value, 2);
                //Aptal unity layermask'� layere d�n��t�remiyo o y�zden elle yapt�m

                instGo.name = $"TDTile({x},{y})";
            }
        }

        tempParent.position = trans.position;
        tempParent.rotation = trans.rotation;

        tempList = tempParent.Cast<Transform>().ToList();
        foreach (var child in tempList)
        {
            child.parent = trans;
        }
        DestroyImmediate(tempParent.gameObject);
    }
}
