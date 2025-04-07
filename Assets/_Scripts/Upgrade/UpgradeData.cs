using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace UpgradeSystem
{
    [CreateAssetMenu(menuName = "Upgrade/Upgrade Data", fileName = "Generic Upgrade Data")]
    public class UpgradeData : GameplayElementData<UpgradeData>
    {
        [SerializeField] List<AYellowpaper.SerializedCollections.SerializedKeyValuePair<string, float>> _FloatValues;
        [SerializeField] List<AYellowpaper.SerializedCollections.SerializedKeyValuePair<string, GameObject>> _GameObjectValues;

        public bool TryGetFloatValue(string name, out float value)
        {
            var result = _FloatValues.FindIndex(x => x.Key == name);
            bool successfull = result >= 0;
            value = successfull ? _FloatValues[result].Value : 0;
            return successfull;
        }
        public GameObject TryGetGameObjectValue(string name)
        {
            var result = _GameObjectValues.FindIndex(x => x.Key == name);
            return result != -1 ? _GameObjectValues[result].Value : null;
        }
    }
}