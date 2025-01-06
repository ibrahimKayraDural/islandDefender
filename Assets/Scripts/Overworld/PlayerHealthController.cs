using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    public class PlayerHealthController : MonoBehaviour, IHealth
    {
        public float Health => _health;

        [SerializeField] float MaxHealth = 100;

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
            _health = MaxHealth;
        }

        public void SetHealth(float setTo)
        {
            _health = setTo;
            _health = Mathf.Clamp(_health, 0, MaxHealth);
            if (_health == 0) Die();
        }
        void Die()
        {
            _Inventory.Clean();
            _health = MaxHealth;
            _playerInstance.Respawn();
        }
    }
}
