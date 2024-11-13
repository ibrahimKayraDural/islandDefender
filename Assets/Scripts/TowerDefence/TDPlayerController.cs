using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TowerDefence
{
    public enum TowerDefenceControlMode { None, Remote, Full }
    public enum TowerDefenceGameplayMode { Idle, Play, Edit }
    public class TDPlayerController : MonoBehaviour
    {
        [SerializeField] LayerMask TowerDefenceLayer;
        [SerializeField] TextMeshProUGUI _CurrentTurretText;
        [SerializeField] TextMeshProUGUI _CurrentModeText;
        [SerializeField] TDCanvasManager _TDCanvasManager;
        [SerializeField] SpawnManager _SpawnManager;
        [SerializeField] Transform _MouseTracker;

        [SerializeField] Camera _camera = null;

        TurretData _currentTurret => _turrets[currentIndex];

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

        TowerDefenceTileScript _currentTile = null;
        TurretData[] _turrets;
        Turret_Remote _selectedTurret = null;
        int currentIndex = 0;
        float _changeTurret_TargetTime = -1;
        float _changeTurret_Cooldown = .2f;

        TowerDefenceGameplayMode _currentGameplayMode = TowerDefenceGameplayMode.Edit;
        TowerDefenceControlMode _currentControlMode = TowerDefenceControlMode.None;

        void Awake()
        {
            if (_camera == null)
            {
                Debug.LogError("No camera was found");
                this.enabled = false;
            }

            _turrets = GLOBAL.GetTurretDatabase().DataList.ToArray();
            _CurrentTurretText.text = _currentTurret.DisplayName;
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
                    DeselectTurret();
                    break;
                case TowerDefenceGameplayMode.Edit:
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
        }
        public void ExitBattle()
        {
            _currentControlMode = TowerDefenceControlMode.None;
            DeselectCurrentTile();
            EvaluateGameplayMode(SpawnManager.WaveIsActive);
            _TDCanvasManager.gameObject.SetActive(false);
        }

        void HandleEditMode()
        {
            SetCurrentTile();

            if (Input.GetButtonDown("PlaceTurret")) TryPlaceTurret();
            else if (Input.GetButtonDown("DeleteTurret")) DeleteTurret();

            TryChangeTurret(Input.GetAxisRaw("ChangeTurret"));
        }

        void HandlePlayMode()
        {
            SetCurrentTile();

            if (Input.GetButtonDown("ToggleSelectRemoteTurret"))
            {
                Turret_Remote turret = _currentTile != null ? _currentTile.OccupyingTurret as Turret_Remote : null;
                EvaluateTurret(turret);
            }
            if (_selectedTurret != null && Input.GetButton("UseSelectedRemoteTurret"))
            {
                _selectedTurret.UseTurret();
            }

            void EvaluateTurret(Turret_Remote turret)
            {
                if (turret == null)
                {
                    DeselectTurret();
                    return;
                }
                else if (_selectedTurret != null)
                {
                    if (_selectedTurret == turret) return;
                    DeselectTurret();
                }

                turret.SelectTurret(_MouseTracker);
                _selectedTurret = turret;
            }
        }
        void DeselectTurret()
        {
            if (_selectedTurret == null) return;

            _selectedTurret.DeselectTurret();
            _selectedTurret = null;
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

        void TryChangeTurret(float v)
        {
            if (v == 0) return;
            if (_changeTurret_TargetTime >= Time.time) return;

            v = v > 0 ? 1 : -1;
            currentIndex += (int)v;

            if (currentIndex < 0) currentIndex = _turrets.Length - 1;
            if (currentIndex >= _turrets.Length) currentIndex = 0;

            _CurrentTurretText.text = _currentTurret.DisplayName;

            _changeTurret_TargetTime = Time.time + _changeTurret_Cooldown;
        }

        void TryPlaceTurret()
        {
            if (_currentTile == null) return;
            if (_currentTile.IsOccupied) return;

            if (BaseResourceController.Instance.TryBuyTurret(_currentTurret) == false) return;

            TurretUnit unit = Instantiate(_currentTurret.PrefabObject).GetComponent<TurretUnit>();
            unit.Initialize(_currentTurret, _currentTile);
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
    }
}
