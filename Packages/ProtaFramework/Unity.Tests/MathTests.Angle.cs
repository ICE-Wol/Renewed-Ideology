using System;
using System.Collections.Generic;
using NUnit.Framework;
using Prota;
using UnityEngine;

public static partial class MathTests
{
	public static class AngleTest
	{
		private const float tolerance = 1e-4f;

		[Test]
		public static void AngleDiff180_BasicCases_ReturnsCorrectDifference()
		{
			Assert.AreEqual(30f, 30f.AngleDiff180(60f), tolerance);
			Assert.AreEqual(-30f, 60f.AngleDiff180(30f), tolerance);
			Assert.AreEqual(90f, 0f.AngleDiff180(90f), tolerance);
			Assert.AreEqual(-90f, 90f.AngleDiff180(0f), tolerance);
		}

		[Test]
		public static void AngleDiff180_SameAngle_ReturnsZero()
		{
			Assert.AreEqual(0f, 45f.AngleDiff180(45f), tolerance);
			Assert.AreEqual(0f, 0f.AngleDiff180(0f), tolerance);
			Assert.AreEqual(0f, 180f.AngleDiff180(180f), tolerance);
			Assert.AreEqual(0f, 360f.AngleDiff180(360f), tolerance);
		}

		[Test]
		public static void AngleDiff180_CrossesZeroBoundary_ReturnsShortestPath()
		{
			Assert.AreEqual(20f, 350f.AngleDiff180(10f), tolerance);
			Assert.AreEqual(-20f, 10f.AngleDiff180(350f), tolerance);
			Assert.AreEqual(2f, 359f.AngleDiff180(1f), tolerance);
			Assert.AreEqual(-2f, 1f.AngleDiff180(359f), tolerance);
		}

		[Test]
		public static void AngleDiff180_180DegreeDifference_ReturnsPlusOrMinus180()
		{
			Assert.AreEqual(180f, Mathf.Abs(0f.AngleDiff180(180f)), tolerance);
			Assert.AreEqual(180f, Mathf.Abs(180f.AngleDiff180(0f)), tolerance);
			Assert.AreEqual(180f, Mathf.Abs(90f.AngleDiff180(270f)), tolerance);
			Assert.AreEqual(180f, Mathf.Abs(270f.AngleDiff180(90f)), tolerance);
		}

		[Test]
		public static void AngleDiff180_AnglesGreaterThan360_HandlesCorrectly()
		{
			Assert.AreEqual(0f, 20f.AngleDiff180(380f), tolerance);
			Assert.AreEqual(0f, 380f.AngleDiff180(20f), tolerance);
			Assert.AreEqual(0f, 10f.AngleDiff180(730f), tolerance);
			Assert.AreEqual(0f, 730f.AngleDiff180(10f), tolerance);
			Assert.AreEqual(0f, 350f.AngleDiff180(710f), tolerance);
		}

		[Test]
		public static void AngleDiff180_NegativeAngles_HandlesCorrectly()
		{
			Assert.AreEqual(20f, (-10f).AngleDiff180(10f), tolerance);
			Assert.AreEqual(-20f, 10f.AngleDiff180(-10f), tolerance);
			Assert.AreEqual(-20f, (-350f).AngleDiff180(-10f), tolerance);
			Assert.AreEqual(20f, (-10f).AngleDiff180(-350f), tolerance);
		}

		[Test]
		public static void AngleDiff180_LargeAngleDifference_ChoosesShortestPath()
		{
			Assert.AreEqual(-30f, 20f.AngleDiff180(350f), tolerance);
			Assert.AreEqual(30f, 350f.AngleDiff180(20f), tolerance);
			Assert.AreEqual(170f, 200f.AngleDiff180(10f), tolerance);
			Assert.AreEqual(-170f, 10f.AngleDiff180(200f), tolerance);
		}

		[Test]
		public static void AngleDiff180_NearBoundary_HandlesCorrectly()
		{
			Assert.AreEqual(2f, 179f.AngleDiff180(181f), tolerance);
			Assert.AreEqual(-2f, 181f.AngleDiff180(179f), tolerance);
			Assert.AreEqual(-2f, 1f.AngleDiff180(359f), tolerance);
			Assert.AreEqual(2f, 359f.AngleDiff180(1f), tolerance);
		}

