using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScannerUpgradeDatabase", menuName = "Upgrade/Scanner Upgrade Database", order = 1)]
public class ScannerUpgradeDatabase : ScriptableObject
{
    public List<ScannerUpgradeData> Levels;

    public ScannerUpgradeData GetDataByLevel(int level)
    {
        return Levels.Find(x => x.ScannerLevel == level);
    }
}
