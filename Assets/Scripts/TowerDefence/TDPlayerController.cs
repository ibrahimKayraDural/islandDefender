using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TowerDefence
{
    public class TDPlayerController : MonoBehaviour
    {
        [SerializeField] LayerMask TowerDefenceLayer;
        [SerializeField] TextMeshProUGUI _CurrentTurretText;

        Camera _camera = null;
        TowerDefenceTileScript _currentTile = null;
        int currentIndex = 0;
        TurretData[] _turrets;
        TurretData _currentTurret => _turrets[currentIndex];
        float _changeTurret_TargetTime = -1;
        float _changeTurret_Cooldown = .2f;

        void Awake()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Debug.LogError("No camera was found");
                this.enabled = false;
            }

            _turrets = GLOBAL.GetTurretDatabase().Turrets.ToArray();
            _CurrentTurretText.text = _currentTurret.DisplayName;
        }

        void Update()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
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

            if (Input.GetButtonDown("PlaceTurret")) TryPlaceTurret();
            else if (Input.GetButtonDown("DeleteTurret")) DeleteTurret();
            TryChangeTurret(Input.GetAxisRaw("ChangeTurret"));
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

        void TryPlaceTurret()
        {
            if (_currentTile == null) return;
            if (_currentTile.IsOccupied) return;

            TurretUnit unit = Instantiate(_currentTurret.PrefabObject).GetComponent<TurretUnit>();
            unit.Initialize(_currentTurret, _currentTile);
        }

        void DeleteTurret()
        {
            _currentTile?.UnOccupy();
        }
    }
}
