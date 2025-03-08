using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace TowerDefence
{
    public enum TowerDefenceControlMode { None, Remote, Full }
    public enum TowerDefenceGameplayMode { Idle, Play, Edit }
    public class TDPlayerController : MonoBehaviour, IUICellOwner
    {
        [SerializeField] LayerMask TowerDefenceLayer;
        [SerializeField] Transform _MouseTracker;
        [SerializeField] TurretIndicator _Indicator;
        [SerializeField] TDCanvasManager _TDCanvasManager;
        [SerializeField] SpawnManager _SpawnManager;
        [SerializeField] OwnedTurretController _OwnedTurretController;
        //[SerializeField] ManualTurretManager _ManualTurretManager;
        [SerializeField] ButtonToggleHelper _CraftTabToggler;
        [SerializeField] GraphicRaycasterScript _GraphicRaycasterScript;
        [SerializeField] TextMeshProUGUI _DescriptionTitle;
        [SerializeField] TextMeshProUGUI _DescriptionText;
        [SerializeField] TextMeshProUGUI _CurrentModeText;

        [SerializeField] Camera _camera = null;

        BattleManager _battleManager
        {
            get
            {
                if (AUTO_battleManager == null)
                    AUTO_battleManager = BattleManager.Instance;
                return AUTO_battleManager;
            }
        }

        public UICell OldCell { get; set; }
        public UICell CurrentCell { get; set; }

        GraphicRaycasterScript IUICellOwner.GraphicRaycasterS => _GraphicRaycasterScript;

        TextMeshProUGUI IUICellOwner.DescriptionTitle => _DescriptionTitle;

        TextMeshProUGUI IUICellOwner.DescriptionText => _DescriptionText;

        BattleManager AUTO_battleManager = null;

        float targetTime_CanPlaceTurret = -1;
        TurretUnit _turretToSwap = null;
        TurretData _currentTurretToPlace = null;
        TowerDefenceTileScript _currentTile = null;
        Turret_Remote _selectedRemoteTurret = null;

        TowerDefenceGameplayMode _currentGameplayMode = TowerDefenceGameplayMode.Edit;
        TowerDefenceControlMode _currentControlMode = TowerDefenceControlMode.None;

        void Awake()
        {
            if (_camera == null)
            {
                Debug.LogError("No camera was found");
                this.enabled = false;
            }
        }

        void Update()
        {
            if (_currentControlMode == TowerDefenceControlMode.None) return;

            switch (_currentGameplayMode)
            {
                case TowerDefenceGameplayMode.Play: HandlePlayMode(); break;
                case TowerDefenceGameplayMode.Edit: HandleEditMode(); break;
                case TowerDefenceGameplayMode.Idle: HandleIdleMode(); break;
            }

            if (Input.GetButtonDown("Exit")) _battleManager.ExitBattle();

            _CurrentModeText.text = _currentGameplayMode.ToString();
        }

        public void EvaluateGameplayMode(bool WaveIsActive)
        {
            if (WaveIsActive == false)
            {
                switch (_currentControlMode)
                {
                    case TowerDefenceControlMode.None:
                    case TowerDefenceControlMode.Remote: SetGameplayMode(TowerDefenceGameplayMode.Idle); break;

                    case TowerDefenceControlMode.Full: SetGameplayMode(TowerDefenceGameplayMode.Edit); break;
                }
            }
            else
            {
                switch (_currentControlMode)
                {
                    case TowerDefenceControlMode.None: SetGameplayMode(TowerDefenceGameplayMode.Idle); break;

                    case TowerDefenceControlMode.Remote:
                    case TowerDefenceControlMode.Full: SetGameplayMode(TowerDefenceGameplayMode.Play); break;
                }
            }
        }

        void SetGameplayMode(TowerDefenceGameplayMode setTo)
        {
            TowerDefenceGameplayMode oldMode = _currentGameplayMode;
            _currentGameplayMode = setTo;

            //Run exit behaviour of mode
            switch (oldMode)
            {
                case TowerDefenceGameplayMode.Play:
                    DeselectRemoteTurret();
                    //_ManualTurretManager.DeselectCurrentTurret();
                    break;
                case TowerDefenceGameplayMode.Edit:
                    (this as IUICellOwner).OnEnd();
                    DeselectCurrentTurret();
                    _CraftTabToggler.SetStatus(false);
                    break;
                case TowerDefenceGameplayMode.Idle:
                    break;
            }

            //Run enter behaviour of mode
            switch (_currentGameplayMode)
            {
                case TowerDefenceGameplayMode.Play:
                    break;
                case TowerDefenceGameplayMode.Edit:
                    (this as IUICellOwner).OnStart();
                    break;
                case TowerDefenceGameplayMode.Idle:
                    break;
            }

            if (_TDCanvasManager.gameObject.activeInHierarchy == true)
                _TDCanvasManager.SetCanvas(_currentGameplayMode);
        }

        public void EnterBattle(TowerDefenceControlMode mode)
        {
            _currentControlMode = mode;
            EvaluateGameplayMode(SpawnManager.WaveIsActive);
            _TDCanvasManager.gameObject.SetActive(true);
            _TDCanvasManager.SetCanvas(_currentGameplayMode);
        }
        public void ExitBattle()
        {
            _currentControlMode = TowerDefenceControlMode.None;
            DeselectCurrentTurret();
            EvaluateGameplayMode(SpawnManager.WaveIsActive);
            _TDCanvasManager.gameObject.SetActive(false);
        }

        void HandleEditMode()
        {
            if (_currentTurretToPlace != null && _CraftTabToggler.Status == false)
            {
                SetCurrentTile();
            }

            (this as IUICellOwner).OnLoop();

            if (Input.GetButtonDown("CraftingTabOpen"))
            {
                DeselectCurrentTurret();
                _CraftTabToggler.Toggle();
            }

            if (Input.GetButtonDown("PlaceTurret")) TryPlaceTurret();
            else if (Input.GetMouseButtonDown(1)) DeselectCurrentTurret();
        }

        void DeselectCurrentTurret()
        {
            _currentTurretToPlace = null;
            _Indicator.SetTurret(null);
            DeselectCurrentTile();
        }

        void HandlePlayMode()
        {
            SetCurrentTile();

            if (Input.GetMouseButtonDown(0))
            {
                if (_turretToSwap)
                {
                    if (_currentTile)
                    {
                        SwapTiles(_currentTile, _turretToSwap._parentTile);
                        DeselectTurretToSwap();
                    }
                }
                else if (_selectedRemoteTurret)
                {
                    if (_currentTile != null)
                    {
                        _selectedRemoteTurret.UseTurret();
                        DeselectRemoteTurret();
                    }
                }
                else
                {
                    SelectTurretToSwap(_currentTile?.OccupyingTurret);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (_turretToSwap)
                {
                    DeselectTurretToSwap();
                }
                else if (_selectedRemoteTurret)
                {
                    DeselectRemoteTurret();
                }
                else
                {
                    Turret_Remote turret = _currentTile?.OccupyingTurret as Turret_Remote;

                    EvaluateRemoteTurret(turret);
                }
            }

            void EvaluateRemoteTurret(Turret_Remote turret)
            {
                if (turret && turret.IsUsable)
                {
                    if (_selectedRemoteTurret != null)
                    {
                        if (_selectedRemoteTurret == turret) return;
                        DeselectRemoteTurret();
                    }

                    turret.SelectTurret(_MouseTracker);
                    _selectedRemoteTurret = turret;
                }
            }
        }

        private void SelectTurretToSwap(TurretUnit turret)
        {
            if (_turretToSwap != null) DeselectTurretToSwap();

            _turretToSwap = turret;
        }

        private void DeselectTurretToSwap()
        {
            _turretToSwap = null;
        }

        void DeselectRemoteTurret()
        {
            if (_selectedRemoteTurret == null) return;

            _selectedRemoteTurret.DeselectTurret();
            _selectedRemoteTurret = null;
        }

        void HandleIdleMode()
        {

        }

        void SetCurrentTile()
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out RaycastHit hit, 100, TowerDefenceLayer);

            GameObject hitGo = hit.transform?.gameObject;
            if (hitGo == null || hitGo.TryGetComponent(out TowerDefenceTileScript tdts) == false)
            {
                DeselectCurrentTile();
            }
            else
            {
                if (_currentTile != null && tdts != _currentTile)
                {
                    DeselectCurrentTile();
                }

                if (_currentTile == null)
                {
                    SelectTile(tdts);
                }
            }

            _MouseTracker.position = hit.point;
        }

        void TryPlaceTurret()
        {
            if (targetTime_CanPlaceTurret > Time.time) return;
            if (_currentTile == null) {/*DeselectCurrentTurret();*/ return; }
            if (_currentTile.IsOccupied) return;

            TurretUnit unit = Instantiate(_currentTurretToPlace.PrefabObject).GetComponent<TurretUnit>();
            unit.Initialize(_currentTurretToPlace, _currentTile);

            _OwnedTurretController.RemoveTurret(_currentTurretToPlace);
            if (_OwnedTurretController.HasTurret(_currentTurretToPlace) == false) DeselectCurrentTurret();
        }

        void DeleteTurret()
        {
            _currentTile?.OccupyingTurret?.KillSelf();
        }

        void SelectTile(TowerDefenceTileScript tdts)
        {
            _currentTile = tdts;
            _currentTile.GetHighlighted();

            if (tdts.IsOccupied == false)
            {
                _Indicator.SetPosition(tdts.transform.position);
                _Indicator.SetEnablity(true);
            }
        }
        void SwapTiles(TowerDefenceTileScript tile1, TowerDefenceTileScript tile2)
        {
            if (tile1 == null || tile2 == null) return;

            var turret1 = tile1.OccupyingTurret;
            var turret2 = tile2.OccupyingTurret;

            var data1 = turret1?.Data;
            var data2 = turret2?.Data;

            turret1?.KillSelf(false);
            turret2?.KillSelf(false);

            if (data1)
            {
                TurretUnit unit = Instantiate(data1.PrefabObject).GetComponent<TurretUnit>();
                unit.Initialize(data1, tile2);
            }
            if (data2)
            {
                TurretUnit unit = Instantiate(data2.PrefabObject).GetComponent<TurretUnit>();
                unit.Initialize(data2, tile1);
            }
        }

        void DeselectCurrentTile()
        {
            if (_currentTile == null) return;

            _currentTile.GetUnhighlighted();
            _currentTile = null;

            _Indicator.SetEnablity(false);
        }

        public void OnHoverInteractableCell(UICell currentCell) { }

        void IUICellOwner.OnCellClicked(UICell cell)
        {
            var tCell = cell as OwnedTurretUIScript;
            if (tCell == null) return;
            TurretData data = tCell.TData;
            if (data == null) return;

            targetTime_CanPlaceTurret = Time.time + .1f;

            _CraftTabToggler.SetStatus(false);
            _currentTurretToPlace = data;
            _Indicator.SetTurret(data);
        }

        bool IUICellOwner.CellIsValid(UICell cell)
        {
            var newCell = cell as OwnedTurretUIScript;
            if (newCell == null) return false;
            return newCell.TData != null;
        }
    }
}
