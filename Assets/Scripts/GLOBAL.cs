using TowerDefence;
using UnityEngine;

public static class GLOBAL
{
    public static string UnassignedString = "UNASSIGNED";


    static TurretDatabase _turretDB = null;
    public static TurretDatabase GetTurretDatabase()
    {
        if(_turretDB == null) _turretDB = Resources.Load<TurretDatabase>("Databases/TurretDatabase");
        return _turretDB;
    }
}
