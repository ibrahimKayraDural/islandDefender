using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class TurretUnit : MonoBehaviour
    {
        internal TurretData _data;
        internal TowerDefenceTileScript _parentTile;


        public void Initialize(TurretData data, TowerDefenceTileScript tile)
        {
            _data = data;
            _parentTile = tile;
            transform.parent = tile.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            tile.SetOccupied(this);
        }
        public void KillSelf()
        {
            Destroy(gameObject);
        }
    } 
}
