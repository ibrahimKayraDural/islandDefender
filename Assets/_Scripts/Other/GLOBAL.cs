using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using Overworld;
using UnityEngine;

public static class GLOBAL
{

    #region Variables

    public readonly static string UnassignedString = "UNASSIGNED";

    public readonly static string TurretUnlockID = "turret";
    public readonly static string UpgradeUnlockID = "upgrade";
    public readonly static string ToolUnlockID = "tool";
    public readonly static string EnemyUnlockID = "enemy";
    public readonly static string ScannerUnlockID = "scanner";
    public readonly static string SpecialUnlockID = "special";

    public readonly static float TDColliderElevation = 1f;
    public readonly static float BaseProjectileSpeed = 50;

    public readonly static List<float> FailsafeEnemyCooldowns = new List<float>() { 5, 5, 5, 3, 3, 3, 2.5f, 2.5f, 2, 2, 2, 1 };
    public readonly static List<int> FailsafeWaveCooldowns = new List<int>() { 10, 8, 5, 5, 5, 3 };
    public readonly static List<KeyCode> AlphaNumberKeys = new List<KeyCode>()
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0,
    };
    public readonly static List<KeyCode> KeypadNumberKeys = new List<KeyCode>()
    {
        KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6, KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9, KeyCode.Keypad0,
    };
    public readonly static Dictionary<EnemyDifficulty, float> EnemyDifficultyMultipliers = new()
    {
        { EnemyDifficulty.Weak, 1f} ,
        { EnemyDifficulty.Medium, 2f} ,
        { EnemyDifficulty.Tough, 5f} ,
    };
    public readonly static Dictionary<EnemyDifficulty, float> EnemyResearchPointGain = new()
    {
        { EnemyDifficulty.Weak, 100f} ,
        { EnemyDifficulty.Medium, 250f} ,
        { EnemyDifficulty.Tough, 600f} ,
    };

    #endregion

    #region Enum IDs

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
        {"explosive",EnemyType.Explosive },
        {"swarm",EnemyType.Swarm },
        {"none",EnemyType.None },
    };

    //-------------------------^

    #endregion

    #region Functions

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

    public static bool StringHasValue(string str) => !(str == null || str == "" || str == UnassignedString);

    public static float EnemyCooldownDifficultyCalculator(float cooldown)
    {
        switch (cooldown)
        {
            case <= 0: return 1;
            case <= .3f: return 7;
            case <= .8f: return 5;
            case <= 1.5f: return 4;
            case <= 3f: return 3;
            case <= 6f: return 2;
            default: return 1;
        }
    }

    public static float DecimalSimplifier(float number, int placeCount)
    {
        placeCount = Mathf.Clamp(placeCount, 0, 10);
        float mod = 1 / Mathf.Pow(10, placeCount);
        float sub = number % mod;
        return number - sub;
    }

    #endregion

    #region Databases

    static TurretDatabase _turretDB = null;
    public static TurretDatabase GetTurretDatabase()
    {
        if (_turretDB == null) _turretDB = Resources.Load<TurretDatabase>("Databases/TurretDatabase");
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

    static TD_WaveDatabase _waveDB = null;
    public static TD_WaveDatabase GetWaveDatabase()
    {
        if (_waveDB == null) _waveDB = Resources.Load<TD_WaveDatabase>("Databases/WaveDatabase");
        return _waveDB;
    }

    static ToolDatabase _toolDB = null;
    public static ToolDatabase GetToolDatabase()
    {
        if (_toolDB == null) _toolDB = Resources.Load<ToolDatabase>("Databases/ToolDatabase");
        return _toolDB;
    }

    static UpgradeDatabase _upgradeDB = null;
    public static UpgradeDatabase GetUpgradeDatabase()
    {
        if (_upgradeDB == null) _upgradeDB = Resources.Load<UpgradeDatabase>("Databases/UpgradeDatabase");
        return _upgradeDB;
    }

    static CoreDatabase _coreDatabase = null;
    public static CoreDatabase GetCoreDatabase()
    {
        if (_coreDatabase == null) _coreDatabase = Resources.Load<CoreDatabase>("Databases/CoreDatabase");
        return _coreDatabase;
    }

    static TurretUpgradeTree _upgradeTree = null;
    public static TurretUpgradeTree GetTurretUpgradeTree()
    {
        if (_upgradeTree == null) _upgradeTree = Resources.Load<TurretUpgradeTree>("Databases/TurretUpgradeTree");
        return _upgradeTree;
    }

    #endregion
}
public enum DamageType { Fire }
