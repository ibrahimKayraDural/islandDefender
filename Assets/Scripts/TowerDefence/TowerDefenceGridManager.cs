using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TowerDefence
{
    public class TowerDefenceGridManager : MonoBehaviour
    {
        [Header("Variables")]
        [SerializeField, Min(1)] int _Width = 1;
        [SerializeField, Min(1)] int _Height = 1;
        [SerializeField] Material _FirstTileMaterial;
        [SerializeField] Material _SecondTileMaterial;
        [SerializeField, Min(0)] float _BoundingBoxHeight = .01f;
        [SerializeField] Vector3 _SpawnerOffset;

        [Header("Reference")]
        [SerializeField] SpawnManager _SpawnManager;
        [SerializeField, Tooltip("The plane must have 1 in scale to work properly.")] GameObject _TilePrefab;
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

            if (_SpawnManager == null) Debug.LogError("No SpawnManager was assigned");
            _SpawnManager?.DeleteSpawners();

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

            GameObject collisionPlane = new GameObject("collision plane", new System.Type[] { typeof(BoxCollider) });
            Transform cpTrans = collisionPlane.transform;
            BoxCollider cpCol = collisionPlane.GetComponent<BoxCollider>();
            cpTrans.parent = tempParent;
            cpTrans.localPosition = new Vector3(0, -.25f, 0);
            cpTrans.localRotation = Quaternion.identity;
            cpTrans.localScale = new Vector3(50, .1f, 50);
            cpCol.isTrigger = true;
            collisionPlane.layer = 13;

            for (int y = 0; y < _Height; y++)
            {
                for (int x = 0; x < _Width; x++)
                {
                    GameObject instGo = Instantiate(_TilePrefab, tempParent);
                    Vector3 targetPos = new Vector3(((x + 1) * 2 - 1) * bounds.extents.x, -bounds.extents.y, ((y + 1) * 2 - 1) * bounds.extents.z);
                    instGo.transform.position = targetPos;

                    if (instGo.TryGetComponent(out MeshRenderer mr))
                    { mr.sharedMaterial = (((y % 2) + (x % 2)) % 2) == 0 ? new Material(_FirstTileMaterial) : new Material(_SecondTileMaterial); }

                    instGo.layer = (int)Mathf.Log(_TileLayer.value, 2);
                    //Aptal unity layermask'ý layere dönüþtüremiyo o yüzden elle yaptým

                    instGo.name = $"TDTile({x},{y})";

                    if (y == _Height - 1)
                    {
                        targetPos.z += bounds.extents.z * 3;
                        _SpawnManager?.SpawnSpawnerAt(targetPos + _SpawnerOffset, tempParent);
                    }
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
