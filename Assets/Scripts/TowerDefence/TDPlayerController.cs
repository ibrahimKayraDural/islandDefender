using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TowerDefence
{
    public class TDPlayerController : MonoBehaviour
    {
        [SerializeField] LayerMask TowerDefenceLayer;
        [SerializeField] GameObject _Debugger1;
        [SerializeField] TextMeshProUGUI _CurrentTurretText;

        Camera _camera = null;
        TowerDefenceTileScript _currentTile = null;

        void Awake()
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Debug.LogError("No camera was found");
                this.enabled = false;
            }
        }

        void Update()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit hit, 100, TowerDefenceLayer);
            _Debugger1.transform.position = hit.point;

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
        }

        private void SelectTile(TowerDefenceTileScript tdts)
        {
            _currentTile = tdts;
            _currentTile.GetHighlighted();
        }

        private void DeselectCurrentTile()
        {
            if (_currentTile == null) return;

            _currentTile.GetUnhighlighted();
            _currentTile = null;
        }
    }
}
