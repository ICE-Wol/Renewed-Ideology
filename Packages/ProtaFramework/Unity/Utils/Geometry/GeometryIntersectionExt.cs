using System;
using UnityEngine;

namespace Prota.Unity
{

	public static class GeometryIntersectionExt
	{
		private const float Eps = 1e-6f;
		
		/// <summary>
		/// 验证Line的direction是否有效
		/// </summary>
		private static void ValidateDirection(this Line line)
		{
			if (line.direction.magnitude < Eps)
				throw new InvalidOperationException("Line的direction不能为零向量");
		}
		
		#region Line 相交判定
		
		/// <summary>
		/// 判断两条直线是否相交，并返回交点
		/// </summary>
		public static bool IntersectsWith(this Line line, Line other, out Vector2 intersection)
		{
			line.ValidateDirection();
			if (other.direction.magnitude < Eps)
				throw new ArgumentException("other Line的direction不能为零向量");
			
			intersection = Vector2.zero;
			
			Vector2 d1 = line.direction;
			Vector2 d2 = other.direction;
			Vector2 r = other.point - line.point;
			
			float cross = d1.x * d2.y - d1.y * d2.x;
			
			// 平行线处理
			if (Mathf.Abs(cross) < Eps)
			{
				// 平行线，检查是否重合
				if (Mathf.Abs((r.x * d1.y - r.y * d1.x)) < Eps)
				{
					// 重合，返回任意一点
					intersection = line.point;
					return true;
				}
				return false;
			}
			
			// 计算交点参数 t，使得 point + t * d1 = other.point + u * d2
			// 解方程组：point + t * d1 = other.point + u * d2
			// 即：t * d1 - u * d2 = other.point - point = -r
			// 使用克莱姆法则求解
			float t = (r.x * d2.y - r.y * d2.x) / cross;
			intersection = line.point + t * d1;
			return true;
		}
		
		#endregion
		
		#region Segment 相交判定
		
