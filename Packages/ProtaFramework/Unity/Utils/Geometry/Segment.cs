using System;
using UnityEngine;

namespace Prota.Unity
{
	public struct Segment : IEquatable<Segment>, IComparable<Segment>
	{
		private const float Eps = 1e-6f;
		
		public Vector2 from;
		public Vector2 to;
		
		public Segment(Vector2 from, Vector2 to)
		{
			this.from = from;
			this.to = to;
		}
		
		public readonly bool isZeroLength => length < Eps;
		
		public readonly Vector2 center => (from + to) / 2;
		
		public readonly Vector2 direction
		{
			get
			{
				if (isZeroLength)
					throw new InvalidOperationException("零长度Segment无法获取方向");
				return (to - from).normalized;
			}
		}
		
		public readonly Vector2 delta => to - from;
		
		public readonly float length => delta.magnitude;
		
		public bool Equals(Segment other)
		{
			if(from.Equals(other.from) && to.Equals(other.to)) return true;
			if(from.Equals(other.to) && to.Equals(other.from)) return true;
			return false;
		}
		
		public override bool Equals(object obj)
		{
			return obj is Segment other && Equals(other);
		}
		
		public override int GetHashCode()
		{
			// 确保线段的方向不影响哈希值
			var min = from.x < to.x || (from.x == to.x && from.y < to.y) ? from : to;
			var max = from.x > to.x || (from.x == to.x && from.y > to.y) ? from : to;
			return HashCode.Combine(min, max);
		}
		public int CompareTo(Segment other)
		{
			if(this.Equals(other)) return 0;
			if(length.CompareTo(other.length) != 0) return length.CompareTo(other.length);
			if(from.x.CompareTo(other.from.x) != 0) return from.x.CompareTo(other.from.x);
			if(from.y.CompareTo(other.from.y) != 0) return from.y.CompareTo(other.from.y);
			if(to.x.CompareTo(other.to.x) != 0) return to.x.CompareTo(other.to.x);
			if(to.y.CompareTo(other.to.y) != 0) return to.y.CompareTo(other.to.y);
			return 0;
		}
		
		public static bool operator ==(Segment left, Segment right) => left.Equals(right);
		public static bool operator !=(Segment left, Segment right) => !left.Equals(right);
		
		public static Segment operator +(Segment left, Segment right) => new(left.from + right.from, left.to + right.to);
		public static Segment operator -(Segment left, Segment right) => new(left.from - right.from, left.to - right.to);
		
		// 辅助方法：判断点是否在线段上（包括端点）
		public bool ContainsPoint(Vector2 point, float tolerance = Eps)
		{
			if (length < tolerance) return (point - from).magnitude < tolerance;
			
			float t = Vector2.Dot(point - from, delta) / (length * length);
			if (t < 0 || t > 1) return false;
			
			Vector2 closestPoint = from + t * delta;
			return (point - closestPoint).magnitude < tolerance;
		}
		
		// 辅助方法：判断点是否在线段上（不包括端点）
		public bool ContainsPointExclusive(Vector2 point, float tolerance = Eps)
		{
			if (length < tolerance) return false;
			
			float t = Vector2.Dot(point - from, delta) / (length * length);
			if (t <= 0 || t >= 1) return false;
			
			Vector2 closestPoint = from + t * delta;
			return (point - closestPoint).magnitude < tolerance;
		}
		
		// 辅助方法：获取点到线段的最近点参数t
		public float GetClosestPointParameter(Vector2 point)
		{
			if (length < Eps) return 0;
			return Mathf.Clamp01(Vector2.Dot(point - from, delta) / (length * length));
		}
		
		// 辅助方法：获取点到线段的最近点
		public Vector2 GetClosestPoint(Vector2 point)
		{
			float t = GetClosestPointParameter(point);
			return from + t * delta;
		}
		
		// 获取线段所在直线
		public readonly Line line
		{
			get
			{
				if (isZeroLength)
					throw new ArgumentException("Line不能从零长度Segment创建");
				return new Line(from, to);
			}
		}
		
		
	}
}
