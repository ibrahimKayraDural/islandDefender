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

        internal float _MaxHealth;
        internal TurretData _data;
        internal TowerDefenceTileScript _parentTile;
        internal float _health;
        internal bool _isInitialized;

        virtual public void Initialize(TurretData data, TowerDefenceTileScript tile)
        {
            if (_isInitialized) return;

            _data = data;
            _MaxHealth = data.Health;
            _parentTile = tile;
            transform.parent = tile.transform;
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
            SetHealth(_MaxHealth);

            tile.SetOccupied(this);
            OnInitialized();

            _isInitialized = true;
        }

        abstract internal void OnInitialized();

        abstract internal void ActivationMethod();

        virtual public void RemoveHealth(float amount)
        {
            AudioManager.Instance.PlayClip(Data.ID + "_GettingDamaged", Data.GetDamagedSFX);
            SetHealth(Health - amount);
        }
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
