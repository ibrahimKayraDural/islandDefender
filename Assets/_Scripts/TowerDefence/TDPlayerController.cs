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
    public class TDPlayerController : MonoBehaviour/*, IUICellOwner*/
    {
        public static TDPlayerController Instance { get; private set; } = null;

        public Transform MouseTracker => _MouseTracker;

        [SerializeField] LayerMask TowerDefenceLayer;
        [SerializeField] Transform _MouseTracker;
        [SerializeField] TurretIndicator _CursorIndicator;
        [SerializeField] TurretIndicatorManager _IndicatorManager;
        [SerializeField] TDCanvasManager _TDCanvasManager;
        [SerializeField] SpawnManager _SpawnManager;
        [SerializeField] ButtonToggleHelper _CraftTabToggler;

        //[SerializeField] ManualTurretManager _ManualTurretManager;
        /*[SerializeField] GraphicRaycasterScript _GraphicRaycasterScript;
          [SerializeField] TextMeshProUGUI _DescriptionTitle;
          [SerializeField] TextMeshProUGUI _DescriptionText;*/

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
        BattleManager AUTO_battleManager = null;

        ActiveTurretManager _activeTurretManager
        {
            get
            {
                if (AUTO_activeTurretManager == null)
                    AUTO_activeTurretManager = ActiveTurretManager.Instance;
                return AUTO_activeTurretManager;
            }
        }
        ActiveTurretManager AUTO_activeTurretManager = null;

        /*public UICell OldCell { get; set; }
          public UICell CurrentCell { get; set; }
          GraphicRaycasterScript IUICellOwner.GraphicRaycasterS => _GraphicRaycasterScript;
          TextMeshProUGUI IUICellOwner.DescriptionTitle => _DescriptionTitle;
          TextMeshProUGUI IUICellOwner.DescriptionText => _DescriptionText;*/


        TurretUnit _turretToSwap = null;
        //TurretData _currentTurretToPlace = null;
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
                return;
            }
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(this);
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

            if (Input.GetButtonDown("Exit") && SpawnManager.WaveIsActive == false) _battleManager.ExitBattle();
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
                    //(this as IUICellOwner).OnEnd();

                    //DeselectCurrentTurret(); 
                    //changed this(^) to that(v)
                    DeselectCurrentTile();

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
                    //(this as IUICellOwner).OnStart();
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

            //DeselectCurrentTurret(); 
            //changed this(^) to that(v)
            DeselectCurrentTile();

            EvaluateGameplayMode(SpawnManager.WaveIsActive);
            _TDCanvasManager.gameObject.SetActive(false);
        }

        /*
        void DeselectCurrentTurret()
        {
            _currentTurretToPlace = null;
            _CursorIndicator.SetTurret(null);
            DeselectCurrentTile();
        }*/
        void HandleEditMode()
        {
            if (_CraftTabToggler.Status == false)
            {
                SetCurrentTile();
            }

            //(this as IUICellOwner).OnLoop();

            if (Input.GetButtonDown("CraftingTabOpen"))
            {
                //DeselectCurrentTurret(); 
                //changed this(^) to that(v)
                DeselectCurrentTile();

                _CraftTabToggler.Toggle();

                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                /*
                if (_currentTurretToPlace)
                {
                    TryPlaceTurret();
                }
         else */if (_turretToSwap)
                {
                    TrySwapCurrentTurret();
                }
                else
                {
                    SelectTurretToSwap(_currentTile);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                //DeselectCurrentTurret(); 
                //changed this(^) to that(v)
                DeselectCurrentTile();

                DeselectTurretToSwap();
            }
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
                        TrySwapCurrentTurret();
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
                    SelectTurretToSwap(_currentTile);
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

        void TrySwapCurrentTurret()
        {
            if (_currentTile && _currentTile.IsLocked) return;

            var tur = _currentTile?.OccupyingTurret;

            //infinite turret bugfix
            if (tur && tur == _turretToSwap) return;

            SwapTiles(_currentTile, _turretToSwap._parentTile);
            DeselectTurretToSwap();
        }

        void HandleIdleMode()
        {

        }
        void SelectTurretToSwap(TowerDefenceTileScript tile)
        {
            if (tile && tile.IsLocked) return;

            var turret = tile?.OccupyingTurret;
            if (_turretToSwap != null) DeselectTurretToSwap();

            _turretToSwap = turret;
            _turretToSwap?.SetHighlight(true, this);
        }

        public void DeselectTurretToSwap()
        {
            _turretToSwap?.SetHighlight(false, this);
            _turretToSwap = null;
        }
        void DeselectRemoteTurret()
        {
            if (_selectedRemoteTurret == null) return;

            _selectedRemoteTurret.DeselectTurret();
            _selectedRemoteTurret = null;
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

        void DeleteTurret()
        {
            _currentTile?.OccupyingTurret?.KillSelf();
        }

        void SelectTile(TowerDefenceTileScript tdts)
        {
            _currentTile = tdts;
            _currentTile.GetHighlighted();

            if (tdts.IsOccupied == false && tdts.IsLocked == false)
            {
                _CursorIndicator.SetPosition(tdts.transform.position);
                _CursorIndicator.SetEnablity(true);
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

            bool dontWaitCooldown = _currentGameplayMode == TowerDefenceGameplayMode.Edit;

            if (data1)
            {
                StartCoroutine(nameof(IENUM_SwapTiles), new SwappableTurretData(data1, tile2, dontWaitCooldown));
            }
            if (data2)
            {
                StartCoroutine(nameof(IENUM_SwapTiles), new SwappableTurretData(data2, tile1, dontWaitCooldown));
            }
        }
        struct SwappableTurretData
        {
            public TurretData TData;
            public TowerDefenceTileScript TDTile;
            public bool DontWaitCooldown;

            public SwappableTurretData(TurretData data, TowerDefenceTileScript tile, bool dontWaitCooldown)
            {
                TData = data;
                TDTile = tile;
                DontWaitCooldown = dontWaitCooldown;
            }
        }
        IEnumerator IENUM_SwapTiles(SwappableTurretData data)
        {
            var tData = data.TData;
            var tile = data.TDTile;

            var ind = _IndicatorManager.GetFreeIndicator(out int i);
            ind.SetTurret(tData);
            ind.SetPosition(tile.transform.position);
            ind.SetEnablity(true);
            tile.SetIsLocked(true);

            var cooldown = data.DontWaitCooldown ? 0 : tData.SwapCooldown;
            yield return new WaitForSeconds(cooldown);

            _IndicatorManager.ReleaseIndicator(i);
            tile.SetIsLocked(false);

            _activeTurretManager.PlaceTurret(tData, tile);
        }

        void DeselectCurrentTile()
        {
            if (_currentTile == null) return;

            _currentTile.GetUnhighlighted();
            _currentTile = null;

            _CursorIndicator.SetEnablity(false);
        }

        //public void OnHoverInteractableCell(UICell currentCell) { }


        /*void IUICellOwner.OnCellClicked(UICell cell)
          {
              var tCell = cell as OwnedTurretUIScript;
              if (tCell == null) return;
              TurretData data = tCell.TData;
              if (data == null) return;

              targetTime_CanPlaceTurret = Time.time + .1f;

              _CraftTabToggler.SetStatus(false);
              _currentTurretToPlace = data;
              _CursorIndicator.SetTurret(data);
              DeselectTurretToSwap();
          }
          bool IUICellOwner.CellIsValid(UICell cell)
          {
              var newCell = cell as OwnedTurretUIScript;
              if (newCell == null) return false;
              return newCell.TData != null;
          }*/
    }
}
