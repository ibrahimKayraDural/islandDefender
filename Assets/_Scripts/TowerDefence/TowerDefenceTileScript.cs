using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class TowerDefenceTileScript : MonoBehaviour
    {
        public bool IsLocked => _isLocked;
        public TurretUnit OccupyingTurret => _occupyingTurret;
        public bool IsOccupied => _occupyingTurret != null;
        public Vector2Int TilePosition => _tilePosition;

        [SerializeField] Vector2Int _tilePosition = Vector2Int.one * -1;

        Material _material = null;
        TurretUnit _occupyingTurret = null;
        bool _isLocked = false;
        bool _isInitialized = false;

        void Awake()
        {
            if (TryGetComponent(out MeshRenderer mr))
            {
                _material = mr.material;
            }
        }

        public void Initialize(int x, int y)
        {
            if (_isInitialized) return;

            _tilePosition = new Vector2Int(x, y);
            _isInitialized = true;
        }
        public void GetHighlighted() => SetHighlighted(true);
        public void GetUnhighlighted() => SetHighlighted(false);
        public void SetHighlighted(bool setTo)
        {
            if (_material == null) return;

            _material.SetFloat("_IsHighlighted", setTo ? 1 : 0);
        }
        public void SetIsLocked(bool setTo) => _isLocked = setTo;
        public void UnOccupy()
        {
            _occupyingTurret = null;
        }
        public void SetOccupied(TurretUnit occupyingTurret)
        {
            if (IsOccupied || IsLocked) return;
            if (occupyingTurret == null)
            {
                UnOccupy();
                return;
            }

            _occupyingTurret = occupyingTurret;
        }
    }
}
