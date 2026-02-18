using System;
using UnityEngine;

namespace Prota.Unity
{


	public struct Line : IEquatable<Line>
	{
		private const float Eps = 1e-6f;
		
		public Vector2 point;
		public Vector2 direction;
		
		// 从两点创建Line
		public Line(Vector2 from, Vector2 to)
		{
			Vector2 dir = to - from;
			if (dir.magnitude < Eps)
				throw new ArgumentException("Line的两点不能重合");
			this.point = from;
			this.direction = dir;
		}
		
		// 从Segment创建Line
		public Line(Segment segment)
		{
			if (segment.length < Eps)
				throw new ArgumentException("Line不能从零长度Segment创建");
			this.point = segment.from;
			this.direction = segment.delta;
		}
		
		// 验证direction是否有效
		private readonly void ValidateDirection()
		{
			if (direction.magnitude < Eps)
				throw new InvalidOperationException("Line的direction不能为零向量");
		}
		
		// 获取线段（从point开始，沿着direction延伸单位长度）
		public readonly Segment segment
		{
			get
			{
				ValidateDirection();
				return new Segment(point, point + direction);
			}
		}
		
		public bool Equals(Line other)
		{
			ValidateDirection();
			if (other.direction.magnitude < Eps)
				throw new ArgumentException("other Line的direction不能为零向量");
			
			// 两条直线相等当且仅当它们共线且通过同一个点
			if (!IsCollinearWith(other)) return false;
			
			// 检查点是否在另一条直线上
			Vector2 d = other.point - this.point;
			float cross = this.direction.x * d.y - this.direction.y * d.x;
			return MathF.Abs(cross) <= Eps;
		}
		
		public override bool Equals(object obj)
		{
			return obj is Line other && Equals(other);
		}
		
		public override int GetHashCode()
		{
			return HashCode.Combine(point, direction);
		}
		
		public static bool operator ==(Line left, Line right) => left.Equals(right);
		public static bool operator !=(Line left, Line right) => !left.Equals(right);
		
		// 判断两条直线是否平行
		public bool IsParallelWith(Line other)
		{
			ValidateDirection();
			if (other.direction.magnitude < Eps)
				throw new ArgumentException("other Line的direction不能为零向量");
			
			// 计算叉积，如果接近0则平行
			float cross = direction.x * other.direction.y - direction.y * other.direction.x;
			return MathF.Abs(cross) <= Eps;
		}
		
		// 判断两条直线是否共线
		public bool IsCollinearWith(Line other, float epsilon = Eps)
		{
			ValidateDirection();
			if (other.direction.magnitude < Eps)
				throw new ArgumentException("other Line的direction不能为零向量");
			
			// 平行检测
			if (!IsParallelWith(other))
				return false;
			
			// 位置检测（other的点是否在当前直线上）
			Vector2 d = other.point - this.point;
			float cross = direction.x * d.y - direction.y * d.x;
			return MathF.Abs(cross) <= epsilon;
		}
		
		// 判断两条直线是否垂直
		public bool IsPerpendicular(Line other)
		{
			ValidateDirection();
			if (other.direction.magnitude < Eps)
				throw new ArgumentException("other Line的direction不能为零向量");
			
			// 计算点积，如果接近0则垂直
			float dot = Vector2.Dot(direction, other.direction);
			return MathF.Abs(dot) <= Eps;
		}
		
	}

}
