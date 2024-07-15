using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TowerDefence
{
    public class GridManager : MonoBehaviour
    {
        [Header("Variables")]
        [SerializeField, Min(1)] int _Width = 1;
        [SerializeField, Min(1)] int _Height = 1;
        [SerializeField] Material _FirstTileMaterial;
        [SerializeField] Material _SecondTileMaterial;

        [Header("Reference")]
        [Tooltip("The plane must have 1 in scale to work properly.")]
        [SerializeField] GameObject _TilePrefab;
        [SerializeField] LayerMask _TileLayer;

        public void Generate()
        {
            if (_TilePrefab.TryGetComponent(out MeshFilter mf) == false) Debug.LogError($"Can not generate. There are no mesh filters in {_TilePrefab.name}");

            Bounds bounds = mf.sharedMesh.bounds;

            var tempList = transform.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                DestroyImmediate(child.gameObject);
            }

            Transform tempParent = new GameObject().transform;

            for (int y = 0; y < _Height; y++)
            {
                for (int x = 0; x < _Width; x++)
                {
                    GameObject instGo = Instantiate(_TilePrefab, tempParent);
                    instGo.transform.position = new Vector3(((x + 1) * 2 - 1) * bounds.extents.x, 0, ((y + 1) * 2 - 1) * bounds.extents.z);

                    if (instGo.TryGetComponent(out MeshRenderer mr))
                    { mr.sharedMaterial = (((y % 2) + (x % 2)) % 2) == 0 ? _FirstTileMaterial : _SecondTileMaterial; }

                    instGo.layer = (int)Mathf.Log(_TileLayer.value, 2);
                    //Aptal unity layermask'� layere d�n��t�remiyo o y�zden elle yapt�m
                }
            }

            tempParent.position = transform.position;
            tempParent.rotation = transform.rotation;

            Transform trans = transform;
            tempList = tempParent.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                child.parent = trans;
            }
            DestroyImmediate(tempParent.gameObject);
        }
    } 
}
