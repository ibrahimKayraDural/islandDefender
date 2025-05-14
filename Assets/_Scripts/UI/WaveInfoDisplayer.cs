using Overworld;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefence;
using UnityEngine;
using static TowerDefence.SpawnManager;

public class WaveInfoDisplayer : MonoBehaviour
{
    [SerializeField] bool _ShowPreviousWave = false;
    [SerializeField] SpawnManager _SpawnManager;
    [SerializeField] TextMeshProUGUI _DifficultyTM;
    [SerializeField] TextMeshProUGUI _EnemyTypesTM;
    [SerializeField] TextMeshProUGUI _RewardTM;

    ToolDatabase _toolDatabase;
    TurretDatabase _turretDatabase;
    EnemyDatabase _enemyDatabase;

    void Awake()
    {
        _toolDatabase = GLOBAL.GetToolDatabase();
        _turretDatabase = GLOBAL.GetTurretDatabase();
        _enemyDatabase = GLOBAL.GetEnemyDatabase();
    }

    public void RefreshWaveInfo()
    {
        var info = GetWaveInfo();
        if (info == null)
        {
            CleanAllInfo();
            return;
        }

        SetValues(info.Value);
    }

    void SetValues(WaveValueInfo info)
    {
        _DifficultyTM.text = info.DifficultyMultiplier.ToString();
        _EnemyTypesTM.text = "";
        for (int i = 0; i < info.EnemyTypes.Count; i++)
        {
            _EnemyTypesTM.text += info.EnemyTypes[i];
            if (i < info.EnemyTypes.Count - 1) _EnemyTypesTM.text += "\n";
        }

        _RewardTM.text = "";
        for (int i = 0; i < info.Rewards.Count; i++)
        {
            var reward = info.Rewards[i];
            _RewardTM.text += reward.Count + " ";
            _RewardTM.text += reward.Resource.DisplayName;
            if (i < info.Rewards.Count - 1) _RewardTM.text += "\n";
        }
        for (int i = 0; i < info.UnlockIDs.Count; i++)
        {
            if (i == 0) _RewardTM.text += "\n";

            var u = info.UnlockIDs[i];
            var unlockName = GetUnlockDisplayName(u);
            if (unlockName == null) continue;

            _RewardTM.text += "<color=yellow><i><b>Unlocks:</b></i></color> " + unlockName;
            if (i < info.UnlockIDs.Count - 1) _RewardTM.text += "\n";
        }
    }
    string GetUnlockDisplayName(string id)
    {
        var sep = id.Split('=');
        if (sep.Length <= 1) return null;

        string type = sep[0];
        string val = sep[1];

        if (type == GLOBAL.TurretUnlockID)
            return _turretDatabase?.GetDataByDisplayNameOrID(val)?.DisplayName;
        else if (type == GLOBAL.ToolUnlockID)
            return _toolDatabase?.GetToolByNameOrID(val)?.Data?.DisplayName;
        //else if (id == GLOBAL.EnemyUnlockID)
        //    return _enemyDatabase?.GetDataByDisplayNameOrID(id)?.DisplayName;
        else if (type == GLOBAL.UpgradeUnlockID)
        {
            return val?.Replace("-", " ");
        }
        else if (type == GLOBAL.SpecialUnlockID)
        {
            return val?.Replace("-", " ");
        }

        return null;
    }
    void CleanAllInfo()
    {
        _DifficultyTM.text = "";
        _EnemyTypesTM.text = "";
        _RewardTM.text = "";
    }
    WaveValueInfo? GetWaveInfo()
    {
        if (_SpawnManager == null) return null;

        if (_ShowPreviousWave) return _SpawnManager.PreviousWaveValueInfo;
        else return _SpawnManager.CurrentWaveValueInfo;
    }
}
