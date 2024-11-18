using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Turret Database")]
    public class TurretDatabase : Database<TurretData>
    {
        public List<TurretData> GetAllTurretsOfType(TurretControlType[] types) => DataList.FindAll(x => types.Contains(x.Type));
        public List<TurretData> GetManualTurrets() => GetAllTurretsOfType(new TurretControlType[] { TurretControlType.Manual });
        public List<TurretData> GetFieldTurrets() => GetAllTurretsOfType
            (new TurretControlType[] { TurretControlType.Automatic, TurretControlType.Remote });
    }
}
