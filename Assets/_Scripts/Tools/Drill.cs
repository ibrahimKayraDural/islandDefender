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
        PlayerFuelController _PlayerFuelController
        {
            get
            {
                if (AUTO_PlayerFuelController == null)
                    AUTO_PlayerFuelController = PlayerInstance.Instance.PlayerFuelController_Ref;

                return AUTO_PlayerFuelController;
            }
        }
        PlayerFuelController AUTO_PlayerFuelController = null;

        public UpgradeData CurrentSpeedUpgrade { get; set; } = null;
        public float SpeedUpgradeValue { get; set; } = 1;
        public UpgradeData CurrentStrengthUpgrade { get; set; } = null;
        public float StrengthMultiplier { get; set; } = 1;

        readonly string SFX_ID = "DrillTool_Running";

        bool _fuelEmpty = false;

        public void Awake()
        {
            _Rigidbody.detectCollisions = false;
            (this as ISpeedUpgradable).RefreshSpeedUpgrade();
        }

        internal override IEnumerator FireIEnum()
        {
            if (_fuelEmpty) yield break;

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

        public void SpendFuel()
        {
            if (_fuelEmpty) return;

            _PlayerFuelController.TrySpendFuel();
            if(_PlayerFuelController.FuelEmpty)
            {
                _fuelEmpty = true;
                StopFiring();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Minable mnbl))
            {
                mnbl.StartMining(this);
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
