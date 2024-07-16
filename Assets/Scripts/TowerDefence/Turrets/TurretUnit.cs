using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class TurretUnit : MonoBehaviour
    {
        internal TurretData _data;
        internal TowerDefenceTileScript _parentTile;
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

            tile.SetOccupied(this);
            StartCoroutine(nameof(ActivationLoop), _data.ActivationCooldown);

            _isInitialized = true;
        }

        virtual internal IEnumerator ActivationLoop(float interval)
        {
            while(_breakActivationLoop == false)
            {
                ActivationMethod();
                yield return new WaitForSeconds(interval);
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

        virtual public void KillSelf()
        {
            BreakActivationLoop();
            StopAllCoroutines();
            Destroy(gameObject);
        }
    } 
}
