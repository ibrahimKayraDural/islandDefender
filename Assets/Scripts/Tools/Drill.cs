namespace Overworld
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Drill : Tool, ISpeedUpgradable
    {
        [SerializeField] Rigidbody _Rigidbody;
        [SerializeField] Animator _Animator;
        [SerializeField] AudioClip _RunningSFX;
        AudioManager _audioManager => AudioManager.Instance;

        public UpgradeData CurrentSpeedUpgrade { get; set; } = null;
        public float SpeedMultiplier { get; set; } = 1;

        readonly string SFX_ID = "DrillTool_Running";

        public void Awake()
        {
            _Rigidbody.detectCollisions = false;
            (this as ISpeedUpgradable).RefreshSpeedUpgrade();
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

            _Animator.SetBool("IsRunning", _isFiring);

            _audioManager.StopClip(SFX_ID, true);

            _Rigidbody.detectCollisions = false;
        }

        internal override void ActivationImplementation() { }


        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Minable mnbl))
            {
                mnbl.StartMining(SpeedMultiplier);
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
