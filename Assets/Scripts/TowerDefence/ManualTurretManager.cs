using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public enum ManualTurretStatus { Locked, Buyable, Unlocked }

    public class ManualTurretManager : MonoBehaviour
    {
        [System.Serializable]
        struct ManualTurretData
        {
            public TurretData Data => _data;
            public ManualTurretStatus Status => _status;

            [SerializeField] TurretData _data;
            ManualTurretStatus _status;

            public ManualTurretData(TurretData data, ManualTurretStatus status)
            {
                _data = data;
                _status = status;
            }

            public void SetStatus(ManualTurretStatus setTo) => _status = setTo;
        }

        [SerializeField] List<ManualTurretData> _Datas;
        [SerializeField] Transform _TurretParent;

        Turret_Manual _currentTurret = null;

        void OnValidate()
        {
            for (int i = 0; i < _Datas.Count; i++)
            {
                ManualTurretData item = _Datas[i];

                if (item.Data == null) return;

                GameObject prefab = item.Data.PrefabObject;
                if (prefab == null || prefab.TryGetComponent(out Turret_Manual _) == false)
                {
                    _Datas[i] = new ManualTurretData();
                }
            }
        }
        void Awake()
        {
            var templist = new List<ManualTurretData>();
            for (int i = 0; i < _Datas.Count; i++) if (_Datas[i].Data != null) templist.Add(_Datas[i]);
            _Datas = templist;

            //Set UI
        }

        public void UseCurrentTurret() => _currentTurret?.UseTurret();
        public void SelectCurrentTurret() => _currentTurret?.SelectTurret();
        public void DeselectCurrentTurret() => _currentTurret?.DeselectTurret();
        public bool TryBuyTurret(int index)
        {
            if (TryGetValidTurretByIndex(index, out TurretData _) == false) return false;

            ManualTurretData data = _Datas[index];
            if (data.Status != ManualTurretStatus.Buyable) return false;
            if (BaseResourceController.Instance.TryBuyTurret(data.Data) == false) return false;

            data.SetStatus(ManualTurretStatus.Unlocked);
            return true;
        }
        public void PlaceTurret(int index)
        {
            if (TryGetValidTurretByIndex(index, out TurretData data) == false) return;

            RemoveTurret();
            _currentTurret = Instantiate(data.PrefabObject).GetComponent<Turret_Manual>();
            _currentTurret.Initialize(data, _TurretParent);
        }
        public void RemoveTurret()
        {
            _currentTurret?.DeselectTurret();
            _currentTurret?.KillSelf();
            _currentTurret = null;
        }
        bool TryGetValidTurretByIndex(int index, out TurretData data)
        {
            data = null;
            if (index < 0 || index >= _Datas.Count) return false;
            ManualTurretData mData = _Datas[index];
            if (mData.Data == null) return false;

            data = mData.Data;
            return true;
        }
    }
}
