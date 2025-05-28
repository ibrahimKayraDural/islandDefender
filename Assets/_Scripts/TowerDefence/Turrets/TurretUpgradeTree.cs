using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [CreateAssetMenu(fileName = "TurretUpgradeTree", menuName = "Tower Defence/Turret Upgrade Tree")]
    public class TurretUpgradeTree : ScriptableObject
    {
        public List<UpgradeTree> AllTrees => _AllTrees;

        [System.Serializable]
        public class TurretUpgradePiece
        {
            public TurretData Turret;
            public List<Cost> Costs;
        }
        [System.Serializable]
        public class UpgradeTree
        {
            public List<TurretUpgradePiece> Upgrades;

            public UpgradeTree(List<TurretUpgradePiece> upgrades)
            {
                Upgrades = upgrades;
            }
            public UpgradeTree(TurretUpgradePiece[] upgrades)
            {
                Upgrades = new(upgrades);
            }

            public bool Contains(TurretData data) => Contains(data, out _);
            public bool Contains(TurretData data, out int index)
            {
                index = Upgrades.FindIndex(x => x.Turret == data);
                return index >= 0;
            }
        }

        [SerializeField] List<UpgradeTree> _AllTrees = new();

        void OnValidate()
        {
            List<Tuple<TurretUpgradePiece, int>> turrets = new();

            for (int i = 0; i < AllTrees.Count; i++)
            {
                var tree = AllTrees[i];
                if (tree == null) continue;
                foreach (var turr in tree.Upgrades)
                {
                    if (turr == null) continue;
                    var found = turrets.FindAll(x => x.Item1 == turr);

                    if (found.Count != 0)
                    {
                        var msg = "Can't have more than one of the same type of turret in any of the trees. " +
                                        "This will lead to unexpected behaviours. Check " +
                                        $"{turr.Turret.DisplayName} in the tree at these indexes: {i}, ";

                        foreach (var f in found) msg += $" {f.Item2},";

                        Debug.LogError(msg);
                    }

                    turrets.Add(new Tuple<TurretUpgradePiece, int>(turr, i));
                }
            }
        }

        public UpgradeTree FindTree(TurretData turret, out int turretIndex)
        {
            turretIndex = -1;
            var tree = AllTrees.Find(x => x.Contains(turret));
            if (tree != null)
            {
                turretIndex = tree.Upgrades.FindIndex(x => x.Turret == turret);
                return tree;
            }
            return null;
        }
    }
}
