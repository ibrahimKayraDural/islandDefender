namespace Overworld
{
    using SaveSystem;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TowerDefence;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UpgradeSystem;

    public class Drill : Tool, IUpgradeable
    {
        public int DrillLevel => Mathf.Max(Mathf.FloorToInt(_strengthMultiplier), 1);
        public float SpeedUpgradeValue => _speedUpgradeValue;

        [SerializeField] string _GUID = "PLAYERDRILL";
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

        readonly string SFX_ID = "DrillTool_Running";

        bool _fuelEmpty = false;
        bool _isSaved;//prevents race conditions

        public void Awake()
        {
            _Rigidbody.detectCollisions = false;
        }
        void Start()
        {
            SceneManager.activeSceneChanged += CheckSave;

            (this as IUpgradeable).LoadUpgradeData();
        }
        void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= CheckSave;
            (this as IUpgradeable).SaveUpgradeData();
            _isSaved = true;
        }
        void CheckSave(Scene oldScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= CheckSave;
            if (_isSaved == false) (this as IUpgradeable).SaveUpgradeData();
        }

        #region Upgrade

        [ContextMenu("Generate GUID")]
        void GenerateGUIDGetter() => _GUID = Guid.NewGuid().ToString();
        public string UpgradeableGUID => _GUID;

        public Dictionary<string, UpgradeData> Upgrades => _Upgrades;

        public SaveManager AUTO_SaveManager { get; set; }
        public UpgradeDatabase AUTO_UpgradeDatabaseGetter { get; set; }
        public string DisplayName => name;

        [SerializeField]
        Dictionary<string, UpgradeData> _Upgrades = new()
        {
            { "speed", null },
            { "strength", null },
        };

        float _speedUpgradeValue = 1;
        float _strengthMultiplier = 1;

        public void HandleUpgradeValues(string id, UpgradeData data)
        {
            if (data.TryGetFloatValue(id, out float temp) == false) return;

            Upgrades[id] = data;

            if (id == "speed")
            {
                _speedUpgradeValue = temp;
            }
            else if (id == "strength")
            {
                _strengthMultiplier = temp;
            }
        }
        #endregion

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
            if (_PlayerFuelController.FuelEmpty)
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
