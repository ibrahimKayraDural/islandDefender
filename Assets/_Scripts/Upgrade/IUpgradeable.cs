using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeSystem
{
    public interface IUpgradeable
    {
        void HandleUpgradeValues(string id, UpgradeData data);
        Dictionary<string, UpgradeData> Upgrades { get; }
        void SetUpgradeData(string id, UpgradeData data, bool skipIfWorse = true)
        {
            if (data == null || id == null) return;
            if (Upgrades.ContainsKey(id) == false) return;

            if (data.TryGetFloatValue(id, out float value) == false) return;

            if (skipIfWorse && CheckIfBetter(id, value) == false) return;

            HandleUpgradeValues(id, data);
        }
        bool CheckIfBetter(string id, float value)
        {
            var oldData = Upgrades[id];
            if (oldData == null) return true;

            if (oldData.TryGetFloatValue(id, out float oldValue) == false) return true;

            return value > oldValue;
        }
    }
}
