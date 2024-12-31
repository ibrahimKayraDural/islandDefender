using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpeedUpgradable
{
    /// <summary>
    /// Set the speed upgrade. Upgrade must have a float value named "speed"
    /// </summary>
    /// <param name="DoNotChangeIfWorse">Check if the new upgrade is faster, do not change if it is not</param>
    /// <returns>Old movement upgrade. Null if did not changed</returns>
    public UpgradeData SetSpeedUpgrade(UpgradeData newUpgrade, bool DoNotChangeIfWorse = false)
    {
        float? newSpeed = newUpgrade?.TryGetFloatValue("speed");
        if (newSpeed == null) return null;
        if (CurrentSpeedUpgrade != null && DoNotChangeIfWorse && newSpeed <= SpeedMultiplier) return null;

        var oldUpgrade = CurrentSpeedUpgrade;
        CurrentSpeedUpgrade = newUpgrade;
        RefreshSpeedUpgrade();
        return oldUpgrade;
    }
    void RefreshSpeedUpgrade()
    {
        SpeedMultiplier = 1;

        if (CurrentSpeedUpgrade == null) return;

        var temp = CurrentSpeedUpgrade.TryGetFloatValue("speed");
        if (temp.HasValue == false) return;

        SpeedMultiplier = temp.Value;
    }
    UpgradeData CurrentSpeedUpgrade { get; set; }
    float SpeedMultiplier { get; set; }
}
