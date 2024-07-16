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
        [SerializeField, Min(0)] float _BoundingBoxHeight = 1;

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

            GameObject boundingBox = new GameObject("bounding box", new System.Type[] { typeof(BoxCollider) });
            Transform bbTrans = boundingBox.transform;
            bbTrans.parent = tempParent;
            bbTrans.localPosition = new Vector3(_Width * bounds.extents.x, _BoundingBoxHeight / 2, _Height * bounds.extents.z);
            bbTrans.localRotation = Quaternion.identity;
            Vector3 targetBBScale = Vector3.Scale(bounds.size, new Vector3(_Width, 1, _Height));
            targetBBScale.y = _BoundingBoxHeight;
            bbTrans.localScale = targetBBScale;
            boundingBox.GetComponent<BoxCollider>().isTrigger = true;
            boundingBox.tag = "TowerDefencePlayground";

            for (int y = 0; y < _Height; y++)
            {
                for (int x = 0; x < _Width; x++)
                {
                    GameObject instGo = Instantiate(_TilePrefab, tempParent);
                    instGo.transform.position = new Vector3(((x + 1) * 2 - 1) * bounds.extents.x, 0, ((y + 1) * 2 - 1) * bounds.extents.z);

                    if (instGo.TryGetComponent(out MeshRenderer mr))
                    { mr.sharedMaterial = (((y % 2) + (x % 2)) % 2) == 0 ? new Material(_FirstTileMaterial) : new Material(_SecondTileMaterial); }

                    instGo.layer = (int)Mathf.Log(_TileLayer.value, 2);
                    //Aptal unity layermask'ý layere dönüþtüremiyo o yüzden elle yaptým
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
