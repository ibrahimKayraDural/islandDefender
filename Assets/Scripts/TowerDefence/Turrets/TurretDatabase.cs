using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Turret Database")]
    public class TurretDatabase : ScriptableObject
    {
        public List<TurretData> Turrets => _Turrets;

        [SerializeField] List<TurretData> _Turrets = new List<TurretData>();

        public TurretData GetTurretByID(string id) => Turrets.Find(x => x.ID == id);
        public TurretData GetTurretByDisplayName(string displayName) => Turrets.Find(x => x.DisplayName == displayName);
    }
}