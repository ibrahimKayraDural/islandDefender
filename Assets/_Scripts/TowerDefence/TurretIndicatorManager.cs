using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Linq;

namespace TowerDefence
{
    public class TurretIndicatorManager : MonoBehaviour
    {
        [SerializeField, SerializedDictionary("Indicator", "Lock Status")]
        SerializedDictionary<TurretIndicator, bool> _Indicators;
        [SerializeField] Material _GhostMaterial;
        [SerializeField] Mesh _FailsafeMesh;

        public TurretIndicator GetFreeIndicator(out int index)
        {
            var ind = GetFree();
            if (ind == null) ind = MakeNew();

            index = _Indicators.ToList().FindIndex(x => x.Key == ind);
            _Indicators[ind] = true;
            return ind;
        }
        public void ReleaseIndicator(int index)
        {
            var ind = _Indicators.ToList()[index].Key;
            _Indicators[ind] = false;
            ind.SetTurret(null);
            ind.SetEnablity(false);
        }

        TurretIndicator GetFree() => _Indicators.ToList().Find(x => x.Value == false).Key;
        TurretIndicator MakeNew()
        {
            var ind = new GameObject("Turret Indicator", new System.Type[] { typeof(TurretIndicator) })
                .GetComponent<TurretIndicator>();

            ind.transform.parent = transform;
            ind.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

            ind.InitMeshes(_GhostMaterial, _FailsafeMesh);
            _Indicators.Add(ind, false);

            return ind;
        }
    }
}
