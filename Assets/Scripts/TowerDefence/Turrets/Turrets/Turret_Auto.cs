using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public abstract class Turret_Auto : TurretUnit
    {
        internal bool _breakActivationLoop;

        override public void Initialize(TurretData data, TowerDefenceTileScript tile)
        {
            if (_isInitialized) return;

            _data = data;
            _parentTile = tile;
            transform.parent = tile.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            SetHealth(_MaxHealth);

            tile.SetOccupied(this);
            StartCoroutine(nameof(ActivationLoop), _data.ActivationCooldown);

            _isInitialized = true;
        }

        virtual internal IEnumerator ActivationLoop(float interval)
        {
            while (_breakActivationLoop == false)
            {
                yield return new WaitForSeconds(interval);
                ActivationMethod();
            }
            _breakActivationLoop = false;
        }

        virtual internal void BreakActivationLoop()
        {
            _breakActivationLoop = true;
            StopCoroutine(nameof(ActivationLoop));
        }

        public override void KillSelf()
        {
            StopAllCoroutines();
            BreakActivationLoop();
            base.KillSelf();
        }
    } 
}
