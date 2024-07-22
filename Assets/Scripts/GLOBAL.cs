using TowerDefence;
using UnityEngine;

public static class GLOBAL
{
    public static string UnassignedString = "UNASSIGNED";

    public static float BaseProjectileSpeed = 5;

    static TurretDatabase _turretDB = null;
    public static TurretDatabase GetTurretDatabase()
    {
        if(_turretDB == null) _turretDB = Resources.Load<TurretDatabase>("Databases/TurretDatabase");
        return _turretDB;
    }

    static ResourceDatabase _resourceDB = null;
    public static ResourceDatabase GetResourceDatabase()
    {
        if (_resourceDB == null) _resourceDB = Resources.Load<ResourceDatabase>("Databases/ResourceDatabase");
        return _resourceDB;
    }
}
