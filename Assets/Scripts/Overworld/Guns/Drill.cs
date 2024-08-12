namespace Overworld
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Drill : Tool
    {
        [SerializeField] Rigidbody _Rigidbody;
        [SerializeField] Animator _Animator;
        [SerializeField] AudioSource _ASource;

        public void Awake()
        {
            _Rigidbody.detectCollisions = false;
        }

        internal override IEnumerator FireIEnum()
        {
            _isFiring = true;
            _Animator.SetBool("IsRunning", _isFiring);
            StopCoroutine(nameof(StopSoundIEnum));
            _ASource.Play();
            _Rigidbody.detectCollisions = true;

            yield return null;
        }
        public override void StopFiring()
        {
            base.StopFiring();

            _Animator.SetBool("IsRunning",_isFiring);
            StartCoroutine(nameof(StopSoundIEnum));
            _Rigidbody.detectCollisions = false;
        }

        IEnumerator StopSoundIEnum()
        {
            float remaining = (_ASource.clip.length - _ASource.time) - .05f;
            yield return new WaitForSeconds(remaining);
            _ASource.Stop();
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
