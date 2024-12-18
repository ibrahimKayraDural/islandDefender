using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class TowerDefenceTileScript : MonoBehaviour
    {
        public TurretUnit OccupyingTurret => _occupyingTurret;
        public bool IsOccupied => _occupyingTurret != null;

        Material _material = null;
        TurretUnit _occupyingTurret = null;

        void Awake()
        {
            if (TryGetComponent(out MeshRenderer mr))
            {
                _material = mr.material;
            }
        }

        public void GetHighlighted() => SetHighlighted(true);
        public void GetUnhighlighted() => SetHighlighted(false);
        public void SetHighlighted(bool setTo)
        {
            if (_material == null) return;

            _material.SetFloat("_IsHighlighted", setTo ? 1 : 0);
        }
        public void UnOccupy()
        {
            _occupyingTurret = null;
        }
        public void SetOccupied(TurretUnit occupyingTurret)
        {
            if (IsOccupied) return;
            if (occupyingTurret == null) return;

            _occupyingTurret = occupyingTurret;
        }
    } 
}