		[Test]
		public static void AngleDiff180_MultipleRotations_HandlesCorrectly()
		{
			Assert.AreEqual(0f, 0f.AngleDiff180(720f), tolerance);
			Assert.AreEqual(0f, 720f.AngleDiff180(0f), tolerance);
			Assert.AreEqual(0f, 10f.AngleDiff180(370f), tolerance);
			Assert.AreEqual(0f, 370f.AngleDiff180(10f), tolerance);
		}

		[Test]
		public static void AngleDiff180_ResultAlwaysInRange_180To180()
		{
			// 测试大量角度组合，确保结果始终在 [-180, 180] 范围内
			for (float from = -720f; from <= 720f; from += 15f)
			{
				for (float to = -720f; to <= 720f; to += 15f)
				{
					float diff = from.AngleDiff180(to);
					Assert.GreaterOrEqual(diff, -180f - tolerance, 
						$"AngleDiff180({from}, {to}) = {diff} should be >= -180, result is not in range [-180, 180]");
					Assert.LessOrEqual(diff, 180f + tolerance, 
						$"AngleDiff180({from}, {to}) = {diff} should be <= 180, result is not in range [-180, 180]");
				}
			}
		}

		[Test]
		public static void AngleDiff180_CommutativeProperty_Consistent()
		{
			// 验证 AngleDiff180(a, b) = -AngleDiff180(b, a) 的性质
			// 注意：当角度差为180度时，由于区间是 [-180, 180)，结果总是-180，因此不满足交换律
			for (float from = -360f; from <= 720f; from += 30f)
			{
				for (float to = -360f; to <= 720f; to += 30f)
				{
					float diff1 = from.AngleDiff180(to);
					float diff2 = to.AngleDiff180(from);
					// 当角度差为180度时，两个方向都返回-180，跳过此情况
					if (Mathf.Abs(Mathf.Abs(diff1) - 180f) < tolerance)
						continue;
					Assert.AreEqual(diff1, -diff2, tolerance, 
						$"AngleDiff180({from}, {to}) should equal -AngleDiff180({to}, {from}) but the result is {diff1} != {-diff2}");
				}
			}
		}

		// ====================================================================================================
		// NormalizeAngle180 测试
		// ====================================================================================================

		[Test]
		public static void NormalizeAngle180_BasicCases_NormalizesToMinus180To180()
		{
			Assert.AreEqual(0f, 0f.NormalizeAngle180(), tolerance);
			Assert.AreEqual(90f, 90f.NormalizeAngle180(), tolerance);
			Assert.AreEqual(-90f, (-90f).NormalizeAngle180(), tolerance);
			Assert.AreEqual(-180f, 180f.NormalizeAngle180(), tolerance); // 180 归一化为 -180
			Assert.AreEqual(-180f, (-180f).NormalizeAngle180(), tolerance);
		}

		[Test]
		public static void NormalizeAngle180_AnglesGreaterThan360_NormalizesCorrectly()
		{
			Assert.AreEqual(0f, 360f.NormalizeAngle180(), tolerance);
			Assert.AreEqual(0f, 720f.NormalizeAngle180(), tolerance);
			Assert.AreEqual(90f, 450f.NormalizeAngle180(), tolerance);
			Assert.AreEqual(-90f, 270f.NormalizeAngle180(), tolerance);
			Assert.AreEqual(10f, 370f.NormalizeAngle180(), tolerance);
			Assert.AreEqual(-10f, 350f.NormalizeAngle180(), tolerance);
		}

		[Test]
		public static void NormalizeAngle180_NegativeAngles_NormalizesCorrectly()
		{
			Assert.AreEqual(0f, (-360f).NormalizeAngle180(), tolerance);
			Assert.AreEqual(0f, (-720f).NormalizeAngle180(), tolerance);
			Assert.AreEqual(90f, (-270f).NormalizeAngle180(), tolerance);
			Assert.AreEqual(-90f, (-90f).NormalizeAngle180(), tolerance);
			Assert.AreEqual(10f, (-350f).NormalizeAngle180(), tolerance);
			Assert.AreEqual(-10f, (-370f).NormalizeAngle180(), tolerance);
		}

