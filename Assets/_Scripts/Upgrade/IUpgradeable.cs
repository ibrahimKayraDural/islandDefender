using SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UpgradeSystem
{
    public interface IUpgradeable
    {
        string UpgradeableGUID { get; }
        void HandleUpgradeValues(string id, UpgradeData data);
        Dictionary<string, UpgradeData> Upgrades { get; }
        SaveManager SaveManagerGetter
        {
            get
            {
                if (AUTO_SaveManager == null)
                    AUTO_SaveManager = SaveManager.Instance;

                return AUTO_SaveManager;
            }
        }
        SaveManager AUTO_SaveManager { get; set; }
        UpgradeDatabase UpgradeDatabaseGetter
        {
            get
            {
                if (AUTO_UpgradeDatabaseGetter == null)
                    AUTO_UpgradeDatabaseGetter = GLOBAL.GetUpgradeDatabase();

                return AUTO_UpgradeDatabaseGetter;
            }
        }
        UpgradeDatabase AUTO_UpgradeDatabaseGetter { get; set; }
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
        void SaveUpgradeData()
        {
            if (CheckGUID() == false) return;

            var templist = new List<UpgradeData>();
            foreach (var val in Upgrades.Values) templist.Add(val);

            SaveManagerGetter.AddOrReplaceUpgradeable(UpgradeableGUID, templist);
        }
        void LoadUpgradeData()//TODO: remake dicID part. maybe use enums
        {
            if (CheckGUID() == false) return;

            var upgrades = SaveManagerGetter.CurrentSave.GetUpgradesOfGUID(UpgradeableGUID);
            if (upgrades == null) return;

            var keys = Upgrades.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                for (int n = 0; n < upgrades.UpgradeIDs.Count; n++)
                {
                    var upgradeID = upgrades.UpgradeIDs[n];

                    var data = UpgradeDatabaseGetter.GetDataByDisplayNameOrID(upgradeID);
                    SetUpgradeData(keys[i], data);
                }
            }
        }
        bool CheckGUID()
        {
            if (UpgradeableGUID == null || UpgradeableGUID == "")
            {
                Debug.LogError("A guid was not assigned to " + DisplayName +
                    "Try right clicking and selecting ''Generate GUID'' from the inspector, in the editor mode");
                return false;
            }
            return true;
        }
        string DisplayName { get; }
    }
}
