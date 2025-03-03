using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    public class PlayerHealthController : MonoBehaviour, IHealth
    {
        public float Health
        {
            get => _health;
            set
            {
                _health = Mathf.Clamp(value, 0, MaxHealth);
                _PlayerHealthbarManager.SetSliderValue(_health / MaxHealth);
            }
        }

        [SerializeField] float MaxHealth = 100;
        [SerializeField] PlayerHealthbarManager _PlayerHealthbarManager;

        PlayerInstance _playerInstance
        {
            get
            {
                if (AUTO_playerInstance == null)
                    AUTO_playerInstance = PlayerInstance.Instance;
                return AUTO_playerInstance;
            }
        }
        PlayerInstance AUTO_playerInstance = null;

        Inventory _Inventory => _playerInstance.Inventory_Ref;

        float _health;

        void Awake()
        {
            Health = MaxHealth;
        }

        public void SetHealth(float setTo)
        {
            Health = setTo;
            if (Health == 0) Die();
        }
        void Die()
        {
            _Inventory.Clean();
            Health = MaxHealth;
            _playerInstance.Respawn();
        }
    }
}
