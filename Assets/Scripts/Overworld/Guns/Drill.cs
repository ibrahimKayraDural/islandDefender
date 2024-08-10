namespace Overworld
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Drill : Tool
    {
        [SerializeField] Collider _DrillCollider;

        internal override IEnumerator FireIEnum()
        {
            _isFiring = true;
            _DrillCollider.enabled = true;
            yield return null;
        }
        public override void StopFiring()
        {
            base.StopFiring();

            _DrillCollider.enabled = false;
        }

        internal override void ActivationImplementation() { }


        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent(out Minable mnbl))
            {
                mnbl.StartMining();
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Minable mnbl))
            {
                mnbl.StopMining();
            }
        }
    }
}
