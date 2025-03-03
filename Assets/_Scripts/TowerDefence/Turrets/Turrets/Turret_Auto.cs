using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public abstract class Turret_Auto : TurretUnit
    {
        internal bool _breakActivationLoop;

        internal override void OnInitialized()
        {
            StartCoroutine(nameof(ActivationLoop), _data.ActivationCooldown);
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
