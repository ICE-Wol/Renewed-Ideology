using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Prota.Unity
{
    #region grid
    public static partial class UnityMethodExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2Int WorldToCellClosest(this Grid grid, Vector2 worldPos)
        {
            if(grid.cellLayout == GridLayout.CellLayout.Rectangle)
                worldPos -= grid.cellSize.ToVec2() / 2;
            else if(grid.cellLayout == GridLayout.CellLayout.Isometric)
                worldPos += Vector2.up * grid.cellSize.y / 2;
                
            return grid.WorldToCell(worldPos).ToVec2Int();
        }
        
    }
    #endregion
    
    #region Rect
    public static partial class UnityMethodExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsInclusive(this Rect rect, Vector2 point)
        {
            return rect.xMin <= point.x && point.x <= rect.xMax
                && rect.yMin <= point.y && point.y <= rect.yMax;
        }
		
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ContainsExclusive(this Rect rect, Vector2 point)
		{
			return rect.xMin < point.x && point.x < rect.xMax
				&& rect.yMin < point.y && point.y < rect.yMax;
		}
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 BottomLeft(this Rect rect) => rect.min;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 BottomRight(this Rect rect) => new Vector2(rect.xMax, rect.yMin);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 TopLeft(this Rect rect) => new Vector2(rect.xMin, rect.yMax);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 TopRight(this Rect rect) => rect.max;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 TopCenter(this Rect rect) => new Vector2(rect.center.x, rect.yMax);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 BottomCenter(this Rect rect) => new Vector2(rect.center.x, rect.yMin);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 LeftCenter(this Rect rect) => new Vector2(rect.xMin, rect.center.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 RightCenter(this Rect rect) => new Vector2(rect.xMax, rect.center.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EdgeDistanceToPoint(this Rect rect, Vector2 point)
        {
            if(rect.xMin <= point.x && point.x <= rect.xMax) return (0f).Max(
                (rect.yMin - point.y).Max(point.y - rect.yMax)
            );
            if(point.x < rect.xMin) return (point, rect.BottomLeft()).Length().Min((point, rect.TopLeft()).Length());
            else return (point, rect.BottomRight()).Length().Min((point, rect.TopRight()).Length());
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CornerDistanceToPoint(this Rect rect, Vector2 point)
        {
            return (point, rect.BottomLeft()).Length()
                .Min((point, rect.BottomRight()).Length())
                .Min((point, rect.TopLeft()).Length())
                .Min((point, rect.TopRight()).Length())
            ;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect MoveDown(this Rect x, float a) => new Rect(x.x, x.y - a, x.width, x.height);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect MoveLeft(this Rect x, float a) => new Rect(x.x - a, x.y, x.width, x.height);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect MoveRight(this Rect x, float a) => new Rect(x.x + a, x.y, x.width, x.height);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect MoveUp(this Rect x, float a) => new Rect(x.x, x.y + a, x.width, x.height);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Shrink(this Rect x, float l, float r, float b, float t) => new Rect(x.x + l, x.y + b, x.width - l - r, x.height - b - t);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Expend(this Rect x, float l, float r, float b, float t) => new Rect(x.x - l, x.y - b, x.width + l + r, x.height + b + t);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithHeight(this Rect x, float a) => new Rect(x.x, x.y, x.width, a);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithWidth(this Rect x, float a) => new Rect(x.x, x.y, a, x.height);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithSize(this Rect x, Vector2 a) => new Rect(x.x, x.y, a.x, a.y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect WithSize(this Rect x, float a, float b) => new Rect(x.x, x.y, a, b);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Move(this Rect x, Vector2 d) => new Rect(x.position + d, x.size);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Move(this Rect r, float x, float y) => new Rect(r.x + x, r.y + y, r.width, r.height);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToLocalPosition(this Rect r, Vector2 d) => new Vector2((d.x - r.x) / r.size.x, (d.y - r.y) / r.size.y);
        
        // return: dx, dy (for top points);
        // shear: angle in degree.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ShearOffset(this Rect r, float shear)
        {
            var angle = shear * Mathf.Deg2Rad;
            var dx = r.height * Mathf.Sin(angle);
            var dy = r.width * (1 - Mathf.Cos(angle));
            return new Vector2(dx, dy);
        }
        
        // 最小的覆盖两个矩形的矩形.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect BoundingBox(this Rect r, Rect g)
        {
            var xMin = r.xMin.Min(g.xMin);
            var xMax = r.xMax.Max(g.xMax);
            var yMin = r.yMin.Min(g.yMin);
            var yMax = r.yMax.Max(g.yMax);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }
        
        // 把负数的长宽变为正数, 保持矩形的位置不变.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect Normalize(this Rect r)
        {
            var min = r.min;
            var max = r.max;
            if(min.x > max.x) (min.x, max.x) = (max.x, min.x);
            if(min.y > max.y) (min.y, max.y) = (max.y, min.y);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
        
        
        // anchor: 缩放位置, 是矩形所处坐标系上,和矩形同级的点.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect ResizeWithAnchor(this Rect r, Vector2 anchor, Vector2 size)
        {
            var min = r.min - anchor;
            var max = r.max - anchor;
            min = Vector2.Scale(min, size);
            max = Vector2.Scale(max, size);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
        
        // 点 p 在 r 坐标系中的相对位置. Rect r 定义坐标系, 左下角 r.min 是原点 (0, 0), 右上角 r.max 是 (1, 1).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 NormalizedToPoint(this Rect r, Vector2 p)
        {
            return Rect.NormalizedToPoint(r, p);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 PointToNormalized(this Rect r, Vector2 p)
        {
            return Rect.PointToNormalized(r, p);
        }
        
        // 将矩形 r 从 t 定义的坐标系转换到 v 定义的坐标系.
        // 参考坐标系左下角是 (0, 0), 右上角是 (1, 1).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect XMap(this Rect r, Rect from, Rect to)
        {
            var min = from.NormalizedToPoint(r.min);
            var max = from.NormalizedToPoint(r.max);
            min = to.min + to.size * min;
            max = to.min + to.size * max;
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
        
        // 将矩形 r 从自身所处的坐标系转换到 reference 定义的坐标系.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rect NormalizedIn(this Rect r, Rect reference)
        {
            return r.XMap(reference, Rect.MinMaxRect(0, 0, 1, 1));
        }
        
        // 把矩形 r 变换为矩形 p, 需要做的缩放和平移操作.
        // 缩放和平移都以 (xmin, ymin) 为原点. 平移使用世界坐标(即两个矩形所在的坐标系).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Vector2 offset, Vector2 scale) OffsetScaleFor(this Rect r, Rect p)
        {
            var scale = p.size / r.size;
            var offset = p.min - r.min;
            return (offset, scale);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FillWithStandardMeshOrder(this Rect x, Vector2[] arr)
        {
            arr[0] = x.TopLeft();
            arr[1] = x.TopRight();
            arr[2] = x.BottomLeft();
            arr[3] = x.BottomRight();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FillWithStandardMeshOrder(this Rect x, Vector3[] arr)
        {
            arr[0] = x.TopLeft();
            arr[1] = x.TopRight();
            arr[2] = x.BottomLeft();
            arr[3] = x.BottomRight();
        }
		
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CastFromInside(this Rect rect, Ray2D ray, out Vector2 hitPoint)
		{
			return rect.CastFromInside(ray.origin, ray.direction, out hitPoint);
		}
		
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CastFromInside(this Rect rect, Vector2 origin, Vector2 dir, out Vector2 hitPoint)
		{
			if(dir.sqrMagnitude < 1e-7f)
			{
				hitPoint = default;
				return false;
			}
			
			if(!rect.ContainsInclusive(origin))
			{
				hitPoint = default;
				return false;
			}
			
			hitPoint = origin;
			float tMin = float.PositiveInfinity;
			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static void TestEdge(float t, float min, float max, float fixedCoord, bool isX, ref float tMinRef, ref Vector2 hitRef, Vector2 origin, Vector2 dir)
			{
				if (t >= 0f && t < tMinRef)
				{
					float coord = isX ? origin.y + dir.y * t : origin.x + dir.x * t;
					if (coord >= min && coord <= max)
					{
						tMinRef = t;
						hitRef = isX ? new Vector2(fixedCoord, coord) : new Vector2(coord, fixedCoord);
					}
				}
			}

			if (dir.x != 0f)
			{
				TestEdge((rect.xMin - origin.x) / dir.x, rect.yMin, rect.yMax, rect.xMin, true, ref tMin, ref hitPoint, origin, dir);
				TestEdge((rect.xMax - origin.x) / dir.x, rect.yMin, rect.yMax, rect.xMax, true, ref tMin, ref hitPoint, origin, dir);
			}

			if (dir.y != 0f)
			{
				TestEdge((rect.yMin - origin.y) / dir.y, rect.xMin, rect.xMax, rect.yMin, false, ref tMin, ref hitPoint, origin, dir);
				TestEdge((rect.yMax - origin.y) / dir.y, rect.xMin, rect.xMax, rect.yMax, false, ref tMin, ref hitPoint, origin, dir);
			}

			return tMin < float.PositiveInfinity;
		}
    }
    #endregion
    
    #region Color
    public static partial class UnityMethodExtensions
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Inverse(this Color c) => new Color(1 - c.r, 1 - c.g, 1 - c.b, c.a);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Add(this Color a, Color b) => new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Sub(this Color p, Color q) => new Color(p.r - q.r, p.g - q.g, p.b - q.b, p.a - q.a);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithRGB(this Color c, Color r) => new Color(r.r, r.g, r.b, c.a);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithR(this Color c, float r) => new Color(r, c.g, c.b, c.a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithG(this Color c, float g) => new Color(c.r, g, c.b, c.a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithB(this Color c, float b) => new Color(c.r, c.g, b, c.a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color WithA(this Color c, float a) => new Color(c.r, c.g, c.b, a);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Color SetR(this ref Color c, float r) { c.r = r; return ref c; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Color SetG(this ref Color c, float g) { c.g = g; return ref c; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Color SetB(this ref Color c, float b) { c.b = b; return ref c; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref Color SetA(this ref Color c, float a) { c.a = a; return ref c; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToWebString(this Color c) => ColorUtility.ToHtmlStringRGBA(c);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ToColor(this string str) => ColorUtility.TryParseHtmlString(str, out Color c) ? c : Color.clear;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVec4(this Color c) => new Vector4(c.r, c.g, c.b, c.a);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color ToColor(this Vector4 x) => new Color(x.x, x.y, x.z, x.w);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Lerp(this (Color from, Color to) p, float x) => p.from + (p.to - p.from) * x;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LerpAngle(this (Vector3 eulerA, Vector3 eulerB) v, float x)
        {
            return new Vector3(
                Mathf.LerpAngle(v.eulerA.x, v.eulerB.x, x),
                Mathf.LerpAngle(v.eulerA.y, v.eulerB.y, x),
                Mathf.LerpAngle(v.eulerA.z, v.eulerB.z, x)
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Diff(this (Color32 a, Color32 b) x) => (x.a.r - x.b.r).Abs()
            + (x.a.g - x.b.g).Abs()
            + (x.a.b - x.b.b).Abs()
            + (x.a.a - x.b.a).Abs();
        
    }
    

    
    
    #endregion
    
    
    #region Universal Maths
    public static partial class UnityMethodExtensions
    {
        
        // 4向分离, 从0开始, 逆时针.
        // 返回值为0, 1, 2, 3, 分别表示右, 上, 左, 下.
        public static int DirectionPartition4(this Vector2 dir)
        {
            var angle = Vector2.SignedAngle(Vector2.right, dir);
            if(angle < 0) angle += 360;
            if(angle < 45f) return 0;
            if(angle < 135f) return 1;
            if(angle < 225f) return 2;
            if(angle < 315f) return 3;
            return 0;
        }
        
        // 4向分离.
        public static Vector2Int DirectionNormalize4(this Vector2 dir)
        {
            var partition = dir.DirectionPartition4();
            switch (partition)
            {
                case 0: return Vector2Int.right;
                case 1: return Vector2Int.up;
                case 2: return Vector2Int.left;
                case 3: return Vector2Int.down;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        // 8向分离, 从0开始, 逆时针.
        // 返回值为0, 1, 2, 3, 4, 5, 6, 7, 分别表示右, 右上, 上, 左上, 左, 左下, 下, 右下.
        public static int DirectionPartition8(this Vector2 dir)
        {
            var angle = Vector2.SignedAngle(Vector2.right, dir);
            if(angle < 0) angle += 360;
            if(angle < 22.5f) return 0;
            if(angle < 67.5f) return 1;
            if(angle < 112.5f) return 2;
            if(angle < 157.5f) return 3;
            if(angle < 202.5f) return 4;
            if(angle < 247.5f) return 5;
            if(angle < 292.5f) return 6;
            if(angle < 337.5f) return 7;
            return 0;
        }
        
        // 8向分离.
        public static Vector2Int DirectionNormalize8(this Vector2 dir)
        {
            var partition = dir.DirectionPartition8();
            switch (partition)
            {
                case 0: return Vector2Int.right;
                case 1: return new Vector2Int(1, 1);
                case 2: return Vector2Int.up;
                case 3: return new Vector2Int(-1, 1);
                case 4: return Vector2Int.left;
                case 5: return new Vector2Int(-1, -1);
                case 6: return Vector2Int.down;
                case 7: return new Vector2Int(1, -1);
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static string DirectionPartitionName4(this Vector2 dir)
        {
            var partition = dir.DirectionPartition4();
            switch (partition)
            {
                case 0: return "Right";
                case 1: return "Up";
                case 2: return "Left";
                case 3: return "Down";
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static string DirectionPartitionName8(this Vector2 dir)
        {
            var partition = dir.DirectionPartition8();
            switch (partition)
            {
                case 0: return "Right";
                case 1: return "UpRight";
                case 2: return "Up";
                case 3: return "UpLeft";
                case 4: return "Left";
                case 5: return "DownLeft";
                case 6: return "Down";
                case 7: return "DownRight";
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static string DirectionPartitionToName8(this int dir)
        {
            switch (dir)
            {
                case 0: return "Right";
                case 1: return "UpRight";
                case 2: return "Up";
                case 3: return "UpLeft";
                case 4: return "Left";
                case 5: return "DownLeft";
                case 6: return "Down";
                case 7: return "DownRight";
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static string DirectionPartitionToName4(this int dir)
        {
            switch (dir)
            {
                case 0: return "Right";
                case 1: return "Up";
                case 2: return "Left";
                case 3: return "Down";
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static int DirectionNameToPartition4(this string dir)
        {
            switch (dir)
            {
                case "Right": return 0;
                case "Up": return 1;
                case "Left": return 2;
                case "Down": return 3;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        public static int DirectionNameToPartition8(this string dir)
        {
            switch (dir)
            {
                case "Right": return 0;
                case "UpRight": return 1;
                case "Up": return 2;
                case "UpLeft": return 3;
                case "Left": return 4;
                case "DownLeft": return 5;
                case "Down": return 6;
                case "DownRight": return 7;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        
        public static bool PointInTriangle(this Vector2 x, Vector2 a, Vector2 b, Vector2 c)
        {
            var areaABC = Mathf.Abs((a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) / 2f);
            var areaPBC = Mathf.Abs((x.x * (b.y - c.y) + b.x * (c.y - x.y) + c.x * (x.y - b.y)) / 2f);
            var areaPCA = Mathf.Abs((a.x * (x.y - c.y) + x.x * (c.y - a.y) + c.x * (a.y - x.y)) / 2f);
            var areaPAB = Mathf.Abs((a.x * (b.y - x.y) + b.x * (x.y - a.y) + x.x * (a.y - b.y)) / 2f);
            return Mathf.Approximately(areaABC, areaPBC + areaPCA + areaPAB);
        }
        
        public static Vector2Int StepLeft(this Vector2Int a) => new Vector2Int(a.x - 1, a.y);
        public static Vector2Int StepRight(this Vector2Int a) => new Vector2Int(a.x + 1, a.y);
        public static Vector2Int StepUp(this Vector2Int a) => new Vector2Int(a.x, a.y + 1);
        public static Vector2Int StepDown(this Vector2Int a) => new Vector2Int(a.x, a.y - 1);
        public static Vector2Int StepUpLeft(this Vector2Int a) => new Vector2Int(a.x - 1, a.y + 1);
        public static Vector2Int StepUpRight(this Vector2Int a) => new Vector2Int(a.x + 1, a.y + 1);
        public static Vector2Int StepDownLeft(this Vector2Int a) => new Vector2Int(a.x - 1, a.y - 1);
        public static Vector2Int StepDownRight(this Vector2Int a) => new Vector2Int(a.x + 1, a.y - 1);
        
        public static Vector3Int StepLeft(this Vector3Int a) => new Vector3Int(a.x - 1, a.y, a.z);
        public static Vector3Int StepRight(this Vector3Int a) => new Vector3Int(a.x + 1, a.y, a.z);
        public static Vector3Int StepUp(this Vector3Int a) => new Vector3Int(a.x, a.y + 1, a.z);
        public static Vector3Int StepDown(this Vector3Int a) => new Vector3Int(a.x, a.y - 1, a.z);
        public static Vector3Int StepForward(this Vector3Int a) => new Vector3Int(a.x, a.y, a.z + 1);
        public static Vector3Int StepBack(this Vector3Int a) => new Vector3Int(a.x, a.y, a.z - 1);
        
        
        public static float ManhattanLength(this Vector2 a) => a.x + a.y;
        public static float ManhattanLength(this Vector3 a) => a.x + a.y + a.z;
        
        public static float ManhattanDistance(this Vector2 a, Vector2 b) => (a - b).ManhattanLength();
        public static float ManhattanDistance(this Vector3 a, Vector3 b) => (a - b).ManhattanLength();
        
        // 从a到b的两条曼哈顿路径的转折点.
        public static (Vector2 a, Vector2 b) ManhattanCorner(this Vector2 a, Vector2 b)
            => (new Vector2(a.x, b.y), new Vector2(b.x, a.y));
        
        public static float Sqrt(this float x) => Mathf.Sqrt(x);
        public static float Sqrt(this int x) => Mathf.Sqrt(x);
        public static double Sqrt(this double x) => Math.Sqrt(x);
        public static double Sqrt(this long x) => Math.Sqrt(x);
        
        public static float Sin(this float x) => Mathf.Sin(x);
        public static float Cos(this float x) => Mathf.Cos(x);
        public static float Tan(this float x) => Mathf.Tan(x);
        
        public static float PingPong(this float x, float a) => Mathf.PingPong(x, a);
        
        public static bool IsZero(this Vector2 a) => a == Vector2.zero;
        public static bool IsZero(this Vector3 a) => a == Vector3.zero;
        public static bool IsZero(this Vector4 a) => a == Vector4.zero;
        
        public static Vector2 Abs(this Vector2 a) => new Vector2(Mathf.Abs(a.x), Mathf.Abs(a.y));
        public static Vector3 Abs(this Vector3 a) => new Vector3(Mathf.Abs(a.x), Mathf.Abs(a.y), Mathf.Abs(a.z));
        
        public static Vector2 To(this Vector2 a, Vector2 b) => b - a;
        public static Vector3 To(this Vector3 a, Vector3 b) => b - a;
        public static Vector4 To(this Vector4 a, Vector4 b) => b - a;
        
        
        public static Vector2 WithX(this Vector2 a, float x) => new Vector2(x, a.y);
        public static Vector2 WithY(this Vector2 a, float y) => new Vector2(a.x, y);
        
        public static Vector2 WithNegativeX(this Vector2 a) => new Vector2(-a.x, a.y);
        public static Vector2 WithNegativeY(this Vector2 a) => new Vector2(a.x, -a.y);
        
        public static Vector3 WithX(this Vector3 a, float x) => new Vector3(x, a.y, a.z);
        public static Vector3 WithY(this Vector3 a, float y) => new Vector3(a.x, y, a.z);
        public static Vector3 WithZ(this Vector3 a, float z) => new Vector3(a.x, a.y, z);
        
        public static Vector3 WithXY(this Vector3 a, Vector2 xy) => new Vector3(xy.x, xy.y, a.z);
        
        
        public static Vector4 WithX(this Vector4 a, float x) => new Vector4(x, a.y, a.z, a.w);
        public static Vector4 WithY(this Vector4 a, float y) => new Vector4(a.x, y, a.z, a.w);
        public static Vector4 WithZ(this Vector4 a, float z) => new Vector4(a.x, a.y, z, a.w);
        public static Vector4 WithW(this Vector4 a, float w) => new Vector4(a.x, a.y, a.z, w);
        
        
        public static ref Vector2 SetX(this ref Vector2 a, float x) { a.x = x; return ref a; }
        public static ref Vector2 SetY(this ref Vector2 a, float y) { a.y = y; return ref a; }
        
        public static ref Vector3 SetX(this ref Vector3 a, float x) { a.x = x; return ref a; }
        public static ref Vector3 SetY(this ref Vector3 a, float y) { a.y = y; return ref a; }
        public static ref Vector3 SetZ(this ref Vector3 a, float z) { a.z = z; return ref a; }
        
        public static ref Vector4 SetX(this ref Vector4 a, float x) { a.x = x; return ref a; }
        public static ref Vector4 SetY(this ref Vector4 a, float y) { a.y = y; return ref a; }
        public static ref Vector4 SetZ(this ref Vector4 a, float z) { a.z = z; return ref a; }
        public static ref Vector4 SetW(this ref Vector4 a, float w) { a.w = w; return ref a; }
        
        public static Vector3 XYToXZ(this Vector2 a, float y = 0) => new Vector3(a.x, y, a.y);
        public static Vector2 XZToXY(this Vector3 a) => new Vector2(a.x, a.z);
        
        public static Vector3 WithLength(this Vector3 a, float len)
		{
			#if UNITY_EDITOR
			if(a.x == 0 && a.y == 0 && a.z == 0) return a;
			if(float.IsInfinity(a.x) || float.IsInfinity(a.y)) throw new Exception("Vector2 is infinity");
			#endif
			
			return a.normalized * len;
		}
		
        public static Vector2 WithLength(this Vector2 a, float len)
		{
			#if UNITY_EDITOR
			if(a.x == 0 && a.y == 0) return a;
			if(float.IsInfinity(a.x) || float.IsInfinity(a.y)) throw new Exception("Vector2 is infinity");
			#endif
			
			return a.normalized * len;
		}
		
        public static Vector2 WithMaxLength(this Vector2 a, float maxLen)
		{
			if(float.IsInfinity(a.x) || float.IsInfinity(a.y)) throw new Exception("Vector2 is infinity");
			if(a.sqrMagnitude <= maxLen * maxLen) return a;
			return a.WithLength(maxLen);
		}
        public static Vector3 WithMaxLength(this Vector3 a, float maxLen)
		{
			if(float.IsInfinity(a.x) || float.IsInfinity(a.y) || float.IsInfinity(a.z)) throw new Exception("Vector3 is infinity");
			if(a.sqrMagnitude <= maxLen * maxLen) return a;
			return a.WithLength(maxLen);
		}
        
        public static Vector2 WithMinLength(this Vector2 a, float maxLen)
		{
			if(float.IsInfinity(a.x) || float.IsInfinity(a.y)) throw new Exception("Vector2 is infinity");
			if(a.sqrMagnitude <= maxLen * maxLen) return a;
			return a.WithLength(maxLen);
		}
        public static Vector3 WithMinLength(this Vector3 a, float maxLen)
		{
			if(float.IsInfinity(a.x) || float.IsInfinity(a.y) || float.IsInfinity(a.z)) throw new Exception("Vector3 is infinity");
			if(a.sqrMagnitude <= maxLen * maxLen) return a;
			return a.WithLength(maxLen);
		}
        
        public static Vector2 AddLength(this Vector2 a, float addLen) => a.WithLength(a.magnitude + addLen);
        public static Vector3 AddLength(this Vector3 a, float addLen) => a.WithLength(a.magnitude + addLen);
        
        public static Vector2 MoveTowards(this Vector2 a, Vector2 b, float maxDistanceDelta)
            => Vector2.MoveTowards(a, b, maxDistanceDelta);
            
        public static Vector3 MoveTowards(this Vector3 a, Vector3 b, float maxDistanceDelta)
            => Vector3.MoveTowards(a, b, maxDistanceDelta);
        
		/// <summary>
		/// L2 = 二阶（临界阻尼）Approach
		/// - from / to : 本帧的起点与目标（几何状态）
		/// - velocity  : 系统的唯一 context（动力学状态）
		/// - 结果      : 从 from 出发、受 velocity 影响，朝 to 演化 dt 后的位置
		/// </summary>
		/// <param name="velocity">当前速度</param>
		/// <param name="currentPosition">当前值</param>
		/// <param name="targetPosition">目标值</param>
		/// <param name="smoothTime">平滑时间</param>
		/// <param name="maxSpeed">最大速度</param>
		/// <param name="dt">时间间隔</param>
		/// <param name="epsilon">最小距离阈值</param>
		/// <returns>逼近后的位置</returns>
		public static Vector2 ApproachL2(
			this ref Vector2 velocity,
			Vector2 currentPosition,
			Vector2 targetPosition,
			float smoothTime,
			float maxSpeed,
			float dt,
			float epsilon = 1e-4f)
		{
			smoothTime = Mathf.Max(epsilon, smoothTime);

			float omega = 2f / smoothTime;
			float x = omega * dt;
			
			// 临界阻尼二阶系统的指数衰减近似项
			// 对连续系统 x'' + 2ω x' + ω² x = ω² target 做数值积分的稳定近似
			float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);

			// 几何误差（仅由起止状态决定）
			var error = currentPosition - targetPosition;

			// 限制远距离速度爆冲（几何约束）
			float maxError = maxSpeed * smoothTime;
			float mag = error.magnitude;
			if (mag > maxError)
				error = error / mag * maxError;

			// 二阶动力学推进
			var temp = (velocity + omega * error) * dt;
			velocity = (velocity - omega * temp) * exp;

			var next = targetPosition + (error + temp) * exp;

			if ((next - targetPosition).sqrMagnitude <= epsilon * epsilon)
			{
				velocity = Vector2.zero;
				return targetPosition;
			}

			return next;
		}
		
        // 以乘算方式缩减距离, 每次按 ratio^dt 的比例缩减剩余距离.
        // ratio: 衰减率, 通常为 0-1 之间的值, 越小衰减越快.
        // dt: 时间间隔.
        // epsilon: 最小距离阈值, 当距离小于此值时直接返回目标值.
        public static Vector2 Approach(this Vector2 from, Vector2 to, float ratio, float dt, float epsilon = 1e-4f)
        {
            var delta = from - to;
            var newDelta = delta * Mathf.Pow(ratio, dt);
            if(newDelta.sqrMagnitude <= epsilon * epsilon) return to;
            return to + newDelta;
        }
        
        public static Vector3 Approach(this Vector3 from, Vector3 to, float ratio, float dt, float epsilon = 1e-4f)
        {
            var delta = from - to;
            var newDelta = delta * Mathf.Pow(ratio, dt);
            if(newDelta.sqrMagnitude <= epsilon * epsilon) return to;
            return to + newDelta;
        }
        
        public static float Approach(this float from, float to, float ratio, float dt, float epsilon = 1e-4f)
        {
            var delta = from - to;
            var newDelta = delta * Mathf.Pow(ratio, dt);
            if(Mathf.Abs(newDelta) <= epsilon) return to;
            return to + newDelta;
        }
        
        public static float Angle(this Vector2 a, Vector2 b) => Vector2.SignedAngle(a, b);
        
        public static float Dot(this Vector2 a, Vector2 b) => Vector2.Dot(a, b);
        public static float Dot(this Vector3 a, Vector3 b) => Vector3.Dot(a, b);
        public static float Dot(this Vector4 a, Vector4 b) => Vector4.Dot(a, b);
        
        public static float Cross(this Vector2 a, Vector2 b) => Vector3.Cross((Vector3)a, (Vector3)b).z;
        public static Vector3 Cross(this Vector3 a, Vector3 b) => Vector3.Cross(a, b);
        
        public static Vector4 ToVec4(this Quaternion q) => new Vector4(q.x, q.y, q.z, q.w);
        
        public static Quaternion ToQuaternion(this Vector4 q) => new Quaternion(q.x, q.y, q.z, q.w); 
        
        public static Vector2 Lerp(this (Vector2 from, Vector2 to) p, float x) => p.from + (p.to - p.from) * x;
        public static Vector3 Lerp(this (Vector3 from, Vector3 to) p, float x) => p.from + (p.to - p.from) * x;
        public static Vector4 Lerp(this (Vector4 from, Vector4 to) p, float x) => p.from + (p.to - p.from) * x;
        
        public static Vector2 ToVec2(this (float x, float y) a) => new Vector2(a.x, a.y);
        public static Vector3 ToVec3(this (float x, float y, float z) a) => new Vector3(a.x, a.y, a.z);
        
        public static (float x, float y) ToTuple(this Vector2 a) => (a.x, a.y);
        public static (float x, float y, float z) ToTuple(this Vector3 a) => (a.x, a.y, a.z);
        
        public static Vector2 Center(this (Vector2 from, Vector2 to) p) => p.Lerp(0.5f);
        public static Vector3 Center(this (Vector3 from, Vector3 to) p) => p.Lerp(0.5f);
        
        public static Vector2 Vec(this (Vector2 from, Vector2 to) p) => p.to - p.from;
        public static Vector3 Vec(this (Vector3 from, Vector3 to) p) => p.to - p.from;
        
        public static float Length(this (Vector2 from, Vector2 to) p) => p.Vec().magnitude;
        public static float Length(this (Vector3 from, Vector3 to) p) => p.Vec().magnitude;
        
        public static float Area(this Vector2 p) => p.x * p.y;
        
        public static float Perimiter(this Vector2 p) => (p.x + p.y) * 2;
        
        public static float Volume(this Vector3 p) => p.x * p.y * p.z;
        
        public static float SurfaceArea(this Vector3 p) => (p.x * p.y + p.x * p.z + p.y * p.z) * 2;
        
        public static Vector2 ToVec2(this Vector3 p) => new Vector2(p.x, p.y);
        public static Vector2 ToVec2(this Vector4 p) => new Vector2(p.x, p.y);
        public static Vector3 ToVec3(this Vector2 p, float z = 0) => new Vector3(p.x, p.y, z);
        public static Vector3 ToVec3(this Vector4 p) => new Vector3(p.x, p.y, p.z);
        public static Vector4 ToVec4(this Vector2 p, float z = 0, float w = 0) => new Vector4(p.x, p.y, z, w);
        public static Vector4 ToVec4(this Vector3 p, float w = 0) => new Vector4(p.x, p.y, p.z, w);
        
        public static Vector2Int FloorToInt(this Vector2 a) => new Vector2Int(a.x.FloorToInt(), a.y.FloorToInt());
        public static Vector3Int FloorToInt(this Vector3 a) => new Vector3Int(a.x.FloorToInt(), a.y.FloorToInt(), a.z.FloorToInt());
        public static Vector2Int CeilToInt(this Vector2 a) => new Vector2Int(a.x.CeilToInt(), a.y.CeilToInt());
        public static Vector3Int CeilToInt(this Vector3 a) => new Vector3Int(a.x.CeilToInt(), a.y.CeilToInt(), a.z.CeilToInt());
        public static Vector2Int RoundToInt(this Vector2 a) => new Vector2Int(a.x.RoundToInt(), a.y.RoundToInt());
        public static Vector3Int RoundToInt(this Vector3 a) => new Vector3Int(a.x.RoundToInt(), a.y.RoundToInt(), a.z.RoundToInt());
        
        public static Vector2 Floor(this Vector2 a) => new Vector2(a.x.Floor(), a.y.Floor());
        public static Vector3 Floor(this Vector3 a) => new Vector3(a.x.Floor(), a.y.Floor(), a.z.Floor());
        public static Vector2 Ceil(this Vector2 a) => new Vector2(a.x.Ceil(), a.y.Ceil());
        public static Vector3 Ceil(this Vector3 a) => new Vector3(a.x.Ceil(), a.y.Ceil(), a.z.Ceil());
        public static Vector2 Round(this Vector2 a) => new Vector2(a.x.Round(), a.y.Round());
        public static Vector3 Round(this Vector3 a) => new Vector3(a.x.Round(), a.y.Round(), a.z.Round());
        
        public static Vector3 Divide(this Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.x, a.z / b.z);
        public static Vector2 Divide(this Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.x);
        
        public static Vector2 Clamp(this Vector2 a, Vector2 min, Vector2 max) => new Vector2(a.x.Clamp(min.x, max.x), a.y.Clamp(min.y, max.y));
        public static Vector3 Clamp(this Vector3 a, Vector3 min, Vector3 max) => new Vector3(a.x.Clamp(min.x, max.x), a.y.Clamp(min.y, max.y), a.z.Clamp(min.z, max.z));
        
        public static Vector2 AngleToVec2(this float angleInDegree)
		{
			var rad = angleInDegree.NormalizeAngle360() * Mathf.Deg2Rad;
			var cos = Mathf.Cos(rad);
			var sin = Mathf.Sin(rad);
			return new Vector2(cos, sin);
		}
		
		public static Vector2 AngleToVec2(this float angleInDegree, float distance)
		{
			var vec = angleInDegree.AngleToVec2();
			return vec * distance;
		}
		
		
        public static Vector2 Rotate90Left(this Vector2 a) => new Vector2(-a.y, a.x);
        public static Vector2 Rotate90Right(this Vector2 a) => new Vector2(a.y, -a.x);
        
        public static Vector2 Rotate(this Vector2 a, float angleInDegree)
        {
            var rad = angleInDegree * Mathf.Deg2Rad;
            var cos = Mathf.Cos(rad);
            var sin = Mathf.Sin(rad);
            return new Vector2(a.x * cos - a.y * sin, a.x * sin + a.y * cos);
        }
        
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Angle(this Vector2 a) => Vector2.SignedAngle(Vector2.right, a);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float AngleUp(this Vector2 a) => Vector2.SignedAngle(Vector2.up, a);
        
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion AsEulerAngle(this (float x, float y, float z) a) => Quaternion.Euler(a.x, a.y, a.z);
        
        
        // Verlet 积分, 用位置相对时间的函数作泰勒展开, 用来精确模拟物理位移/速度/加速度行为
        // https://www.bilibili.com/video/BV1pG411F7b1
        public static float Verlet(this float curPos, float prevPos, float acceleration, float dt)
            => 2 * curPos - prevPos + dt * dt * acceleration;
        public static Vector2 Verlet(this Vector2 curPos, Vector2 prevPos, Vector2 acceleration, float dt)
            => 2 * curPos - prevPos + dt * dt * acceleration;
        public static Vector3 Verlet(this Vector3 curPos, Vector3 prevPos, Vector3 acceleration, float dt)
            => 2 * curPos - prevPos + dt * dt * acceleration;
        
        
        public static Vector3 ProjectToPerpendicularPlane(this Vector3 a, Vector3 normal)
            => a - Vector3.Project(a, normal);
        
        public static Vector3 Project(this Vector3 a, Vector3 normal)
            => Vector3.Project(a, normal);
        
        public static Vector2 ProjectToPerpendicularLine(this Vector2 a, Vector2 normal)
            => a - a.Project(normal);
        
        public static Vector2 Project(this Vector2 a, Vector2 normal)
            => a.Dot(normal.normalized) * normal.normalized;
        
        
        public static IEnumerable<Vector2> AverageDivide(this (Vector2 from, Vector2 to) v, int count)
        {
            (count > 0).Assert();
            for (var i = 0; i < count; i++) yield return v.from + (v.to - v.from) * (i + 1) / (count + 1);
        }
        
        public static IEnumerable<Vector3> AverageDivide(this (Vector3 from, Vector3 to) v, int count)
        {
            (count > 0).Assert();
            for (var i = 0; i < count; i++) yield return v.from + (v.to - v.from) * (i + 1) / (count + 1);
        }
        
        
        public static float SmoothStep(this float x) => x * x * (3 - 2 * x);
        
        public static float ToDegree(this float x) => x * Mathf.Rad2Deg;
        
        public static float ToRadian(this float x) => x * Mathf.Deg2Rad;
        
        public static Vector2Int Abs(this Vector2Int a) => new Vector2Int(a.x.Abs(), a.y.Abs());
        
        public static Vector3Int Abs(this Vector3Int a) => new Vector3Int(a.x.Abs(), a.y.Abs(), a.z.Abs());
        
        public static Vector2Int Min(this Vector2Int a, Vector2Int b) => new Vector2Int(a.x.Min(b.x), a.y.Min(b.y));
        
        public static Vector3Int Min(this Vector3Int a, Vector3Int b) => new Vector3Int(a.x.Min(b.x), a.y.Min(b.y), a.z.Min(b.z));
        
        public static Vector2Int Max(this Vector2Int a, Vector2Int b) => new Vector2Int(a.x.Max(b.x), a.y.Max(b.y));
        
        public static Vector3Int Max(this Vector3Int a, Vector3Int b) => new Vector3Int(a.x.Max(b.x), a.y.Max(b.y), a.z.Max(b.z));
        
        public static Vector2 Min(this Vector2 a, Vector2 b) => new Vector2(a.x.Min(b.x), a.y.Min(b.y));
        
        public static Vector3 Min(this Vector3 a, Vector3 b) => new Vector3(a.x.Min(b.x), a.y.Min(b.y), a.z.Min(b.z));
        
        public static Vector2 Max(this Vector2 a, Vector2 b) => new Vector2(a.x.Max(b.x), a.y.Max(b.y));
        
        public static Vector3 Max(this Vector3 a, Vector3 b) => new Vector3(a.x.Max(b.x), a.y.Max(b.y), a.z.Max(b.z));
        
        public static float MaxComponent(this Vector2 a) => a.x.Max(a.y);
        
        public static float MaxComponent(this Vector3 a) => a.x.Max(a.y).Max(a.z);
        
        public static float MinComponent(this Vector2 a) => a.x.Min(a.y);
        
        public static float MinComponent(this Vector3 a) => a.x.Min(a.y).Min(a.z);
        
        public static int MaxComponent(this Vector2Int a) => a.x.Max(a.y);
        
        public static int MaxComponent(this Vector3Int a) => a.x.Max(a.y).Max(a.z);
        
        public static int MinComponent(this Vector2Int a) => a.x.Min(a.y);
        
        public static int MinComponent(this Vector3Int a) => a.x.Min(a.y).Min(a.z);
        
        public static float SumAllComponents(this Vector2 a) => a.x + a.y;
        
        public static float SumAllComponents(this Vector3 a) => a.x + a.y + a.z;
        
        public static int SumAllComponents(this Vector2Int a) => a.x + a.y;
        
        public static int SumAllComponents(this Vector3Int a) => a.x + a.y + a.z;
        
        
        public static Vector3 Multiply(this Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        
        
        // 切比雪夫距离
        public static int DistanceChebyshev(this Vector2Int a, Vector2Int b)
            => (a - b).Abs().MaxComponent();
        
        public static int DistanceChebyshev(this Vector3Int a, Vector3Int b)
            => (a - b).Abs().MaxComponent();
            
        public static float DistanceChebyshev(this Vector2 a, Vector2 b)
            => (a - b).Abs().MaxComponent();
            
        public static float DistanceChebyshev(this Vector3 a, Vector3 b)
            => (a - b).Abs().MaxComponent();
            
        public static int DistanceManhattan(this Vector2Int a, Vector2Int b)
            => (a - b).Abs().SumAllComponents();
            
        public static int DistanceManhattan(this Vector3Int a, Vector3Int b)
            => (a - b).Abs().SumAllComponents();
            
        public static float DistanceManhattan(this Vector2 a, Vector2 b)
            => (a - b).Abs().SumAllComponents();
            
        public static float DistanceManhattan(this Vector3 a, Vector3 b)
            => (a - b).Abs().SumAllComponents();
        
        public static float Distance(this Vector2 a, Vector2 b)
            => (a - b).magnitude;
        
        public static float Distance(this Vector3 a, Vector3 b)
            => (a - b).magnitude;
            
        public static float Distance(this Vector2Int a, Vector2Int b)
            => (a - b).magnitude;
        
        public static float Distance(this Vector3Int a, Vector3Int b)
            => (a - b).magnitude;
            
        
        public static Vector2Int ToVec2Int(this Vector3Int a) => new Vector2Int(a.x, a.y);
        
        public static Vector2 ToVec2(this Vector3Int a) => new Vector2(a.x, a.y);
        
        public static Vector3Int ToVec3Int(this Vector2Int a, int z = 0) => new Vector3Int(a.x, a.y, z);
        
        public static Vector3 ToVec3(this Vector2Int a, float z = 0) => new Vector3(a.x, a.y, z);
        
        public static Vector2 ToVec2(this Vector2Int a) => new Vector2(a.x, a.y);
        
        public static Vector3 ToVec3(this Vector3Int a) => new Vector3(a.x, a.y, a.z);
        
        public static Vector2 HitXYPlane(this Ray a, float z = 0)
        {
            var t = -(a.origin.z - z) / a.direction.z;
            return a.origin + a.direction * t;
        }
        
        
        public static Vector2 SnapTo(this Vector2 p, Vector2 snap)
        {
            return p.SnapTo(snap, Vector2.zero);
        }
        
        public static Vector2 SnapTo(this Vector2 p, Vector2 snap, Vector2 origin)
        {
            if(snap.x <= 1e-12f || snap.y <= 1e-12f) return p;
            var delta = p - origin;
            var x = Mathf.Round(delta.x / snap.x) * snap.x;
            var y = Mathf.Round(delta.y / snap.y) * snap.y;
            return origin + new Vector2(x, y);
        }
        
        public static Matrix4x4 GetShearMatrixRadial2D(this float angleInDegree, Vector2 pivot, bool radial)
        {
            var mat = Matrix4x4.zero;
            mat.m33 = 1;
            var rad = angleInDegree.NormalizeAngle360() * Mathf.Deg2Rad;
            
            if(radial)
            {
                var cos = Mathf.Cos(rad);
                var sin = Mathf.Sin(rad);
                mat.m00 = 1;
                mat.m01 = sin;
                mat.m11 = cos;
            }
            else
            {
                mat.m01 = Mathf.Tan(rad);
            }
            
            if(pivot == Vector2.zero) return mat;
            return Matrix4x4.Translate(pivot) * mat * Matrix4x4.Translate(-pivot);
        }
        
        
        public static bool LaunchDirection(
            Vector2 mypos,
            Vector2 tgtpos, Vector2 tgtRelativeV,
            float projectileSpeed,
            out float t)
        {
            if(projectileSpeed <= 0)
            {
                t = 0;
                return false;
            }
            
            var dp = mypos.To(tgtpos);
            if(dp.magnitude / projectileSpeed < 1e-4)
            {
                t = 0;
                return true;
            }
            
            var a = tgtRelativeV.sqrMagnitude - projectileSpeed * projectileSpeed;
            var b = 2 * Vector2.Dot(dp, tgtRelativeV);
            var c = dp.sqrMagnitude;
            
            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                t = 0;
                return false;
            }
            
            // 优先拿小的那个.
            t = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
            if (t < 0) t = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            
            if (t < 0)
            {
                return false;
            }
            
            return true;
        }
        
        public static bool LaunchDirection(
            Vector2 mypos,
            Vector2 tgtpos, Vector2 tgtRelativeV,
            float projectileSpeed,
            out float t, out Vector2 aimpos)
        {
            if(!LaunchDirection(mypos, tgtpos, tgtRelativeV, projectileSpeed, out t))
            {
                aimpos = Vector2.zero;
                return false;
            }
            
            aimpos = tgtpos + tgtRelativeV * t;
            return true;
        }
        
        
        // ====================================================================================================
        // ====================================================================================================
        
        /// <param name="poly"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="forward"></param>
        /// <param name="angleSize"></param>
        /// <param name="segments"></param>
        /// <param name="clockwise"></param>
        public static IEnumerable<Vector2> GetArcPoly(Vector2 center, float radius, Vector2 forward, float angleSize, int segments, bool clockwise = true)
        {
            forward = forward.normalized * radius;
            var step = angleSize / segments;
            if (clockwise)
            {
                for (var i = 0; i <= segments; i++)
                {
                    var rad = i * step - angleSize / 2;
                    var dir = forward.Rotate(rad);
                    yield return center + dir;
                }
            }
            else
            {
                for (var i = 0; i <= segments; i++)
                {
                    var rad = i * step - angleSize / 2;
                    var dir = forward.Rotate(-rad);
                    yield return center + dir;
                }
            }
        }
        
        public static IEnumerable<Vector2> GetStepPoints(Vector2 from, Vector2 to, float distancePerPoint)
        {
            var dir = from - to;
            var len = dir.magnitude;
            var count = len / distancePerPoint;
            var step = dir / count;
            for (var i = 0; i < count; i++)
                yield return to + step * i;
        }
        
        // 4方向插值: 根据方向向量 d 在四个方向 (左 l, 右 r, 下 b, 上 t) 的最大长度限制中插值, 返回该方向的最大允许长度.
        public static float Interop4DirLen(this Vector2 d, float l, float r, float b, float t)
        {
            const float eps = 1e-6f;
			
			if(l < 0) throw new System.Exception($"l < 0: {l}");
			if(r < 0) throw new System.Exception($"r < 0: {r}");
			if(b < 0) throw new System.Exception($"b < 0: {b}");
			if(t < 0) throw new System.Exception($"t < 0: {t}");
			
			// d 的方向不确定.
            float len2 = d.sqrMagnitude;
            if (len2 < eps * eps) return 0f;
			
			// 在 xy 轴上, 此时直接取同向最大值.
            if (Mathf.Abs(d.x) < eps) return d.y > 0 ? t : b;

            if (Mathf.Abs(d.y) < eps) return d.x > 0 ? r : l;

			// 计算对应象限的四分之一椭圆.
            if (d.x > 0 && d.y > 0)
            {
				if(r == 0 || t == 0) return 0f;
				// var lenOnStandarizedCircle = Mathf.Sqrt(d.x * d.x / (r * r) + d.y * d.y / (t * t));
				// var intersectPoint = d / lenOnStandarizedCircle;
				var lenOnStandarizedCircle2 = d.x * d.x / (r * r) + d.y * d.y / (t * t);
				return Mathf.Sqrt(d.sqrMagnitude / lenOnStandarizedCircle2);
            }
			else if (d.x > 0 && d.y < 0)
			{
				if(r == 0 || b == 0) return 0f;
				// var lenOnStandarizedCircle = Mathf.Sqrt(d.x * d.x / (r * r) + d.y * d.y / (b * b));
				// var intersectPoint = d / lenOnStandarizedCircle;
				var lenOnStandarizedCircle2 = d.x * d.x / (r * r) + d.y * d.y / (b * b);
				return Mathf.Sqrt(d.sqrMagnitude / lenOnStandarizedCircle2);
			}
			else if (d.x < 0 && d.y > 0)
			{
				if(l == 0 || t == 0) return 0f;
				// var lenOnStandarizedCircle = Mathf.Sqrt(d.x * d.x / (l * l) + d.y * d.y / (t * t));
				// var intersectPoint = d / lenOnStandarizedCircle;
				var lenOnStandarizedCircle2 = d.x * d.x / (l * l) + d.y * d.y / (t * t);
				return Mathf.Sqrt(d.sqrMagnitude / lenOnStandarizedCircle2);
			}
			else if (d.x < 0 && d.y < 0)
			{
				if(l == 0 || b == 0) return 0f;
				// var lenOnStandarizedCircle = Mathf.Sqrt(d.x * d.x / (l * l) + d.y * d.y / (b * b));
				// var intersectPoint = d / lenOnStandarizedCircle;
				var lenOnStandarizedCircle2 = d.x * d.x / (l * l) + d.y * d.y / (b * b);
				return Mathf.Sqrt(d.sqrMagnitude / lenOnStandarizedCircle2);
			}
			
			throw new System.Exception($"invalid d: {d}");
        }
        
        // 4方向插值: 根据方向向量 d 在四个方向 (左 l, 右 r, 下 b, 上 t) 的最大长度限制中插值, 返回限制后的向量.
        public static Vector2 Interop4Dir(this Vector2 d, float l, float r, float b, float t)
        {
            float maxLen = d.Interop4DirLen(l, r, b, t);
            return d.WithLength(maxLen);
        }
    
    }
    
    #endregion
    
}
