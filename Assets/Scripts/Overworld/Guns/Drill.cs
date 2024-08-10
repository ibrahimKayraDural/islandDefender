namespace Overworld
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Drill : Tool
    {
        [SerializeField] Rigidbody _Rigidbody;

        public void Awake()
        {
            _Rigidbody.detectCollisions = false;
        }

        internal override IEnumerator FireIEnum()
        {
            _isFiring = true;
            _Rigidbody.detectCollisions = true;

            yield return null;
        }
        public override void StopFiring()
        {
            base.StopFiring();

            _Rigidbody.detectCollisions = false;
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
