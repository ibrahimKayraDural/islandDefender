using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStrengthUpgradable
{
    /// <summary>
    /// Set the damage upgrade. Upgrade must have a float value named "strength"
    /// </summary>
    /// <param name="DoNotChangeIfWorse">Check if the new upgrade is better, do not change if it is not</param>
    /// <returns>Old upgrade. Null if it did not changed</returns>
    public UpgradeData SetStrengthUpgrade(UpgradeData newUpgrade, bool DoNotChangeIfWorse = false)
    {
        float? newStrength = newUpgrade?.TryGetFloatValue("strength");
        if (newStrength == null) return null;
        if (CurrentStrengthUpgrade != null && DoNotChangeIfWorse && newStrength <= StrengthMultiplier) return null;

        var oldUpgrade = CurrentStrengthUpgrade;
        CurrentStrengthUpgrade = newUpgrade;
        RefreshStrengthUpgrade();
        return oldUpgrade;
    }
    void RefreshStrengthUpgrade()
    {
        StrengthMultiplier = 1;

        if (CurrentStrengthUpgrade == null) return;

        var temp = CurrentStrengthUpgrade.TryGetFloatValue("strength");
        if (temp.HasValue == false) return;

        StrengthMultiplier = temp.Value;
    }
    UpgradeData CurrentStrengthUpgrade { get; set; }
    float StrengthMultiplier { get; set; }
}
