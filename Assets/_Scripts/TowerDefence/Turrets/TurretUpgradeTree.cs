using AYellowpaper.SerializedCollections.Editor.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretUpgradeTree", menuName = "Tower Defence/Turret Upgrade Tree")]
public class TurretUpgradeTree : ScriptableObject
{
    [System.Serializable]
    class UpgradeTree
    {
        public List<TurretData> Turrets;

        public UpgradeTree(List<TurretData> turrets)
        {
            Turrets = turrets;
        }
        public UpgradeTree(TurretData[] turrets)
        {
            Turrets = new(turrets);
        }

        public bool Contains(TurretData data) => Contains(data, out _);
        public bool Contains(TurretData data, out int index)
        {
            index = Turrets.FindIndex(x => x == data);
            return index >= 0;
        }
    }

    [SerializeField] List<UpgradeTree> AllTrees = new();

    void OnValidate()
    {
        List<Tuple<TurretData, int>> turrets = new();

        for (int i = 0; i < AllTrees.Count; i++)
        {
            var tree = AllTrees[i];
            if (tree == null) continue;
            foreach (var turr in tree.Turrets)
            {
                if (turr == null) continue;
                var found = turrets.FindAll(x => x.Item1 == turr);

                if (found.Count != 0)
                {
                    var msg = "Can't have more than one of the same type of turret in any of the trees. " +
                                    "This will lead to unexpected behaviours. Check " +
                                    $"{turr.DisplayName} in the tree at these indexes: {i}, ";

                    foreach (var f in found) msg += $" {f.Item2},";

                    Debug.LogError(msg);
                }

                turrets.Add(new Tuple<TurretData, int>(turr, i));
            }
        }
    }

    public List<TurretData> FindTree(TurretData turret, out int turretIndex)
    {
        turretIndex = -1;
        var tree = AllTrees.Find(x => x.Contains(turret));
        if (tree != null)
        {
            turretIndex = tree.Turrets.FindIndex(x => x == turret);
            return tree.Turrets;
        }
        return null;
    }
}
