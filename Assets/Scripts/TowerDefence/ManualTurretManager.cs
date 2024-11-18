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
            public ManualTurretData(TurretData data)
            {
                _data = data;
                _status = ManualTurretStatus.Locked;
            }

            public void SetStatus(ManualTurretStatus setTo) => _status = setTo;
        }

        [SerializeField] Transform _TurretParent;
        [SerializeField] ManualTurretUI _UICellPrefab;
        [SerializeField] Transform _UIParent;

        Turret_Manual _currentTurret = null;
        List<ManualTurretData> _datas = new List<ManualTurretData>();
        List<ManualTurretUI> _uiPieces = new List<ManualTurretUI>();

        void Awake()
        {
            //Populate _datas list
            GLOBAL.GetTurretDatabase().GetManualTurrets().ForEach(x => _datas.Add(new ManualTurretData(x)));

            //Clean UIParent's children
            foreach (var child in _UIParent.Cast<Transform>()) Destroy(child);

            //Instantiate and Initialize new UI pieces
            for (int i = 0; i < _datas.Count; i++)
            {
                ManualTurretUI mtu = Instantiate(_UICellPrefab.gameObject, _UIParent).GetComponent<ManualTurretUI>();
                mtu.Initialize(_datas[i].Data, i, this);
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
                if (e.Contains(_datas[p.Index].Data))
                {
                    if (_datas[p.Index].Status == ManualTurretStatus.Locked)
                    {
                        _datas[p.Index].SetStatus(ManualTurretStatus.Buyable);
                        p.SetStatus("buyable");
                    }
                }
                else
                {
                    _datas[p.Index].SetStatus(ManualTurretStatus.Locked);
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
            if (_datas[index].Status != ManualTurretStatus.Buyable) return false;
            if (BaseResourceController.Instance.TryBuyTurret(data) == false) return false;

            _datas[index].SetStatus(ManualTurretStatus.Unlocked);
            return true;
        }
        public void PlaceTurret(int index)
        {
            if (TryGetValidTurretByIndex(index, out TurretData data) == false) return;
            if (_datas[index].Status != ManualTurretStatus.Unlocked) return;

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
            if (index < 0 || index >= _datas.Count) return false;
            ManualTurretData mData = _datas[index];
            if (mData.Data == null) return false;

            data = mData.Data;
            return true;
        }
    }
}
