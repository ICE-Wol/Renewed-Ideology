using NUnit.Framework;
using Prota.Unity;
using UnityEngine;

public static partial class MathTests
{
		
	public static class RectTest
	{
		[Test]
		public static void TestUnityRect_Contains()
		{
			Rect rect = new Rect(0, 0, 10, 5);
			Assert.IsTrue(rect.Contains(new Vector2(5, 2)));
			Assert.IsFalse(rect.Contains(new Vector2(15, 2)));
			Assert.IsFalse(rect.Contains(new Vector2(5, 7)));
			Assert.IsFalse(rect.Contains(new Vector2(-5, 2)));
			Assert.IsFalse(rect.Contains(new Vector2(5, -2)));
			Assert.IsFalse(rect.Contains(new Vector2(15, 7)));
			Assert.IsFalse(rect.Contains(new Vector2(-5, 7)));
			Assert.IsFalse(rect.Contains(new Vector2(-5, -2)));
			
			Assert.IsTrue(rect.ContainsInclusive(Vector2.zero));
			Assert.IsFalse(rect.ContainsExclusive(Vector2.zero));
			Assert.IsTrue(rect.ContainsInclusive(new Vector2(10, 5)));
			Assert.IsFalse(rect.ContainsExclusive(new Vector2(10, 5)));
			Assert.IsTrue(rect.ContainsInclusive(new Vector2(0, 0)));
			Assert.IsFalse(rect.ContainsExclusive(new Vector2(0, 0)));
			Assert.IsTrue(rect.ContainsInclusive(new Vector2(10, 0)));
			Assert.IsFalse(rect.ContainsExclusive(new Vector2(10, 0)));
			Assert.IsTrue(rect.ContainsInclusive(new Vector2(0, 5)));
			Assert.IsFalse(rect.ContainsExclusive(new Vector2(0, 5)));
		}
		
		[Test]
		public static void TestRayFromInsideHitsBoundary()
		{
			Rect rect = new Rect(0, 0, 10, 5);

			// 从内部发射到右边
			Vector2 origin = new Vector2(5, 2);
			Vector2 dir = Vector2.right;
			bool hit = rect.CastFromInside(origin, dir, out Vector2 hitPoint);
			Assert.IsTrue(hit);
			Assert.AreEqual(new Vector2(10, 2), hitPoint);

			// 从内部发射到上边
			origin = new Vector2(3, 1);
			dir = Vector2.up;
			hit = rect.CastFromInside(origin, dir, out hitPoint);
			Assert.IsTrue(hit);
			Assert.AreEqual(new Vector2(3, 5), hitPoint);
		}

		[Test]
		public static void TestRayFromInsideDiagonal()
		{
			Rect rect = new Rect(0, 0, 10, 5);

			Vector2 origin = new Vector2(2, 2);
			Vector2 dir = new Vector2(1, 1);
			bool hit = rect.CastFromInside(origin, dir, out Vector2 hitPoint);
			Assert.IsTrue(hit);
			// 射线会先到达上边 y=5
			Assert.AreEqual(new Vector2(5, 5), hitPoint);
		}

		[Test]
		public static void TestRayFromBoundary()
		{
			Rect rect = new Rect(0, 0, 10, 5);

			// 起点在左边界
			Vector2 origin = new Vector2(0, 2);
			Vector2 dir = Vector2.left;
			bool hit = rect.CastFromInside(origin, dir, out Vector2 hitPoint);
			Assert.IsTrue(hit);
			// 结果应该仍在起点
			Assert.AreEqual(origin, hitPoint);

			// 起点在上边界
			origin = new Vector2(4, 5);
			dir = Vector2.up;
			hit = rect.CastFromInside(origin, dir, out hitPoint);
			Assert.IsTrue(hit);
			Assert.AreEqual(origin, hitPoint);
		}

		[Test]
		public static void TestRayZeroDirection()
		{
			Rect rect = new Rect(0, 0, 10, 5);

			Vector2 origin = new Vector2(5, 2);
			Vector2 dir = Vector2.zero;
			bool hit = rect.CastFromInside(origin, dir, out Vector2 hitPoint);
			// 射线无方向, 没有结果.
			Assert.IsFalse(hit);
		}
	
	}
}