		[Test]
		public static void NormalizeAngle180_ResultAlwaysInRange_Minus180To180()
		{
			for (float angle = -1080f; angle <= 1080f; angle += 15f)
			{
				float normalized = angle.NormalizeAngle180();
				Assert.GreaterOrEqual(normalized, -180f - tolerance, 
					$"NormalizeAngle180({angle}) = {normalized} should be >= -180");
				Assert.Less(normalized, 180f + tolerance, 
					$"NormalizeAngle180({angle}) = {normalized} should be < 180");
			}
		}

		[Test]
		public static void NormalizeAngle180_BoundaryCases_HandlesCorrectly()
		{
			Assert.AreEqual(-180f, 180f.NormalizeAngle180(), tolerance);
			Assert.AreEqual(-180f, 540f.NormalizeAngle180(), tolerance);
			Assert.AreEqual(-180f, (-180f).NormalizeAngle180(), tolerance);
			Assert.AreEqual(-180f, (-540f).NormalizeAngle180(), tolerance);
		}

		// ====================================================================================================
		// NormalizeAngle360 测试
		// ====================================================================================================

		[Test]
		public static void NormalizeAngle360_BasicCases_NormalizesToZeroTo360()
		{
			Assert.AreEqual(0f, 0f.NormalizeAngle360(), tolerance);
			Assert.AreEqual(90f, 90f.NormalizeAngle360(), tolerance);
			Assert.AreEqual(270f, (-90f).NormalizeAngle360(), tolerance);
			Assert.AreEqual(180f, 180f.NormalizeAngle360(), tolerance);
			Assert.AreEqual(0f, 360f.NormalizeAngle360(), tolerance);
		}

		[Test]
		public static void NormalizeAngle360_AnglesGreaterThan360_NormalizesCorrectly()
		{
			Assert.AreEqual(0f, 360f.NormalizeAngle360(), tolerance);
			Assert.AreEqual(0f, 720f.NormalizeAngle360(), tolerance);
			Assert.AreEqual(90f, 450f.NormalizeAngle360(), tolerance);
			Assert.AreEqual(270f, 630f.NormalizeAngle360(), tolerance);
			Assert.AreEqual(10f, 370f.NormalizeAngle360(), tolerance);
			Assert.AreEqual(350f, 710f.NormalizeAngle360(), tolerance);
		}

		[Test]
		public static void NormalizeAngle360_NegativeAngles_NormalizesCorrectly()
		{
			Assert.AreEqual(0f, (-360f).NormalizeAngle360(), tolerance);
			Assert.AreEqual(0f, (-720f).NormalizeAngle360(), tolerance);
			Assert.AreEqual(90f, (-270f).NormalizeAngle360(), tolerance);
			Assert.AreEqual(270f, (-90f).NormalizeAngle360(), tolerance);
			Assert.AreEqual(10f, (-350f).NormalizeAngle360(), tolerance);
			Assert.AreEqual(350f, (-10f).NormalizeAngle360(), tolerance);
		}

		[Test]
		public static void NormalizeAngle360_ResultAlwaysInRange_ZeroTo360()
		{
			for (float angle = -1080f; angle <= 1080f; angle += 15f)
			{
				float normalized = angle.NormalizeAngle360();
				Assert.GreaterOrEqual(normalized, 0f - tolerance, 
					$"NormalizeAngle360({angle}) = {normalized} should be >= 0");
				Assert.Less(normalized, 360f + tolerance, 
					$"NormalizeAngle360({angle}) = {normalized} should be < 360");
			}
		}

		[Test]
		public static void NormalizeAngle360_BoundaryCases_HandlesCorrectly()
		{
			Assert.AreEqual(0f, 0f.NormalizeAngle360(), tolerance);
			Assert.AreEqual(0f, 360f.NormalizeAngle360(), tolerance);
			Assert.AreEqual(0f, (-360f).NormalizeAngle360(), tolerance);
			// 接近 360 但不等于 360
			Assert.GreaterOrEqual(359.9f.NormalizeAngle360(), 359.9f - tolerance);
		}

		// ====================================================================================================
		// LerpAngle 测试
		// ====================================================================================================

		[Test]
		public static void LerpAngle_AtZero_ReturnsFrom()
		{
			Assert.AreEqual(0f, (0f, 90f).LerpAngle360(0f), tolerance);
			Assert.AreEqual(45f, (45f, 90f).LerpAngle360(0f), tolerance);
			Assert.AreEqual(180f, (180f, 270f).LerpAngle360(0f), tolerance);
		}

