using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/Upgrade Data", fileName = "Generic Upgrade Data")]
public class UpgradeData : GameplayElementData<UpgradeData>
{
    [SerializeField] List<AYellowpaper.SerializedCollections.SerializedKeyValuePair<string, float>> _FloatValues;
    [SerializeField] List<AYellowpaper.SerializedCollections.SerializedKeyValuePair<string, GameObject>> _GameObjectValues;

    public float? TryGetFloatValue(string name)
    {
        var result = _FloatValues.FindIndex(x => x.Key == name);
        return result != -1 ? _FloatValues[result].Value : null;
    }
    public GameObject TryGetGameObjectValue(string name)
    {
        var result = _GameObjectValues.FindIndex(x => x.Key == name);
        return result != -1 ? _GameObjectValues[result].Value : null;
    }
}