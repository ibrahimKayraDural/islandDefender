using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QueryLocationManager : MonoBehaviour
{
    [SerializeField] ScannerUpgradeDatabase _ScannerDB;
    [SerializeField] List<GameObject> _MineLocations;
    [SerializeField] List<GameObject> _RuinLocations;

    void Start()
    {
        if (_ScannerDB == null) return;

        var level = SaveManager.Instance?.CurrentSave?.SavedIntegers.Find
            (x => x.ID == ScannerManager.SCANNERLEVELKEY)?.Value ?? -1;

        var data = _ScannerDB.GetDataByLevel(level);
        if (data == null) return;

        var ruinCount = data.RevealRuinCount;
        var mineCount = data.RevealMineCount;

        for (int i = 0; i < _RuinLocations.Count; i++)
            _RuinLocations[i].SetActive(i < ruinCount);

        for (int i = 0; i < _MineLocations.Count; i++)
            _MineLocations[i].SetActive(i < mineCount);
    }
}
