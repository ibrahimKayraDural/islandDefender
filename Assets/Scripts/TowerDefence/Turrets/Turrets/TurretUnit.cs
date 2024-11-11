using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public abstract class TurretUnit : MonoBehaviour, IHealth
    {
        public TurretData Data => _data;
        public float MaxHealth => _MaxHealth;
        public float Health => _health;

        [SerializeField,Min(.1f)] internal float _MaxHealth = 1;

        internal TurretData _data;
        internal TowerDefenceTileScript _parentTile;
        internal float _health;
        internal bool _isInitialized;

        virtual public void Initialize(TurretData data, TowerDefenceTileScript tile)
        {
            if (_isInitialized) return;

            _data = data;
            _parentTile = tile;
            transform.parent = tile.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            SetHealth(_MaxHealth);

            tile.SetOccupied(this);

            _isInitialized = true;
        }

        abstract internal void ActivationMethod();

        virtual public void SetHealth(float setTo)
        {
            _health = Mathf.Clamp(setTo, 0, _MaxHealth);

            if (_health == 0) KillSelf();
        }
        virtual public void KillSelf()
        {
            _parentTile.UnOccupy();
            Destroy(gameObject);
        }
    } 
}
