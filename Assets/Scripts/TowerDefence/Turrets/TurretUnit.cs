using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class TurretUnit : MonoBehaviour, IGetHit
    {
        public TurretData Data => _data;
        public float MaxHealth => _MaxHealth;
        
        [SerializeField,Min(1)] internal float _MaxHealth = 1;

        internal TurretData _data;
        internal TowerDefenceTileScript _parentTile;
        internal float _health;
        internal bool _breakActivationLoop;
        internal bool _isInitialized;

        virtual public void Initialize(TurretData data, TowerDefenceTileScript tile)
        {
            if (_isInitialized) return;

            _data = data;
            _parentTile = tile;
            transform.parent = tile.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            _health = _MaxHealth;

            tile.SetOccupied(this);
            StartCoroutine(nameof(ActivationLoop), _data.ActivationCooldown);

            _isInitialized = true;
        }

        virtual internal IEnumerator ActivationLoop(float interval)
        {
            while(_breakActivationLoop == false)
            {
                yield return new WaitForSeconds(interval);
                ActivationMethod();
            }
            _breakActivationLoop = false;
        }

        virtual internal void ActivationMethod()
        {
            Debug.Log(gameObject.name + " is activated, based");
        }

        virtual internal void BreakActivationLoop()
        {
            _breakActivationLoop = true;
            StopCoroutine(nameof(ActivationLoop));
        }
        virtual public void GetHit(float damage) => RemoveHealth(damage);
        virtual public void AddHealth(float value) => SetHealth(_health + value);
        virtual public void RemoveHealth(float value) => SetHealth(_health - value);
        virtual public void SetHealth(float setTo)
        {
            _health = Mathf.Clamp(setTo, 0, _MaxHealth);

            if (_health == 0) KillSelf();
        }
        virtual public void KillSelf()
        {
            _parentTile.UnOccupy();
            BreakActivationLoop();
            StopAllCoroutines();
            Destroy(gameObject);
        }
    } 
}