		[Test]
		public static void LerpAngle_AtOne_ReturnsTo()
		{
			Assert.AreEqual(90f, (0f, 90f).LerpAngle360(1f), tolerance);
			Assert.AreEqual(90f, (45f, 90f).LerpAngle360(1f), tolerance);
			Assert.AreEqual(270f, (180f, 270f).LerpAngle360(1f), tolerance);
		}

		[Test]
		public static void LerpAngle_AtHalf_ReturnsMidpoint()
		{
			Assert.AreEqual(45f, (0f, 90f).LerpAngle360(0.5f), tolerance);
			Assert.AreEqual(67.5f, (45f, 90f).LerpAngle360(0.5f), tolerance);
			Assert.AreEqual(225f, (180f, 270f).LerpAngle360(0.5f), tolerance);
		}

		[Test]
		public static void LerpAngle_CrossesZeroBoundary_ChoosesShortestPath()
		{
			// 从 350 到 10，应该走 20 度的路径，而不是 340 度
			float result = (350f, 10f).LerpAngle360(1f);
			Assert.AreEqual(10f, result, tolerance);
			
			// 从 10 到 350，应该走 -20 度的路径
			result = (10f, 350f).LerpAngle360(1f);
			Assert.AreEqual(350f, result, tolerance);
			
			// 中间值
			result = (350f, 10f).LerpAngle360(0.5f);
			Assert.AreEqual(0f, result, tolerance);
		}

		[Test]
		public static void LerpAngle_LargeAngleDifference_ChoosesShortestPath()
		{
			// 从 20 到 350，应该走 -30 度的路径
			float result = (20f, 350f).LerpAngle360(1f);
			Assert.AreEqual(350f, result, tolerance);
			
			// 从 200 到 10，应该走 -170 度的路径
			result = (200f, 10f).LerpAngle360(1f);
			Assert.AreEqual(10f, result, tolerance);
		}

		[Test]
		public static void LerpAngle_180DegreeDifference_HandlesCorrectly()
		{
			// 180 度差，可以选择任意方向
			float result1 = (0f, 180f).LerpAngle360(0.5f);
			Assert.That(result1, Is.EqualTo(90f).Or.EqualTo(270f).Within(tolerance));
			
			float result2 = (180f, 0f).LerpAngle360(0.5f);
			Assert.That(result2, Is.EqualTo(90f).Or.EqualTo(270f).Within(tolerance));
		}

		[Test]
		public static void LerpAngle_AnglesGreaterThan360_HandlesCorrectly()
		{
			// 大于 360 度的角度应该被正确处理
			Assert.AreEqual(20f, (20f, 380f).LerpAngle360(0f), tolerance);
			Assert.AreEqual(20f, (20f, 380f).LerpAngle360(1f), tolerance); // 380 等同于 20
			
			Assert.AreEqual(10f, (370f, 730f).LerpAngle360(0f), tolerance);
			Assert.AreEqual(10f, (370f, 730f).LerpAngle360(1f), tolerance); // 730 等同于 10
		}

		[Test]
		public static void LerpAngle_NegativeAngles_HandlesCorrectly()
		{
			// 负角度应该被正确处理
			Assert.AreEqual(350f, (-10f, 10f).LerpAngle360(0f), tolerance);
			Assert.AreEqual(10f, (-10f, 10f).LerpAngle360(1f), tolerance);
			Assert.AreEqual(0f, (-10f, 10f).LerpAngle360(0.5f), tolerance);
		}

		[Test]
		public static void LerpAngle_InterpolationSteps_Consistent()
		{
			// 测试多个插值步骤，确保结果一致
			for (float t = 0f; t <= 1f; t += 0.1f)
			{
				float result = (0f, 90f).LerpAngle360(t);
				float expected = 0f + (90f - 0f) * t;
				Assert.AreEqual(expected, result, tolerance, 
					$"LerpAngle(0, 90, {t}) should be {expected}");
			}
		}

		[Test]
		public static void LerpAngle_ReverseDirection_Consistent()
		{
			// 验证反向插值的一致性
			for (float t = 0f; t <= 1f; t += 0.25f)
			{
				float forward = (0f, 90f).LerpAngle360(t);
				float reverse = (90f, 0f).LerpAngle360(1f - t);
				Assert.AreEqual(forward, reverse, tolerance, 
					$"LerpAngle(0, 90, {t}) should equal LerpAngle(90, 0, {1f - t})");
			}
		}
	}
}

