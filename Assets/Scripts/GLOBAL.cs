using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using UnityEngine;

public static class GLOBAL
{
    public static string UnassignedString = "UNASSIGNED";

    public static float BaseProjectileSpeed = 5;
    public static List<float> FailsafeEnemyCooldowns = new List<float>() { 5, 5, 5, 3, 3, 3, 2.5f, 2.5f, 2, 2, 2, 1 };

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

    static EnemyDatabase _enemyDB = null;
    public static EnemyDatabase GetEnemyDatabase()
    {
        if (_enemyDB == null) _enemyDB = Resources.Load<EnemyDatabase>("Databases/EnemyDatabase");
        return _enemyDB;
    }

    static SwarmDatabase _swarmDB = null;
    public static SwarmDatabase GetSwarmDatabase()
    {
        if (_swarmDB == null) _swarmDB = Resources.Load<SwarmDatabase>("Databases/SwarmDatabase");
        return _swarmDB;
    }
}
