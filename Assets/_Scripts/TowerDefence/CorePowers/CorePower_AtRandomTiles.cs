using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using UnityEngine;

namespace CorePowers
{
    public abstract class CorePower_AtRandomTiles : CorePower_Base
    {
        public int TileCount => _TileCount;

        [SerializeField] internal int _TileCount = 1;

        internal List<TowerDefenceTileScript> _tiles = new();

        /// <summary>
        /// Initializes the effect on multiple tiles
        /// </summary>
        /// <param name="args">args[0] must be a List<TowerDefenceTileScript> as the tiles to get effected.</param>
        /// <returns></returns>
        public override bool TryActivate(object[] args)
        {
            if (args.Length <= 0) return false;

            var temp = args[0] as List<TowerDefenceTileScript>;
            if (temp == null) return false;

            _tiles = temp;

            return base.TryActivate(args);
        }
    }
}
