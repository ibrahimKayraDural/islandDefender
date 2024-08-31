using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using Overworld;
using UnityEngine;

public static class GLOBAL
{
    public readonly static string UnassignedString = "UNASSIGNED";

    public readonly static float BaseProjectileSpeed = 5;
    public readonly static List<float> FailsafeEnemyCooldowns = new List<float>() { 5, 5, 5, 3, 3, 3, 2.5f, 2.5f, 2, 2, 2, 1 };
    public readonly static List<int> FailsafeWaveCooldowns = new List<int>() { 10, 8, 5, 5, 5, 3 };

    //register enum ids here --v

    public static Dictionary<string, EnemyDifficulty> EnemyDifficultyIDs = new Dictionary<string, EnemyDifficulty>()
    {
        {"weak",EnemyDifficulty.Weak },
        {"medium",EnemyDifficulty.Medium },
        {"tough",EnemyDifficulty.Tough }
    };
    public static Dictionary<string, EnemyRange> EnemyRangeIDs = new Dictionary<string, EnemyRange>()
    {
        {"melee",EnemyRange.Melee },
        {"ranged",EnemyRange.Ranged },
        {"special",EnemyRange.Special },
    };
    public static Dictionary<string, EnemyType> EnemyTypeIDs = new Dictionary<string, EnemyType>()
    {
        {"durable",EnemyType.Durable },
        {"fast-attacking",EnemyType.FastAttacking },
        {"fast-moving",EnemyType.FastMoving },
        {"hard-hitting",EnemyType.HardHitting },
        {"none",EnemyType.None },
    };

    //-------------------------^

    public static bool IsNull(InventoryItem item)
    {
        //since c# is a dumbass it initializes the item as null, but
        //changes it to an unitialized version (which is not null) a frame later.
        //This causes massive issues but a work-around I found is to check both anyways.
        //This method does that.
        if (item == null) return true;
        else if (item.IsInitialized == false) return true;
        return false;
    }

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

    static ToolDatabase _toolDB = null;
    public static ToolDatabase GetToolDatabase()
    {
        if (_toolDB == null) _toolDB = Resources.Load<ToolDatabase>("Databases/ToolDatabase");
        return _toolDB;
    }
}