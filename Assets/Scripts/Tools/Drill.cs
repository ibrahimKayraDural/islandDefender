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
        [SerializeField] AudioClip _RunningSFX;

        AudioManager _audioManager;
        readonly string SFX_ID = "DrillTool_Running";

        public void Awake()
        {
            _Rigidbody.detectCollisions = false;
        }

        internal void Start()
        {
            _audioManager = AudioManager.Instance;
        }

        internal override IEnumerator FireIEnum()
        {
            _isFiring = true;
            _Animator.SetBool("IsRunning", _isFiring);

            _audioManager.PlayClip(SFX_ID, _RunningSFX, playLooping: true, @override: true);

            _Rigidbody.detectCollisions = true;

            yield return null;
        }
        public override void StopFiring()
        {
            base.StopFiring();

            _Animator.SetBool("IsRunning",_isFiring);

            _audioManager.StopClip(SFX_ID, true);

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
