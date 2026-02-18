using System;
using System.Collections;
using System.Collections.Generic;
using Prota.Unity;
using UnityEngine;

namespace Prota
{
    [Serializable]
    public struct SpatialCoord : IEquatable<SpatialCoord>
    {
        public int x;
        public int y;
        public SpatialCoord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        
        public SpatialCoord left => new SpatialCoord(x - 1, y);
        public SpatialCoord right => new SpatialCoord(x + 1, y);
        public SpatialCoord up => new SpatialCoord(x, y + 1);
        public SpatialCoord down => new SpatialCoord(x, y - 1);
        public SpatialCoord leftUp => new SpatialCoord(x - 1, y + 1);
        public SpatialCoord rightUp => new SpatialCoord(x + 1, y + 1);
        public SpatialCoord leftDown => new SpatialCoord(x - 1, y - 1);
        public SpatialCoord rightDown => new SpatialCoord(x + 1, y - 1);
        
        public static SpatialCoord zero => new SpatialCoord(0, 0);
        public static SpatialCoord one => new SpatialCoord(1, 1);

        public bool Equals(SpatialCoord other) => x == other.x && y == other.y;
        public override bool Equals(object obj) => obj is SpatialCoord other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(x, y);
        public static bool operator ==(SpatialCoord a, SpatialCoord b) => a.Equals(b);
        public static bool operator !=(SpatialCoord a, SpatialCoord b) => !a.Equals(b);
        public static SpatialCoord operator +(SpatialCoord a, SpatialCoord b) => new SpatialCoord(a.x + b.x, a.y + b.y);
        public static SpatialCoord operator -(SpatialCoord a, SpatialCoord b) => new SpatialCoord(a.x - b.x, a.y - b.y);
        public static SpatialCoord operator *(SpatialCoord a, int b) => new SpatialCoord(a.x * b, a.y * b);
        public static SpatialCoord operator /(SpatialCoord a, int b) => new SpatialCoord(a.x / b, a.y / b);
        public static SpatialCoord operator %(SpatialCoord a, int b) => new SpatialCoord(a.x % b, a.y % b);
        
        public Vector2 ToVec() => new Vector2(x, y);
        public Vector2Int ToVecInt() => new Vector2Int(x, y);

        public override string ToString() => $"SpatialCoord({x},{y})";
    }
}
