using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefence
{
    public enum ManualTurretStatus { Locked, Buyable, Unlocked }

    public class ManualTurretManager : MonoBehaviour
    {
        [System.Serializable]
        class ManualTurretData
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
        [SerializeField] ManualTurretUI _UICellPrefab;
        [SerializeField] Transform _UIParent;

        Turret_Manual _currentTurret = null;
        List<ManualTurretUI> _uiPieces = new List<ManualTurretUI>();


        void OnValidate()
        {
            for (int i = 0; i < _Datas.Count; i++)
            {
                ManualTurretData item = _Datas[i];

                if (item.Data == null) return;

                GameObject prefab = item.Data.PrefabObject;
                if (prefab == null || prefab.TryGetComponent(out Turret_Manual _) == false)
                {
                    _Datas[i] = new ManualTurretData(null, ManualTurretStatus.Locked);
                }
            }
        }
        void Awake()
        {
            //Put Datas array to order
            var templist = new List<ManualTurretData>();
            for (int i = 0; i < _Datas.Count; i++) if (_Datas[i].Data != null) templist.Add(_Datas[i]);
            _Datas = templist;

            //Clean UIParent's children
            var temp = _UIParent.Cast<Transform>();
            foreach (var child in temp) Destroy(child);

            //Instantiate and Initialize new UI pieces
            for (int i = 0; i < _Datas.Count; i++)
            {
                ManualTurretUI mtu = Instantiate(_UICellPrefab.gameObject, _UIParent).GetComponent<ManualTurretUI>();
                mtu.Initialize(_Datas[i].Data, i, this);
                _uiPieces.Add(mtu);
            }
        }
        void Start()
        {
            GM_OnLockedTurretListChanged(null, GameplayManager.Instance.UnlockedTurrets);

            GameplayManager.e_OnUnlockedTurretListChanged += GM_OnLockedTurretListChanged;
        }

        void GM_OnLockedTurretListChanged(object sender, List<TurretData> e)
        {
            foreach (var p in _uiPieces)
            {
                if (e.Contains(_Datas[p.Index].Data))
                {
                    if (_Datas[p.Index].Status == ManualTurretStatus.Locked)
                    {
                        _Datas[p.Index].SetStatus(ManualTurretStatus.Buyable);
                        p.SetStatus("buyable");
                    }
                }
                else
                {
                    _Datas[p.Index].SetStatus(ManualTurretStatus.Locked);
                    p.SetStatus("locked");
                }
            }
        }

        public void UseCurrentTurret() => _currentTurret?.UseTurret();
        public void SelectCurrentTurret(Transform lookTransform) => _currentTurret?.SelectTurret(lookTransform);
        public void DeselectCurrentTurret() => _currentTurret?.DeselectTurret();
        public bool TryBuyTurret(int index)
        {
            if (TryGetValidTurretByIndex(index, out TurretData data) == false) return false;
            if (_Datas[index].Status != ManualTurretStatus.Buyable) return false;
            if (BaseResourceController.Instance.TryBuyTurret(data) == false) return false;

            _Datas[index].SetStatus(ManualTurretStatus.Unlocked);
            return true;
        }
        public void PlaceTurret(int index)
        {
            if (TryGetValidTurretByIndex(index, out TurretData data) == false) return;
            if (_Datas[index].Status != ManualTurretStatus.Unlocked) return;

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