		/// <summary>
		/// 获取共线线段的交点
		/// </summary>
		private static bool GetCollinearIntersection(this Segment segment, Segment other, out Vector2 intersection)
		{
			intersection = Vector2.zero;
			
			if (segment.isZeroLength)
			{
				// 当前线段是点
				if (other.ContainsPoint(segment.from))
				{
					intersection = segment.from;
					return true;
				}
				return false;
			}
			
			if (other.isZeroLength)
			{
				// 另一条线段是点
				if (segment.ContainsPoint(other.from))
				{
					intersection = other.from;
					return true;
				}
				return false;
			}
			
			// 计算两个线段在直线上的投影范围
			Vector2 d = segment.delta.normalized;
			float t1 = Vector2.Dot(segment.from - segment.from, d);
			float t2 = Vector2.Dot(segment.to - segment.from, d);
			float t3 = Vector2.Dot(other.from - segment.from, d);
			float t4 = Vector2.Dot(other.to - segment.from, d);
			
			float tMin1 = Mathf.Min(t1, t2);
			float tMax1 = Mathf.Max(t1, t2);
			float tMin2 = Mathf.Min(t3, t4);
			float tMax2 = Mathf.Max(t3, t4);
			
			// 检查是否有重叠
			float overlapStart = Mathf.Max(tMin1, tMin2);
			float overlapEnd = Mathf.Min(tMax1, tMax2);
			
			if (overlapStart <= overlapEnd)
			{
				// 有重叠，返回重叠区域的中心点
				float overlapCenter = (overlapStart + overlapEnd) / 2;
				intersection = segment.from + overlapCenter * d;
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// 判断两条线段是否共线
		/// </summary>
		private static bool IsCollinearWith(this Segment segment, Segment other, float epsilon = 1e-6f)
		{
			// 如果两条线段都是零长度，检查它们是否是同一个点
			if (segment.isZeroLength && other.isZeroLength)
			{
				return (segment.from - other.from).magnitude < epsilon;
			}
			
			// 如果其中一条是零长度，检查点是否在另一条线段所在的直线上
			// 使用叉积判断：如果点在线段所在的直线上，叉积应该接近0
			if (segment.isZeroLength)
			{
				if (other.isZeroLength) return false; // 已经在上面处理了
				Vector2 d = other.delta;
				Vector2 p = segment.from - other.from;
				float cross = d.x * p.y - d.y * p.x;
				return Mathf.Abs(cross) < epsilon;
			}
			
			if (other.isZeroLength)
			{
				Vector2 d = segment.delta;
				Vector2 p = other.from - segment.from;
				float cross = d.x * p.y - d.y * p.x;
				return Mathf.Abs(cross) < epsilon;
			}
			
			return segment.line.IsCollinearWith(other.line, epsilon);
		}
		
		/// <summary>
		/// 判断两条线段是否相交（包括端点），并返回交点
		/// </summary>
		public static bool IntersectsWith(this Segment segment, Segment other, out Vector2 intersection)
		{
			if (segment.IsCollinearWith(other))
			{
				return segment.GetCollinearIntersection(other, out intersection);
			}
			
			// 直线没有交点...直接返回false
			if (!segment.IntersectsLineWith(other, out intersection))
			{
				return false;
			}
			
			// 检查交点是否在两个线段上（包括端点）
			return segment.ContainsPoint(intersection) && other.ContainsPoint(intersection);
		}
		
		/// <summary>
		/// 判断两条线段是否相交（不包括端点），并返回交点
		/// </summary>
		public static bool IntersectsWithExclusive(this Segment segment, Segment other, out Vector2 intersection)
		{
			intersection = Vector2.zero;
			
			// 先计算直线交点
			if (!segment.IntersectsLineWith(other, out intersection))
			{
				return false;
			}
			
			// 检查交点是否在两个线段上（不包括端点）
			return segment.ContainsPointExclusive(intersection) && other.ContainsPointExclusive(intersection);
		}
		
		/// <summary>
		/// 判断线段所在直线是否与另一条线段所在直线相交，并返回交点
		/// </summary>
		public static bool IntersectsLineWith(this Segment segment, Segment other, out Vector2 intersection)
		{
			// 如果其中一条线段是零长度，无法确定直线方向
			if (segment.isZeroLength || other.isZeroLength)
			{
				intersection = Vector2.zero;
				return false;
			}
			
			return segment.line.IntersectsWith(other.line, out intersection);
		}
		
		/// <summary>
		/// 判断线段所在直线是否与另一条直线相交，并返回交点
		/// </summary>
		public static bool IntersectsLineWith(this Segment segment, Line line, out Vector2 intersection)
		{
			// 如果线段是零长度，无法确定直线方向
			if (segment.isZeroLength)
			{
				intersection = Vector2.zero;
				return false;
			}
			
			return segment.line.IntersectsWith(line, out intersection);
		}
		
		/// <summary>
		/// 判断线段是否与另一条线段所在直线相交（包括端点），并返回交点
		/// </summary>
		public static bool IntersectsSegmentWithLine(this Segment segment, Segment line, out Vector2 intersection)
		{
			intersection = Vector2.zero;
			
			// 先计算直线交点
			if (!segment.IntersectsLineWith(line, out intersection))
			{
				return false;
			}
			
			// 检查交点是否在当前线段上（包括端点）
			return segment.ContainsPoint(intersection);
		}
		
		/// <summary>
		/// 判断线段是否与直线相交（包括端点），并返回交点
		/// </summary>
		public static bool IntersectsSegmentWithLine(this Segment segment, Line line, out Vector2 intersection)
		{
			intersection = Vector2.zero;
			
			// 先计算直线交点
			if (!segment.IntersectsLineWith(line, out intersection))
			{
				return false;
			}
			
			// 检查交点是否在当前线段上（包括端点）
			return segment.ContainsPoint(intersection);
		}
		
		/// <summary>
		/// 判断线段是否与另一条线段所在直线相交（不包括端点），并返回交点
		/// </summary>
		public static bool IntersectsSegmentWithLineExclusive(this Segment segment, Segment line, out Vector2 intersection)
		{
			intersection = Vector2.zero;
			
			// 先计算直线交点
			if (!segment.IntersectsLineWith(line, out intersection))
			{
				return false;
			}
			
			// 检查交点是否在当前线段上（不包括端点）
			return segment.ContainsPointExclusive(intersection);
		}
		
		/// <summary>
		/// 判断线段是否与直线相交（不包括端点），并返回交点
		/// </summary>
		public static bool IntersectsSegmentWithLineExclusive(this Segment segment, Line line, out Vector2 intersection)
		{
			intersection = Vector2.zero;
			
			// 先计算直线交点
			if (!segment.IntersectsLineWith(line, out intersection))
			{
				return false;
			}
			
			// 检查交点是否在当前线段上（不包括端点）
			return segment.ContainsPointExclusive(intersection);
		}
		
		#endregion
		
		#region Triangle 相交判定
		
		/// <summary>
		/// 判断三角形是否与线段相交，并返回交点
		/// </summary>
		public static bool IntersectsWith(this Triangle triangle, Segment segment, out Vector2 intersection)
		{
			intersection = Vector2.zero;
			
			// 检查线段是否与三角形的三条边相交
			if (new Segment(triangle.a, triangle.b).IntersectsWith(segment, out intersection))
				return true;
			if (new Segment(triangle.b, triangle.c).IntersectsWith(segment, out intersection))
				return true;
			if (new Segment(triangle.c, triangle.a).IntersectsWith(segment, out intersection))
				return true;
			
			// 检查线段是否完全在三角形内部
			if (triangle.ContainsPoint(segment.from) && triangle.ContainsPoint(segment.to))
			{
				intersection = segment.center;
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// 判断两个三角形是否相交，并返回交点
		/// </summary>
		public static bool IntersectsWith(this Triangle triangle, Triangle other, out Vector2 intersection)
		{
			intersection = Vector2.zero;
			
			// 检查是否有顶点在对方内部
			if (triangle.ContainsPoint(other.a))
			{
				intersection = other.a;
				return true;
			}
			if (triangle.ContainsPoint(other.b))
			{
				intersection = other.b;
				return true;
			}
			if (triangle.ContainsPoint(other.c))
			{
				intersection = other.c;
				return true;
			}
			if (other.ContainsPoint(triangle.a))
			{
				intersection = triangle.a;
				return true;
			}
			if (other.ContainsPoint(triangle.b))
			{
				intersection = triangle.b;
				return true;
			}
			if (other.ContainsPoint(triangle.c))
			{
				intersection = triangle.c;
				return true;
			}
			
			// 检查边是否相交
			Segment[] edges1 = { new Segment(triangle.a, triangle.b), new Segment(triangle.b, triangle.c), new Segment(triangle.c, triangle.a) };
			Segment[] edges2 = { new Segment(other.a, other.b), new Segment(other.b, other.c), new Segment(other.c, other.a) };
			
			foreach (var edge1 in edges1)
			{
				foreach (var edge2 in edges2)
				{
					if (edge1.IntersectsWith(edge2, out intersection))
						return true;
				}
			}
			
			return false;
		}
		
		#endregion
	}

}
