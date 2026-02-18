using NUnit.Framework;
using Prota.Unity;
using UnityEngine;

public static partial class MathTests
{
	public static class Interop4DirLenTest
	{
		
		static float BruteForceInterop4DirLen(Vector2 d, float l, float r, float b, float t)
		{
			const float eps = 1e-6f;
			if (d.sqrMagnitude < eps * eps) return 0f;
			var dir = d.normalized;
			float xr = dir.x >= 0 ? r : l;
			float yr = dir.y >= 0 ? t : b;

			// Binary search max length along dir so that (x/xr)^2 + (y/yr)^2 <= 1
			float lo = 0f;
			float hi = 1000f;
			for (int i = 0; i < 64; i++)
			{
				float mid = (lo + hi) * 0.5f;
				Vector2 p = dir * mid;
				float v = (p.x * p.x) / (xr * xr) + (p.y * p.y) / (yr * yr);
				if (v <= 1f) lo = mid; else hi = mid;
			}
			return lo;
		}

		[Test]
		public static void Interop4DirLen_ZeroVector_ReturnsZero()
		{
			float l = 3, r = 4, b = 5, t = 6;
			Assert.AreEqual(0f, Vector2.zero.Interop4DirLen(l, r, b, t));
		}

		[Test]
		public static void Interop4DirLen_AxisAligned_DoesNotExceedCardinals()
		{
			float l = 2f, r = 3f, b = 4f, t = 5f;
			// Use unit directions for fair comparison
			Assert.AreEqual(r, Vector2.right.Interop4DirLen(l, r, b, t), 1e-5f);
			Assert.AreEqual(l, Vector2.left.Interop4DirLen(l, r, b, t), 1e-5f);
			Assert.AreEqual(t, Vector2.up.Interop4DirLen(l, r, b, t), 1e-5f);
			Assert.AreEqual(b, Vector2.down.Interop4DirLen(l, r, b, t), 1e-5f);
		}

		[Test]
		public static void Interop4DirLen_RandomDirections_MatchBruteForce()
		{
			var rng = new System.Random(12345);
			for (int i = 0; i < 200; i++)
			{
				// Random anisotropic limits in [0.5, 8]
				float l = 0.5f + (float)rng.NextDouble() * 7.5f;
				float r = 0.5f + (float)rng.NextDouble() * 7.5f;
				float b = 0.5f + (float)rng.NextDouble() * 7.5f;
				float t = 0.5f + (float)rng.NextDouble() * 7.5f;

				// Random direction on unit circle
				float angle = (float)rng.NextDouble() * Mathf.PI * 2f;
				var d = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
				d *= (float)rng.NextDouble() * 10f;

				float expected = BruteForceInterop4DirLen(d, l, r, b, t);
				float got = d.Interop4DirLen(l, r, b, t);

				Assert.AreEqual(expected, got, 1e-3f, $"Mismatch at i={i}, d={d}, limits=({l},{r},{b},{t})");
			}
		}

		[Test]
		public static void Interop4DirLen_AxisAligned_WithVariousMagnitudes()
		{
			float l = 2f, r = 3f, b = 4f, t = 5f;
			Assert.AreEqual(r, (Vector2.right * 10f).Interop4DirLen(l, r, b, t), 1e-5f);
			Assert.AreEqual(l, (Vector2.left * 123f).Interop4DirLen(l, r, b, t), 1e-5f);
			Assert.AreEqual(t, (Vector2.up * 0.0001f).Interop4DirLen(l, r, b, t), 1e-5f);
			Assert.AreEqual(b, (Vector2.down * 9999f).Interop4DirLen(l, r, b, t), 1e-5f);
		}

		[Test]
		public static void Interop4DirLen_AxisAligned_SignedDirections()
		{
			float l = 7f, r = 11f, b = 13f, t = 17f;
			Assert.AreEqual(t, new Vector2(0f, 42f).Interop4DirLen(l, r, b, t), 1e-6f);
			Assert.AreEqual(b, new Vector2(0f, -0.5f).Interop4DirLen(l, r, b, t), 1e-6f);
			Assert.AreEqual(r, new Vector2(3.14f, 0f).Interop4DirLen(l, r, b, t), 1e-6f);
			Assert.AreEqual(l, new Vector2(-2.71f, 0f).Interop4DirLen(l, r, b, t), 1e-6f);
		}

		[Test]
		public static void Interop4DirLen_NegativeParameters_ThrowsException()
		{
			var testVector = new Vector2(1f, 1f);
			float validL = 2f, validR = 3f, validB = 4f, validT = 5f;

			// Test l < 0
			Assert.Throws<System.Exception>(() => testVector.Interop4DirLen(-1f, validR, validB, validT));
			Assert.Throws<System.Exception>(() => testVector.Interop4DirLen(-0.1f, validR, validB, validT));

			// Test r < 0
			Assert.Throws<System.Exception>(() => testVector.Interop4DirLen(validL, -1f, validB, validT));
			Assert.Throws<System.Exception>(() => testVector.Interop4DirLen(validL, -0.5f, validB, validT));

			// Test b < 0
			Assert.Throws<System.Exception>(() => testVector.Interop4DirLen(validL, validR, -1f, validT));
			Assert.Throws<System.Exception>(() => testVector.Interop4DirLen(validL, validR, -0.01f, validT));

			// Test t < 0
			Assert.Throws<System.Exception>(() => testVector.Interop4DirLen(validL, validR, validB, -1f));
			Assert.Throws<System.Exception>(() => testVector.Interop4DirLen(validL, validR, validB, -100f));

			// Test multiple negative parameters
			Assert.Throws<System.Exception>(() => testVector.Interop4DirLen(-1f, -2f, validB, validT));
			Assert.Throws<System.Exception>(() => testVector.Interop4DirLen(validL, validR, -1f, -2f));
			Assert.Throws<System.Exception>(() => testVector.Interop4DirLen(-1f, validR, -2f, validT));
			Assert.Throws<System.Exception>(() => testVector.Interop4DirLen(-1f, -2f, -3f, -4f));
		}

		[Test]
		public static void Interop4DirLen_ZeroParameters_WorksCorrectly()
		{
			// Zero parameters should be allowed (check is < 0, not <= 0)
			float l = 0f, r = 3f, b = 4f, t = 5f;
			var testVector = new Vector2(1f, 0f);
			Assert.AreEqual(3f, testVector.Interop4DirLen(l, r, b, t), 1e-6f);

			l = 2f; r = 0f;
			testVector = new Vector2(1f, 0f);
			Assert.AreEqual(0f, testVector.Interop4DirLen(l, r, b, t), 1e-6f);

			l = 2f; r = 3f; b = 0f; t = 5f;
			testVector = new Vector2(0f, 1f);
			Assert.AreEqual(5f, testVector.Interop4DirLen(l, r, b, t), 1e-6f);

			l = 2f; r = 3f; b = 4f; t = 0f;
			testVector = new Vector2(0f, 1f);
			Assert.AreEqual(0f, testVector.Interop4DirLen(l, r, b, t), 1e-6f);

			// All zeros
			l = 0f; r = 0f; b = 0f; t = 0f;
			testVector = new Vector2(1f, 1f);
			Assert.AreEqual(0f, testVector.Interop4DirLen(l, r, b, t), 1e-6f);
		}
	}
}
