using System;
using System.Collections;
using System.Collections.Generic;
using Prota.Unity;
using UnityEngine;

namespace Prota
{
    public static class SpatialUtils
    {
        public static void GetCircleBounding(Vector2 center, float radius, float gridSize, out SpatialCoord min, out SpatialCoord max)
        {
            var minx = center.x - radius;
            var miny = center.y - radius;
            var maxx = center.x + radius;
            var maxy = center.y + radius;
            var minxs = Mathf.FloorToInt(minx / gridSize);
            var minys = Mathf.FloorToInt(miny / gridSize);
            var maxxs = Mathf.CeilToInt(maxx / gridSize);
            var maxys = Mathf.CeilToInt(maxy / gridSize);
            min = new SpatialCoord(minxs, minys);
            max = new SpatialCoord(maxxs, maxys);
        }
    }
}
