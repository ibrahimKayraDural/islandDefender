using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public abstract class TurretUnit : MonoBehaviour, IHealth
    {
        float CalculateModifier(string valueID)
        {
            valueID = valueID.ToLower();
            float totalModifier = 0;
            List<string> usedStatuses = new List<string>();
            foreach (var s in ActiveStatuses)
            {
                if (s.IsStackable == false && usedStatuses.Contains(s.ID)) continue;

                float value;
                switch (valueID)
                {
                    case "maxhealth": value = s.NormalizedPercentAddition_MaxHealth; break;
                    case "damage": value = s.NormalizedPercentAddition_Damage; break;
                    case "attackspeed": case "cooldown": value = s.NormalizedPercentAddition_AttackSpeed; break;
                    default: value = 0; break;
                }

                totalModifier += value;
                usedStatuses.Add(s.ID);
            }
            return totalModifier;
        }

        public float ActiveMaxHealth
        {
            get
            {
                float modifier = CalculateModifier("maxhealth");
                return Mathf.Max(1, _data.Health * (1 + modifier));
            }
        }
        public float ActiveDamage
        {
            get
            {
                float modifier = CalculateModifier("damage");
                return Mathf.Max(0, _data.Damage * (1 + modifier));
            }
        }
        public float ActiveActivationCooldown
        {
            get
            {
                float modifier = CalculateModifier("attackspeed");
                return Data.ActivationCooldown / (1 + Mathf.Max(-.99f, modifier));
            }
        }
        public TurretData Data => _data;
        public float Health => _health;

        internal List<TurretStatus> ActiveStatuses = new List<TurretStatus>();
        internal TurretData _data;
        internal TowerDefenceTileScript _parentTile;
        internal float _health;
        internal bool _isInitialized;

        public bool AddStatus(TurretStatus status)
        {
            string id = status.ID;
            for (int i = 0; i < 100; i++)
            {
                string idThisPass = id + "_" + i;

                if (ActiveStatuses.FindIndex(x => x.InstanceID == idThisPass) != -1) continue;
                else
                {
                    status.InstanceID = idThisPass;
                    ActiveStatuses.Add(status);
                    return true;
                }
            }
            return false;
        }

        virtual public void Initialize(TurretData data, TowerDefenceTileScript tile)
        {
            if (_isInitialized) return;

            _data = data;
            _parentTile = tile;
            transform.parent = tile.transform;
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
            SetHealth(ActiveMaxHealth);

            tile.SetOccupied(this);
            OnInitialized();

            _isInitialized = true;
        }

        abstract internal void OnInitialized();

        abstract internal void ActivationMethod();

        virtual public void SetHealth(float setTo)
        {
            _health = Mathf.Clamp(setTo, 0, ActiveMaxHealth);

            if (_health == 0) KillSelf();
        }
        virtual public void KillSelf()
        {
            _parentTile.UnOccupy();
            Destroy(gameObject);
        }
    }
}
