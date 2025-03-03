namespace Overworld
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Drill : Tool, ISpeedUpgradable, IStrengthUpgradable
    {
        public int DrillLevel => Mathf.Max(Mathf.FloorToInt(StrengthMultiplier), 1);

        [SerializeField] Rigidbody _Rigidbody;
        [SerializeField] Animator _Animator;
        [SerializeField] AudioClip _RunningSFX;

        AudioManager _AudioManager
        {
            get
            {
                if (AUTO_AudioManager == null)
                    AUTO_AudioManager = AudioManager.Instance;

                return AUTO_AudioManager;
            }
        }
        AudioManager AUTO_AudioManager = null;

        public UpgradeData CurrentSpeedUpgrade { get; set; } = null;
        public float SpeedUpgradeValue { get; set; } = 1;
        public UpgradeData CurrentStrengthUpgrade { get; set; } = null;
        public float StrengthMultiplier { get; set; } = 1;

        readonly string SFX_ID = "DrillTool_Running";

        public void Awake()
        {
            _Rigidbody.detectCollisions = false;
            (this as ISpeedUpgradable).RefreshSpeedUpgrade();
        }
        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.Keypad2)) (this as IStrengthUpgradable).SetStrengthUpgrade(GLOBAL.GetUpgradeDatabase().GetDataByDisplayNameOrID("upgrade-strength-2"));
        //}

        internal override IEnumerator FireIEnum()
        {
            _isFiring = true;
            _Animator.SetBool("IsRunning", _isFiring);

            _AudioManager.PlayClip(SFX_ID, _RunningSFX, playLooping: true, @override: true);

            _Rigidbody.detectCollisions = true;

            yield return null;
        }
        public override void StopFiring()
        {
            base.StopFiring();

            _Animator.SetBool("IsRunning", _isFiring);

            _AudioManager.StopClip(SFX_ID, true);

            _Rigidbody.detectCollisions = false;
        }

        internal override void ActivationImplementation() { }


        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Minable mnbl))
            {
                mnbl.StartMining(SpeedUpgradeValue, DrillLevel);
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
