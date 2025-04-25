using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScannerUpgradeData", menuName = "Upgrade/Scanner Upgrade Level", order = 0)]
public class ScannerUpgradeData : ScriptableObject
{
    public int ScannerLevel;
    public List<Cost> Costs;

    public int RevealMineCount = 1;
    public int RevealRuinCount = 1;
}
