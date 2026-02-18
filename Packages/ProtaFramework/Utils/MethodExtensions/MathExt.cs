using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Prota
{
    public static partial class MethodExtensions
    {
        // x: inited.
        // 用法: if(!a.inited.NeedInit()) return;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NeedInit(this ref bool x)
        {
            if(x) return false;
            x = true;
            return true;
        }
        
        // 如果不一样就返回 false. 否则返回 true.
        // 最终的值会设置为 current.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareAndReplace<T>(this ref T record, T current) where T: struct
        {
            var res = EqualityComparer<T>.Default.Equals(record, current);
            record = current;
            return res;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CompareChanged<T>(this T record, T current, out bool changed)
        {
            changed = EqualityComparer<T>.Default.Equals(record, current);
            return changed ? current : record;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LinearStep(this float x, float a, float b) => ((x - a) / (b - a)).Clamp(0, 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LinearStep(this double x, double a, double b) => ((x - a) / (b - a)).Clamp(0, 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LinearStepRev(this float x, float a, float b) => 1 - ((x - a) / (b - a)).Clamp(0, 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LinearStepRev(this double x, double a, double b) => 1 - ((x - a) / (b - a)).Clamp(0, 1);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmoothStep(this float x, float a, float b)
        {
            var t = ((x - a) / (b - a)).Clamp(0, 1);
            return t * t * (3 - 2 * t);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SmoothStep(this double x, double a, double b)
        {
            var t = ((x - a) / (b - a)).Clamp(0, 1);
            return t * t * (3 - 2 * t);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyEqual(this float x, float y, float eps = 1e-6f) => Math.Abs(x - y) < eps;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproximatelyEqual(this double x, double y, double eps = 1e-6f) => Math.Abs(x - y) < eps;
        
		
		
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this float x) => (int)x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this double x) => (int)x;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat(this int x) => (float)x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToFloat(this double x) => (float)x;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDouble(this int x) => (double)x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDouble(this float x) => (double)x;
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqr(this float x) => x * x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sqr(this int x) => x * x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sqr(this double x) => x * x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Sqr(this long x) => x * x;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cube(this float x) => x * x * x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Cube(this int x) => x * x * x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cube(this double x) => x * x * x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Cube(this long x) => x * x * x;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int x) => Math.Abs(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Abs(this long x) => Math.Abs(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Abs(this double x) => Math.Abs(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(this float x) => Math.Abs(x);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sin(this double x) => Math.Sin(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cos(this double x) => Math.Cos(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Tan(this double x) => Math.Tan(x);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MoveTo(this int x, int target, int d) => x < target ? (x + d).Min(target) : (x - d).Max(target);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MoveTo(this long x, long target, long d) => x < target ? (x + d).Min(target) : (x - d).Max(target);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MoveTo(this float x, float target, float d) => x < target ? (x + d).Min(target) : (x - d).Max(target);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double MoveTo(this double x, double target, double d) => x < target ? (x + d).Min(target) : (x - d).Max(target);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AngleMoveTo(this float x, float target, float da)
        {
            Debug.Assert(da > 0);
            // use the shortest path.
            var delta = (x - target + 180).Repeat(360) - 180; // range to -180, 180, sign is shortest direction.
            return (target + delta.MoveTo(0, da)).NormalizeAngle180();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AngleMoveTo(this double x, double target, double da)
        {
            Debug.Assert(da > 0);
            // use the shortest path.
            var delta = (x - target + 180).Repeat(360) - 180; // range to -180, 180, sign is shortest direction.
            return (target + delta.MoveTo(0, da)).NormalizeAngle180();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(this int a, int b) => Math.Max(a, b);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Max(this long a, long b) => Math.Max(a, b);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Max(this double a, double b) => Math.Max(a, b);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(this float a, float b) => Math.Max(a, b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(this int a, int b) => Math.Min(a, b);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Min(this long a, long b) => Math.Min(a, b);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Min(this double a, double b) => Math.Min(a, b);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(this float a, float b) => Math.Min(a, b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this int x) => x.Abs() < 1e-12f ? 0 : x < 0 ? -1 : 1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this long x) => x.Abs() < 1e-12f ? 0 : x < 0 ? -1 : 1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this double x) => x.Abs() < 1e-12f ? 0 : x < 0 ? -1 : 1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sign(this float x) => x.Abs() < 1e-12f ? 0 : x < 0 ? -1 : 1;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Exp(this float x) => (float)Math.Exp(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(this float x, float y) => (float)Math.Pow(x, y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Floor(this float x) => (float)Math.Floor(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Ceil(this float x) => (float)Math.Ceiling(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round(this float x) => (float)Math.Round(x);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Repeat(this int x, int y)
        {
            var m = x % y;
            if(m < 0) m += y;
            return m;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Repeat(this long x, long y)
        {
            var m = x % y;
            if(m < 0) m += y;
            return m;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Repeat(this float x, float y)
        {
            var m = x % y;
            if(m < 0) m += y;
            return m;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Repeat(this double x, double y)
        {
            var m = x % y;
            if(m < 0) m += y;
            return m;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool In(this int x, int a, int b) => a <= x && x <= b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InExclusive(this int x, int a, int b) => a < x && x < b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool In(this float x, float a, float b) => a <= x && x <= b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InExclusive(this float x, float a, float b) => a < x && x < b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool In(this double x, double a, double b) => a <= x && x <= b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InExclusive(this double x, double a, double b) => a < x && x < b;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this (int a, int b) v, int x) => v.a <= x && x <= v.b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsExclusive(this (int a, int b) v, int x) => v.a < x && x < v.b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this (float a, float b) v, float x) => v.a <= x && x <= v.b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsExclusive(this (float a, float b) v, float x) => v.a < x && x < v.b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this (double a, double b) v, double x) => v.a <= x && x <= v.b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsExclusive(this (double a, double b) v, double x) => v.a < x && x < v.b;
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(this float x) => (int)Math.Floor(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(this double x) => (int)Math.Floor(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(this float x) => (int)Math.Ceiling(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(this double x) => (int)Math.Ceiling(x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundToInt(this float x) => (int)Math.Round(x);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int RoundToInt(this double x) => (int)Math.Round(x);
		
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float FloorToMultiplierOf(this float x, float multiplier) => (float)Math.Floor(x / multiplier) * multiplier;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double FloorToMultiplierOf(this double x, double multiplier) => Math.Floor(x / multiplier) * multiplier;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FloorToMultiplierOf(this int x, int multiplier) => x - x.Repeat(multiplier);
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long FloorToMultiplierOf(this long x, long multiplier) => x - x.Repeat(multiplier);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float CeilToMultiplierOf(this float x, float multiplier) => (float)Math.Ceiling(x / multiplier) * multiplier;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double CeilToMultiplierOf(this double x, double multiplier) => Math.Ceiling(x / multiplier) * multiplier;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int CeilToMultiplierOf(this int x, int multiplier) => (int)Math.Ceiling((double)x / multiplier) * multiplier;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long CeilToMultiplierOf(this long x, long multiplier) => (long)Math.Ceiling((double)x / multiplier) * multiplier;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float RoundToMultiplierOf(this float x, float multiplier) => (float)Math.Round(x / multiplier) * multiplier;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double RoundToMultiplierOf(this double x, double multiplier) => Math.Round(x / multiplier) * multiplier;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int RoundToMultiplierOf(this int x, int multiplier) => (int)Math.Round((double)x / multiplier) * multiplier;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long RoundToMultiplierOf(this long x, long multiplier) => (long)Math.Round((double)x / multiplier) * multiplier;
		
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref double SetClamp(this ref double x, double a, double b)
        {
            x = (x < a ? a : x > b ? b : x);
            return ref x;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref float SetClamp(this ref float x, float a, float b)
        {
            x = (x < a ? a : x > b ? b : x);
            return ref x;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref long SetClamp(this ref long x, long a, long b)
        {
            x = (x < a ? a : x > b ? b : x);
            return ref x;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref int SetClamp(this ref int x, int a, int b)
        {
            x = (x < a ? a : x > b ? b : x);
            return ref x;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(this double x, double a, double b) => x < a ? a : x > b ? b : x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float x, float a, float b) => x < a ? a : x > b ? b : x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Clamp(this long x, long a, long b) => x < a ? a : x > b ? b : x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this int x, int a, int b) => x < a ? a : x > b ? b : x;
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float XMap(this float x, float a, float b) => (x - a) / (b - a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float XMap(this float x, float a, float b, float from, float to) => (x - a) / (b - a) * (to - from) + from;
        
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float XMapClamped(this float x, float a, float b, float from, float to)
		{
			if(to < from)
			{
				(a, b) = (b, a);
				(from, to) = (to, from);
			}
			return Math.Clamp((x - a) / (b - a) * (to - from) + from, from, to);
		}
		
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(this (float a, float b) v, float x) => (v.b - v.a) * x + v.a;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(this (double a, double b) v, double x) => (v.b - v.a) * x + v.a;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InvLerp(this (float a, float b) v, float x) => (x - v.a) / (v.b - v.a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double InvLerp(this (double a, double b) v, double x) => (x - v.a) / (v.b - v.a);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LerpAngle360(this (double from, double to) v, double t)
        {
			double delta = v.to - v.from;
			delta -= Math.Floor((delta + 180f) / 360f) * 360f;
			if(delta >= 180f) delta -= 360f;
			return (v.from + delta * t).NormalizeAngle360();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpAngle360(this (float from, float to) v, float t)
        {
			float delta = v.to - v.from;
			delta -= MathF.Floor((delta + 180f) / 360f) * 360f;
			if(delta >= 180f) delta -= 360f;
			return (v.from + delta * t).NormalizeAngle360();
        }
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float LerpAngle180(this (float from, float to) v, float t)
		{
			float delta = v.to - v.from;
			delta -= MathF.Floor((delta + 180f) / 360f) * 360f;
			if(delta >= 180f) delta -= 360f;
			return (v.from + delta * t).NormalizeAngle180();
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double LerpAngle180(this (double from, double to) v, double t)
		{
			double delta = v.to - v.from;
			delta -= Math.Floor((delta + 180f) / 360f) * 360f;
			if(delta >= 180f) delta -= 360f;
			return (v.from + delta * t).NormalizeAngle180();
		}
        
		/// <summary>
		/// 将角度归一化到 [-180, 180)
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NormalizeAngle180(this float angle) => (angle % 360 + 540) % 360 - 180;
		
		/// <summary>
		/// 将角度归一化到 [-180, 180)
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double NormalizeAngle180(this double angle) => (angle % 360 + 540) % 360 - 180;
		
		/// <summary>
		/// 将角度归一化到 [0, 360)
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float NormalizeAngle360(this float angle) => (angle % 360 + 360) % 360;
		
		/// <summary>
		/// 将角度归一化到 [0, 360)
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double NormalizeAngle360(this double angle) => (angle % 360 + 360) % 360;
		
		/// <summary>
		/// 计算角度差（度）
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float AngleDiff180(this float from, float to) => (to - from).NormalizeAngle180();
		
		/// <summary>
		/// 计算角度差（度）
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double AngleDiff180(this double from, double to) => (to - from).NormalizeAngle180();
        
		/// <summary>
		/// 计算角度差（度）
		/// </summary>
		/// <param name="from"></param>
		/// <param name="v"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float AngleDiff180(this (float from, float to) v) => (v.to - v.from).NormalizeAngle180();
		
		/// <summary>
		/// 计算角度差（度）
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double AngleDiff180(this (double from, double to) v) => (v.to - v.from).NormalizeAngle180();
		
		
		/// <summary>
		/// 计算角度差（度）
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float AngleDiff360(this float from, float to) => (to - from).NormalizeAngle360();
		
		/// <summary>
		/// 计算角度差（度）
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double AngleDiff360(this double from, double to) => (to - from).NormalizeAngle360();
		
		
        // 梯形插值. trapzoid interpolation.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Terp(this float x, float start, float fullStart, float fullEnd, float end)
        {
            if (x < start) return 0;
            if (x > end) return 0;
            if(x < fullStart) return x.XMap(start, fullStart, 0, 1);
            if(x > fullEnd) return x.XMap(fullEnd, end, 1, 0);
            return 1;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt<T>(this T e) where T: struct, Enum
        {
            Debug.Assert(Enum.GetUnderlyingType(typeof(T)) == typeof(int));
            return Unsafe.As<T, int>(ref e);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int PositionPowerOfTwo(this int x)
        {
            Debug.Assert(x.IsPowerOfTwo());
            if(x <= 0) throw new ArgumentException("must be positive", "x");
            int result = 0;
            while (x > 1)
            {
                x >>= 1;
                result++;
            }
            return result;
        }
        
        /// <summary>
        /// 返回大于或等于 x 的最小 2 的幂
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextPowerOfTwo(this int x)
        {
            x |= x >> 16;
            x |= x >> 8;
            x |= x >> 4;
            x |= x >> 2;
            x |= x >> 1;
            return x + 1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long NextPowerOfTwo(this long x)
        {
            x |= x >> 32;
            x |= x >> 16;
            x |= x >> 8;
            x |= x >> 4;
            x |= x >> 2;
            x |= x >> 1;
            return x + 1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint NextPowerOfTwo(this uint x)
        {
            x |= x >> 16;
            x |= x >> 8;
            x |= x >> 4;
            x |= x >> 2;
            x |= x >> 1;
            return x + 1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong NextPowerOfTwo(this ulong x)
        {
            x |= x >> 32;
            x |= x >> 16;
            x |= x >> 8;
            x |= x >> 4;
            x |= x >> 2;
            x |= x >> 1;
            return x + 1;
        }
        
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(this int a)
		{
			if(a <= 0) throw new ArgumentException($"a[{a}] must be positive", nameof(a));
			if(!a.IsPowerOfTwo()) throw new ArgumentException($"a[{a}] is not a power of two", nameof(a));
			
			int n = 0;
			
			// 二分查找判别法.
			if ((a & 0xFFFF0000) != 0) { a >>= 16; n += 16; }
			if ((a & 0xFF00) != 0) { a >>= 8;  n += 8; }
			if ((a & 0xF0) != 0) { a >>= 4;  n += 4; }
			if ((a & 0xC) != 0) { a >>= 2;  n += 2; }
			if ((a & 0x2) != 0) { n += 1; }
			
			return n;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int TrailingZeroCount(this uint a)
		{
			if(a == 0) throw new ArgumentException($"a[{a}] must be positive", nameof(a));
			if(!a.IsPowerOfTwo()) throw new ArgumentException($"a[{a}] is not a power of two", nameof(a));
			
			int n = 0;
			if ((a & 0xFFFF0000) != 0) { a >>= 16; n += 16; }
			if ((a & 0xFF00) != 0) { a >>= 8;  n += 8; }
			if ((a & 0xF0) != 0) { a >>= 4;  n += 4; }
			if ((a & 0xC) != 0) { a >>= 2;  n += 2; }
			if ((a & 0x2) != 0) { n += 1; }
			
			return n;
		}
		
        // ====================================================================================================
        // ====================================================================================================
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOfTwo(this int x) => unchecked(x & (x - 1)) == 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOfTwo(this long x) => unchecked(x & (x - 1)) == 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOfTwo(this uint x) => unchecked(x & (x - 1)) == 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOfTwo(this ulong x) => unchecked(x & (x - 1)) == 0;
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(int f, int count)> Factorization(this int x)
        {
            for(int i = 2, sqrt = Math.Sqrt(x).FloorToInt(); i <= sqrt; i += 2)
            {
                int count = 0;
                for(; x % i == 0; count++, x /= i);
                if(count > 0) yield return (i, count);
            }
            if (x > 1) yield return (x, 1);          // x is a prime.
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(long f, int count)> Factorization(this long x)
        {
            for(long i = 2, sqrt = Math.Sqrt(x).FloorToInt(); i <= sqrt; i += 2)
            {
                int count = 0;
                for(; x % i == 0; count++, x /= i);
                if(count > 0) yield return (i, count);
            }
            if (x > 1) yield return (x, 1);          // x is a prime.
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(uint f, int count)> Factorization(this uint x)
        {
            for(uint i = 2, sqrt = (uint)Math.Sqrt(x); i <= sqrt; i += 2)
            {
                int count = 0;
                for(; x % i == 0; count++, x /= i);
                if(count > 0) yield return (i, count);
            }
            if (x > 1) yield return (x, 1);          // x is a prime.
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(ulong f, int count)> Factorization(this ulong x)
        {
            for(ulong i = 2, sqrt = (ulong)Math.Sqrt(x); i <= sqrt; i += 2)
            {
                int count = 0;
                for(; x % i == 0; count++, x /= i);
                if(count > 0) yield return (i, count);
            }
            if (x > 1) yield return (x, 1);          // x is a prime.
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrime(this int x)
        {
            if(x < 2) return false;
            for(int i = 2, sqrt = Math.Sqrt(x).FloorToInt(); i <= sqrt; i += 2) if(x % i == 0) return false;
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isPrime(this long x)
        {
            if(x < 2) return false;
            for(long i = 2, sqrt = Math.Sqrt(x).FloorToInt(); i <= sqrt; i += 2) if(x % i == 0) return false;
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrime(this uint x)
        {
            if(x < 2) return false;
            for(uint i = 2, sqrt = (uint)Math.Sqrt(x); i <= sqrt; i += 2) if(x % i == 0) return false;
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrime(this ulong x)
        {
            if(x < 2) return false;
            for(ulong i = 2, sqrt = (ulong)Math.Sqrt(x); i <= sqrt; i += 2) if(x % i == 0) return false;
            return true;
        }
        
        // ====================================================================================================
        // ====================================================================================================
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt(this int x) => unchecked((uint)x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this uint x) => unchecked((int)x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ToULong(this long x) => unchecked((ulong)x);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ToLong(this ulong x) => unchecked((long)x);
        
		// ============================================================================
		// ============================================================================
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool DistributeTo(this int value, int count, int min, int max, List<int> result)
		{
			if(min > max) throw new ArgumentException($"min[{min}] must be less than max[{max}]");
			if(count <= 0) throw new ArgumentException($"count[{count}] must be greater than 0");
			if(value <= 0) throw new ArgumentException($"value[{value}] must be greater than 0");
			
			result.Clear();
			
			if (value < count * min || value > count * max)
				return false; // 无法满足条件

			// 初始化每个元素为最小值
			int[] temp = new int[count];
			for (int i = 0; i < count; i++) temp[i] = min;
			int remaining = value - count * min;

			while (remaining > 0)
			{
				for (int i = 0; i < count && remaining > 0; i++)
				{
					int maxAdd = Math.Min(max - temp[i], remaining);
					if (maxAdd > 0)
					{
						int add = UnityEngine.Random.Range(0, maxAdd + 1);
						temp[i] += add;
						remaining -= add;
					}
				}
			}

			result.AddRange(temp);
			return true;
		}
        
    }
}
