using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace TowerDefence
{
    public class TurretIndicator : MonoBehaviour
    {
        [TextArea]
        [SerializeField]
        string IMPORTANT = "Right click this script and click " +
            "''Initialize Meshes'' to initialize the meshes";

        [Space(15), SerializeField] Transform _Indicator;
        [SerializeField] Material _GhostMaterial;
        [SerializeField] Mesh _FailsafeMesh;
        [ContextMenu("Initialize Meshes")] void InitMeshesGetter() => InitMeshes();

        [SerializeField, SerializedDictionary("Turret", "Object")]
        SerializedDictionary<TurretData, GameObject> Meshes = new();

        readonly List<Type> IndicatorComponentTypes = new()
        { typeof(Transform), typeof(MeshRenderer), typeof(MeshFilter) };

        GameObject _lastTurret = null;
        GameObject _failsafeGO;

        TurretDatabase _turretDB
        {
            get
            {
                if (AUTO_turretDB == null)
                    AUTO_turretDB = GLOBAL.GetTurretDatabase();

                return AUTO_turretDB;
            }
        }
        TurretDatabase AUTO_turretDB = null;

        public void InitMeshes(Material ghostMaterial = null, Mesh failsafeMesh = null)
        {
            //clear
            Meshes = new SerializedDictionary<TurretData, GameObject>();

            if (_Indicator == null)
            {
                _Indicator = new GameObject("Indicator Parent").transform;
                _Indicator.parent = transform;
                _Indicator.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
            }

            if (ghostMaterial) _GhostMaterial = ghostMaterial;
            if (failsafeMesh) _FailsafeMesh = failsafeMesh;

            var children = _Indicator.transform.Cast<Transform>().ToList();
            foreach (var child in children)
            {
#if UNITY_EDITOR
                DestroyImmediate(child.gameObject);
#else
                Destroy(child.gameObject);
#endif
            }

            //add failsafe gameObject
            var temp = new GameObject("Turret Indicator Failsafe GO", typeof(MeshRenderer), typeof(MeshFilter));
            temp.transform.SetParent(_Indicator);
            temp.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            temp.SetActive(false);
            temp.GetComponent<MeshRenderer>().sharedMaterials = new[] { _GhostMaterial };
            temp.GetComponent<MeshFilter>().sharedMesh = _FailsafeMesh;
            _failsafeGO = temp;
            _failsafeGO.layer = LayerMask.NameToLayer("TowerDefenceVisual");

            //re-initialize
            foreach (var type in _turretDB.DataList)
            {
                var go = type.PrefabObject;
                go = Instantiate(go, _Indicator);
                go.SetActive(false);
                go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                var comps = go.GetComponentsInChildren<Component>();
                for (int i = 0; i < comps.Length; i++)
                {
                    var c = comps[i];
                    var cType = c.GetType();
                    if (IndicatorComponentTypes.Contains(cType) == false)
                    {
#if UNITY_EDITOR
                        DestroyImmediate(c);
#else
                        Destroy(c);
#endif
                    }
                    else if (cType == typeof(MeshRenderer))
                    {
                        var mr = c as MeshRenderer;
                        var ghostM = new Material[mr.sharedMaterials.Length];
                        for (int n = 0; n < ghostM.Length; n++) ghostM[n] = _GhostMaterial;
                        mr.sharedMaterials = ghostM;
                    }
                }
                Meshes.Add(type, go);
            }
        }
        public void SetPosition(Vector3 position) => _Indicator.position = position;
        public void SetEnablity(bool setTo) => _Indicator.gameObject.SetActive(setTo);
        public void SetTurret(TurretData turret)
        {
            GameObject go = null;
            if (_lastTurret) _lastTurret.SetActive(false);
            if (turret)
            {
                go = Meshes.ContainsKey(turret) ? Meshes[turret] : _failsafeGO;
                go.SetActive(true);
            }
            _lastTurret = go;
        }
    }
}
