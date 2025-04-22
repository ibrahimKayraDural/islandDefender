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

    //Difficulty
    //Enemy Types
    //Rewards

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
        _RewardTM.text = info.RPReward + " RP";
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
