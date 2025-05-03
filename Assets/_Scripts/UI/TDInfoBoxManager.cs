using CustomPointerEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDInfoBoxManager : MonoBehaviour
{
    [SerializeField] GameObject[] _InfoBoxes;

    public void EnableInfoBox(HoverData data)
    {
        var floats = data.AdditionalFloatDatas;
        if (floats == null || floats.Count <= 0) return;

        int index = (int)data.AdditionalFloatDatas[0];
        if (index >= _InfoBoxes.Length) return;

        EnableOnly(index);
    }
    public void DisableAllInfoBoxes() => EnableOnly(-1);
    public void EnableOnly(int indexException)
    {
        for (int i = 0; i < _InfoBoxes.Length; i++)
        {
            bool enablity = i == indexException;
            _InfoBoxes[i].SetActive(enablity);
        }
    }
}
