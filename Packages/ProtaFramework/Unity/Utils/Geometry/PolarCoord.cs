using UnityEngine;
using System;
using System.Runtime.InteropServices;
using Prota;
using Prota.Unity;

namespace Prota.Unity
{
    /// <summary>
    /// 极坐标结构 - 表示距离和角度
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct PolarCoord : IEquatable<PolarCoord>
    {
        public float r;
        public float a;
        
        public PolarCoord(float distance, float angle)
        {
            this.r = distance;
            this.a = angle;
        }
        
        public PolarCoord(Vector2 center, Vector2 r)
        {
            var dir = center.To(r);
            this.r = dir.magnitude;
            a = dir.Angle();
        }

        public bool Equals(PolarCoord other)
            => r.ApproximatelyEqual(other.r) && a.ApproximatelyEqual(other.a);
            
        public override bool Equals(object obj) => obj is PolarCoord other && Equals(other);
        
        public override int GetHashCode() => (r, a).GetHashCode();
        
        public static bool operator ==(PolarCoord left, PolarCoord right) => left.Equals(right);
        
        public static bool operator !=(PolarCoord left, PolarCoord right) => !left.Equals(right);
        
        /// <summary>
        /// 转换为向量（从原点）
        /// </summary>
        public Vector2 ToVec() => a.AngleToVec2() * r;
        
        /// <summary>
        /// 转换为向量（从指定中心点）
        /// </summary>
        public Vector2 ToVec(Vector2 center) => center + ToVec();
        
        public static PolarCoord operator +(PolarCoord p, PolarCoord q)
            => new PolarCoord(p.r + q.r, p.a + q.a);
            
        public static PolarCoord operator -(PolarCoord p, PolarCoord q)
            => new PolarCoord(p.r - q.r, p.a - q.a);
            
        /// <summary>
        /// 缩放距离
        /// </summary>
        public PolarCoord Scale(float scale) => new PolarCoord(r * scale, a);
        
        /// <summary>
        /// 旋转角度（度）
        /// </summary>
        public PolarCoord Rotate(float angleInDegree) => new PolarCoord(r, this.a + angleInDegree);
        
        public override string ToString() => $"PolarCoord[{r}, {a}]";
    }
}

