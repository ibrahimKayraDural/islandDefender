using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using TowerDefence;
using System.Linq;

public class EnemyUnlockManager : MonoBehaviour
{
    public static EnemyUnlockManager Instance { get; private set; } = null;

    [SerializeField] List<SerializedKeyValuePair<SerializedKeyValuePair<string, int>, EnemyData>> UnlockWhenMined = new List<SerializedKeyValuePair<SerializedKeyValuePair<string, int>, EnemyData>>();

    Dictionary<string, int> _minedObjects = new Dictionary<string, int>();
    GameplayManager _GameplayManager
    {
        get
        {
            if (AUTO_GameplayManager == null)
                AUTO_GameplayManager = GameplayManager.Instance;
            return AUTO_GameplayManager;
        }
    }
    GameplayManager AUTO_GameplayManager = null;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void RegisterMinedObject(string minedObjectID)
    {
        if (minedObjectID == GLOBAL.UnassignedString) return;

        if (_minedObjects.ContainsKey(minedObjectID)) _minedObjects[minedObjectID]++;
        else _minedObjects.Add(minedObjectID, 1);

        CheckUnlocks();
    }

    void CheckUnlocks()
    {
        var allList = UnlockWhenMined.FindAll(x => _minedObjects.ContainsKey(x.Key.Key) && _minedObjects[x.Key.Key] >= x.Key.Value);
        if (allList.Count <= 0) return;

        var enemyList = allList.Select(x => x.Value).ToList();
        enemyList.ForEach(x => _GameplayManager.AddToEnemyPool(x));
    }
}
