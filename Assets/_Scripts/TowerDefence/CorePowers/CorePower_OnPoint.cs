using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CorePowers
{
    /// <summary>
    /// Same with the CorePower_Base, however, it also initializes with a Vector3? parameter named "_point"
    /// </summary>
    public abstract class CorePower_OnPoint : CorePower_Base
    {
        internal Vector3? _point = null;

        /// <summary>
        /// Initializes the effect on a point
        /// </summary>
        /// <param name="args">args[0] must be a Vector3 as the point of effect.</param>
        /// <returns></returns>
        public override bool TryActivate(object[] args)
        {
            if (args.Length <= 0) return false;
            _point = args[0] as Vector3?;
            if (_point.HasValue == false) return false;

            return base.TryActivate(args);
        }
    }
}
