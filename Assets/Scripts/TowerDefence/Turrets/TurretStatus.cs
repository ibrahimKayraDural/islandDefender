using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class TurretStatus : ScriptableObject
    {
        [HideInInspector] public string InstanceID = GLOBAL.UnassignedString;

        public string DisplayName => _DisplayName;
        public string ID => _id;
        public bool IsStackable => _IsStackable;
        public float Duration => _duration;
        public bool ModifyAttackSpeed => _ModifyAttackSpeed;
        public float NormalizedPercentAddition_AttackSpeed => ModifyAttackSpeed ? _NormalizedPercentAddition_AttackSpeed : 0;
        public bool ModifyDamage => _ModifyDamage;
        public float NormalizedPercentAddition_Damage => ModifyDamage ? _NormalizedPercentAddition_Damage : 0;
        public bool ModifyMaxHealth => _ModifyMaxHealth;
        public float NormalizedPercentAddition_MaxHealth => ModifyMaxHealth ? _NormalizedPercentAddition_MaxHealth : 0;


        [SerializeField] string _DisplayName = GLOBAL.UnassignedString;
        [SerializeField] string _id = GLOBAL.UnassignedString;
        [SerializeField] bool _IsStackable = false;
        [SerializeField, Tooltip("-1 means indefinetely")] float _duration = -1;
        [SerializeField] bool _ModifyAttackSpeed;
        [SerializeField] float _NormalizedPercentAddition_AttackSpeed = 0;
        [SerializeField] bool _ModifyDamage;
        [SerializeField] float _NormalizedPercentAddition_Damage = 0;
        [SerializeField] bool _ModifyMaxHealth;
        [SerializeField] float _NormalizedPercentAddition_MaxHealth = 0;
    }
}
