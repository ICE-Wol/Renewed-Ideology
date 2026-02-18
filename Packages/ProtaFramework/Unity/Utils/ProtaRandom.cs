using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Prota.Unity
{
    public static class ProtaRandom
    {
		/// <summary>
		/// 将非负浮点数视为区间段长度, 返回一个整数, 其期望值为输入值。
		/// 例如 x=2.3 时, 以 0.3 概率返回 3, 否则返回 2。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SegmentRandom(this float x)
		{
			(x >= 0).Assert();
			var b = x.FloorToInt();
			x -= b;
			return b + (UnityEngine.Random.Range(0f, 1f) <= x ? 1 : 0);
		}

		/// <summary>
		/// 返回 [a, b) 范围内的整数随机数。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Random(int a, int b)
		{
			return UnityEngine.Random.Range(a, b);
		}
		
		/// <summary>
		/// 返回 [a, b] 范围内的整数随机数。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int RandomInclusive(int a, int b)
		{
			return UnityEngine.Random.Range(a, b + 1);
		}
        
		/// <summary>
		/// 返回 [a, b) 范围内的浮点随机数。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Random(float a, float b)
        {
            return UnityEngine.Random.Range(a, b);
        }
        
		/// <summary>
		/// 分量分别在 [a, b) 范围内随机的 Vector2Int。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int Random(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(
                Random(a.x, b.x),
                Random(a.y, b.y)
            );
        }
        
		/// <summary>
		/// 分量分别在 [a, b) 范围内随机的 Vector3Int。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Int Random(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(
                Random(a.x, b.x),
                Random(a.y, b.y),
                Random(a.z, b.z)
            );
        }
        
		/// <summary>
		/// 分量分别在 [a, b] 范围内随机的 Vector2 f。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Random(Vector2 a, Vector2 b)
        {
            return new Vector2(
                Random(a.x, b.x),
                Random(a.y, b.y)
            );
        }
        
		/// <summary>
		/// 分量分别在 [a, b] 范围内随机的 Vector3 f。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Random(Vector3 a, Vector3 b)
        {
            return new Vector3(
                Random(a.x, b.x),
                Random(a.y, b.y),
                Random(a.z, b.z)
            );
        }
        
		/// <summary>
		/// 分量分别在 [a, b) 范围内随机的 Vector4。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Random(Vector4 a, Vector4 b)
        {
            return new Vector4(
                Random(a.x, b.x),
                Random(a.y, b.y),
                Random(a.z, b.z),
                Random(a.w, b.w)
            );
        }
        
		/// <summary>
		/// 分量分别在 [a, b) 范围内随机的 Color。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Random(Color a, Color b)
        {
            return new Color(
                Random(a.r, b.r),
                Random(a.g, b.g),
                Random(a.b, b.b),
                Random(a.a, b.a)
            );
        }
        
		/// <summary>
		/// 以 a 为圆心, 在半径 radius 的圆内均匀随机一个点。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RandomCircle(Vector2 a, float radius)
        {
            var angle = Random(90, 360f) * Mathf.Rad2Deg;
            return Vector2.one.Rotate(angle) * Random(0f, radius).Sqrt() + a;
        }
        
		/// <summary>
		/// 以 a 为圆心, 在半径 radius 的圆周上均匀随机一个点。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RandomCircleEdge(Vector2 a, float radius)
        {
            var angle = Random(90, 360f) * Mathf.Rad2Deg;
            return Vector2.one.Rotate(angle) * radius + a;
        }
        
		/// <summary>
		/// 以 a 为中心, 在距离 dist 的球体内均匀随机一个点。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 RandomSphere(Vector3 a, float dist)
        {
            var q = Quaternion.Euler(
                Random(0, 360f),
                Random(0, 360f),
                Random(0, 360f)
            );
            return q * Vector3.one * Random(0f, dist).Pow(1/3f) + a;
        }

		/// <summary>
		/// 以 a 为中心, 在距离 dist 的球面上均匀随机一个点。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 RandomSphereSurface(Vector3 a, float dist)
		{
			var q = Quaternion.Euler(
				Random(0, 360f),
				Random(0, 360f),
				Random(0, 360f)
			);
			return q * Vector3.one * dist + a;
		}
		
		
		/// <summary>
		/// 从列表中等概率随机选择一个元素。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomSelect<T>(this IReadOnlyList<T> e)
        {
            var s = e.Count;
            var index = Random(0, s);
			return e[index];
        }
        
		/// <summary>
		/// 按权重随机选择一个元素, weight 返回值小于等于 0 的元素会被忽略。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T RandomSelect<T>(this IReadOnlyList<T> e, Func<T, float> weight)
		{
            var count = e.Count;
            if(count == 0) return default;
            if(count == 1)
            {
                var w = weight(e[0]);
                if(w > 0) return e[0];
                throw new Exception("weight sum is zero.");
            }

            var sum = 0f;
            for(var i = 0; i < count; i++)
            {
                var w = weight(e[i]);
                if(w <= 0) continue;
                sum += w;
            }

            var ss = Random(0f, sum);
            for(var i = 0; i < count; i++)
            {
                var w = weight(e[i]);
                if(w <= 0) continue;
                ss -= w;
                if(ss <= 0) return e[i];
            }
            throw new Exception("weight sum is zero.");
        }

		/// <summary>
		/// 以随机游走方法将 total 分成 count 份, 且每份在 [min, max] 范围内.
		/// 先构造一个可行解, 再执行 iterations 次随机转移: 每次随机选两个位置, 从一个位置转移一定数量到另一个位置.
		/// 注意: 该方法不保证在方案空间上均匀采样, 只提供可控的随机扰动.
		/// 若在 min/max 限制下无解, 则抛出异常.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int[] RandomDistribute<T>(int total, int count, int min, int max)
		{
			var result = new int[count];
			RandomDistributeFill<T>(total, count, min, max, result);
			return result;
		}

		static void ValidateRandomDistributeArgs(int total, int count, int min, int max)
		{
			if(min > max) throw new ArgumentException($"min[{min}] must be less than max[{max}]");
			if(count <= 0) throw new ArgumentException($"count[{count}] must be greater than 0");

			var minSum = (long)count * min;
			var maxSum = (long)count * max;
			if(total < minSum || total > maxSum)
				throw new Exception($"total[{total}] cannot be distributed to count[{count}] with min[{min}] max[{max}]");
		}

		static void RandomDistributeFill<T>(int total, int count, int min, int max, IList<int> result)
		{
			ValidateRandomDistributeArgs(total, count, min, max);
			RandomDistributeFillUnchecked(total, count, min, max, result);
		}

		static void RandomDistributeFillUnchecked(
			int total, int count, int min, int max, IList<int> result)
		{
			int sum = total - count * min;
			int range = max - min;

			if (count <= 0)
				return;

			if (sum <= 0)
			{
				for (int i = 0; i < count; i++)
					result[i] = min;
				return;
			}

			// 1. 生成随机切点
			var cuts = new int[count + 1];
			cuts[0] = 0;
			cuts[count] = sum;

			for (int i = 1; i < count; i++)
				cuts[i] = UnityEngine.Random.Range(0, sum + 1);

			System.Array.Sort(cuts);

			// 2. 差分得到分配
			int overflow = 0;
			for (int i = 0; i < count; i++)
			{
				int v = cuts[i + 1] - cuts[i];

				if (v > range)
				{
					overflow += v - range;
					v = range;
				}

				result[i] = v;
			}

			// 3. 把溢出随机回填
			while (overflow > 0)
			{
				int i = UnityEngine.Random.Range(0, count);
				int canAdd = range - result[i];
				if (canAdd <= 0) continue;

				int add = Mathf.Min(canAdd, overflow);
				result[i] += add;
				overflow -= add;
			}

			// 4. 加回 min
			for (int i = 0; i < count; i++)
				result[i] += min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void RandomDistribute<T>(int total, int count, int min, int max, List<int> result)
		{
			ValidateRandomDistributeArgs(total, count, min, max);
			result.Clear();
			if(result.Capacity < count) result.Capacity = count;
			while(result.Count < count) result.Add(default);
			RandomDistributeFillUnchecked(total, count, min, max, result);
		}
        
		
		public static float NextFloat(this System.Random rng, float min, float max)
        {
            return min + (float)rng.NextDouble() * (max - min);	
        }
		
		public static double NextDouble(this System.Random rng, double min, double max)
        {
            return min + rng.NextDouble() * (max - min);
        }
		
    }
}
