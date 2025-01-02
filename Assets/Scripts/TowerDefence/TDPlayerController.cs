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
        [SerializeField] TextMeshProUGUI _CurrentModeText;
        [SerializeField] TDCanvasManager _TDCanvasManager;
        [SerializeField] SpawnManager _SpawnManager;
        [SerializeField] Transform _MouseTracker;
        [SerializeField] OwnedTurretController _OwnedTurretController;
        [SerializeField] ManualTurretManager _ManualTurretManager;
        [SerializeField] ButtonToggleHelper _CraftTabToggler;
        [SerializeField] GraphicRaycasterScript _GraphicRaycasterScript;
        [SerializeField] TextMeshProUGUI _DescriptionTitle;
        [SerializeField] TextMeshProUGUI _DescriptionText;

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
        TurretData _currentTurret = null;
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
                    _ManualTurretManager.DeselectCurrentTurret();
                    break;
                case TowerDefenceGameplayMode.Edit:
                    _SpawnManager.SetCooldownIsPaused(false);
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
                    _SpawnManager.SetCooldownIsPaused(true);
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
            if (_currentTurret != null && _CraftTabToggler.Status == false)
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
            _currentTurret = null;
            DeselectCurrentTile();
        }

        void HandlePlayMode()
        {
            SetCurrentTile();

            if (Input.GetButtonDown("ToggleSelectRemoteTurret"))
            {
                Turret_Remote turret = _currentTile != null ? _currentTile.OccupyingTurret as Turret_Remote : null;
                EvaluateTurret(turret);
            }
            if (_selectedRemoteTurret != null)
            {
                _ManualTurretManager.DeselectCurrentTurret();
                if (Input.GetButton("UseSelectedRemoteTurret")) _selectedRemoteTurret.UseTurret();
            }
            else
            {
                _ManualTurretManager.SelectCurrentTurret(_MouseTracker);
                if (Input.GetButton("UseManualTurret")) _ManualTurretManager.UseCurrentTurret();
            }

            void EvaluateTurret(Turret_Remote turret)
            {
                if (turret == null)
                {
                    DeselectRemoteTurret();
                    return;
                }
                else if (_selectedRemoteTurret != null)
                {
                    if (_selectedRemoteTurret == turret) return;
                    DeselectRemoteTurret();
                }

                turret.SelectTurret(_MouseTracker);
                _selectedRemoteTurret = turret;
            }
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

            TurretUnit unit = Instantiate(_currentTurret.PrefabObject).GetComponent<TurretUnit>();
            unit.Initialize(_currentTurret, _currentTile);

            _OwnedTurretController.RemoveTurret(_currentTurret);
            if (_OwnedTurretController.HasTurret(_currentTurret) == false) DeselectCurrentTurret();
        }

        void DeleteTurret()
        {
            _currentTile?.OccupyingTurret?.KillSelf();
        }

        void SelectTile(TowerDefenceTileScript tdts)
        {
            _currentTile = tdts;
            _currentTile.GetHighlighted();
        }

        void DeselectCurrentTile()
        {
            if (_currentTile == null) return;

            _currentTile.GetUnhighlighted();
            _currentTile = null;
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
            _currentTurret = data;
        }

        bool IUICellOwner.CellIsValid(UICell cell)
        {
            var newCell = cell as OwnedTurretUIScript;
            if (newCell == null) return false;
            return newCell.TData != null;
        }
    }
}
