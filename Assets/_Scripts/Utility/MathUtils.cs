using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MathUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="n">Plane normal</param>
    /// <param name="c">Any point on plane</param>
    /// <param name="x0">Origin of the line</param>
    /// <param name="x1">End of the line</param>
    /// <param name="intersection">Point of intersection</param>
    /// <returns>If the line intersects with the plane</returns>
    public static bool LinePlaneIntersection(Vector3 n, Vector3 c, Vector3 x0, Vector3 x1, out Vector3 intersection)
    {
        Vector3 v = x1 - x0;
        Vector3 w = c - x0;

        float k = Vector3.Dot(w, Vector3.up) / Vector3.Dot(v, Vector3.up);
        intersection = x0 + k * v;

        return k >= 0 && k <= 1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n">Plane normal</param>
    /// <param name="c">Any point on plane</param>
    /// <param name="x0">Origin of the line</param>
    /// <param name="dir">Direction of the line</param>
    /// <param name="intersection">Point of intersection</param>
    /// <returns>If the line intersects with the plane</returns>
    public static bool InfiniteLinePlaneIntersection(Vector3 n, Vector3 c, Vector3 x0, Vector3 dir, out Vector3 intersection)
    {
        Vector3 x1 = x0 + dir;
        Vector3 v = x1 - x0;
        Vector3 w = c - x0;

        float k = Vector3.Dot(w, Vector3.up) / Vector3.Dot(v, Vector3.up);
        intersection = x0 + k * v;

        return k >= 0;
    }
}
