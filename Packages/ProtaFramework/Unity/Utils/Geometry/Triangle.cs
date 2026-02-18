using System;
using Prota.Unity;
using UnityEngine;

public struct Triangle : IEquatable<Triangle>, IComparable<Triangle>
{
    public const float Eps = 1e-6f;
    // 用于判断顺逆时针的 epsilon，使用 eps 的平方（面积是二维的）
    public const float OrientationEps = Eps * Eps;
    
    public Vector2 a;
    public Vector2 b;
    public Vector2 c;
    
    public Triangle(Vector2 a, Vector2 b, Vector2 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }
    
    /// <summary>
    /// 计算三角形的有向面积（符号表示顺逆时针）
    /// </summary>
    public readonly float signedArea => (a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y)) * 0.5f;
    
    /// <summary>
    /// 计算三角形的面积（绝对值）
    /// </summary>
    public readonly float area => Mathf.Abs(signedArea);
    
    /// <summary>
    /// 判断三角形顶点是否按逆时针排列
    /// </summary>
    public readonly bool isCounterClockwise => signedArea > OrientationEps;
    
    /// <summary>
    /// 判断三角形顶点是否按顺时针排列
    /// </summary>
    public readonly bool isClockwise => signedArea < -OrientationEps;
    
    /// <summary>
    /// 判断三角形是否退化到点（三个顶点相同或非常接近）
    /// </summary>
    public readonly bool isDegenerateToPoint
    {
        get
        {
            float distAB = (a - b).sqrMagnitude;
            float distBC = (b - c).sqrMagnitude;
            float distCA = (c - a).sqrMagnitude;
            float epsSq = Eps * Eps;
            return distAB < epsSq && distBC < epsSq && distCA < epsSq;
        }
    }
    
    /// <summary>
    /// 判断三角形是否共线（三个顶点在同一条直线上，但不是退化到点）
    /// </summary>
    public readonly bool isCollinear
    {
        get
        {
            if (isDegenerateToPoint)
                return false;
            return area < OrientationEps;
        }
    }
    
    /// <summary>
    /// 获取三角形的中心点(几何中心)
    /// </summary>
    public readonly Vector2 center => (a + b + c) / 3f;
    
    /// <summary>
    /// 向前旋转顶点编号：a->b->c 变成 b->c->a
    /// </summary>
    public readonly Triangle rotatedForward => new(b, c, a);
    
    /// <summary>
    /// 向后旋转顶点编号：a->b->c 变成 c->a->b
    /// </summary>
    public readonly Triangle rotatedBackward => new(c, a, b);
    
    /// <summary>
    /// 反转三角形顶点顺序：(a, b, c) => (a, c, b)
    /// </summary>
    public readonly Triangle reversedTriangle => new(a, c, b);
    
    public bool Equals(Triangle other)
    {
        // 三角形相等需要三个顶点都相等且顺序相同
        return a.Equals(other.a) && b.Equals(other.b) && c.Equals(other.c);
    }
    
    public override bool Equals(object obj)
    {
        return obj is Triangle other && Equals(other);
    }
    
    public override int GetHashCode()
    {
        // 哈希值考虑顶点顺序
        return HashCode.Combine(a, b, c);
    }
    
    /// <summary>
    /// 判断两个三角形是否具有相同的顶点（不考虑顺序）
    /// </summary>
    public bool SameTriangle(Triangle other)
    {
        // 三角形相同需要三个顶点都相等（考虑顶点顺序的排列）
        return (a.Equals(other.a) && b.Equals(other.b) && c.Equals(other.c)) ||
               (a.Equals(other.a) && b.Equals(other.c) && c.Equals(other.b)) ||
               (a.Equals(other.b) && b.Equals(other.a) && c.Equals(other.c)) ||
               (a.Equals(other.b) && b.Equals(other.c) && c.Equals(other.a)) ||
               (a.Equals(other.c) && b.Equals(other.a) && c.Equals(other.b)) ||
               (a.Equals(other.c) && b.Equals(other.b) && c.Equals(other.a));
    }
    
    public int CompareTo(Triangle other)
    {
        if (a.x.CompareTo(other.a.x) != 0) return a.x.CompareTo(other.a.x);
        if (a.y.CompareTo(other.a.y) != 0) return a.y.CompareTo(other.a.y);
        if (b.x.CompareTo(other.b.x) != 0) return b.x.CompareTo(other.b.x);
        if (b.y.CompareTo(other.b.y) != 0) return b.y.CompareTo(other.b.y);
        if (c.x.CompareTo(other.c.x) != 0) return c.x.CompareTo(other.c.x);
        if (c.y.CompareTo(other.c.y) != 0) return c.y.CompareTo(other.c.y);
        return 0;
    }
    
    public static bool operator ==(Triangle left, Triangle right) => left.Equals(right);
    public static bool operator !=(Triangle left, Triangle right) => !left.Equals(right);
    
    /// <summary>
    /// 计算点在边的哪一侧（使用叉积）
    /// </summary>
    private static float CrossProduct(Vector2 v1, Vector2 v2)
    {
        return v1.x * v2.y - v1.y * v2.x;
    }
    
    /// <summary>
    /// 判断点是否在三角形内部（包括边界）
    /// 使用"点在所有边同侧"的方法判定
    /// </summary>
    public bool ContainsPoint(Vector2 point, float tolerance = Eps)
    {
        if (isDegenerateToPoint)
            throw new InvalidOperationException("三角形退化到点，无法判断点是否在三角形内部");
        if (isCollinear)
            throw new InvalidOperationException("三角形共线，无法判断点是否在三角形内部");
        
        // 计算点相对于三条边的位置（使用叉积）
        // 边 AB：从 A 到 B，点 P 在边的哪一侧
        Vector2 ab = b - a;
        Vector2 ap = point - a;
        float crossAB = ab.Cross(ap);
        
        // 边 BC：从 B 到 C，点 P 在边的哪一侧
        Vector2 bc = c - b;
        Vector2 bp = point - b;
        float crossBC = bc.Cross(bp);
        
        // 边 CA：从 C 到 A，点 P 在边的哪一侧
        Vector2 ca = a - c;
        Vector2 cp = point - c;
        float crossCA = ca.Cross(cp);
        
        // 检查点是否在边上（叉积接近0）
        if (Mathf.Abs(crossAB) < tolerance && new Segment(a, b).ContainsPoint(point, tolerance))
            return true;
        if (Mathf.Abs(crossBC) < tolerance && new Segment(b, c).ContainsPoint(point, tolerance))
            return true;
        if (Mathf.Abs(crossCA) < tolerance && new Segment(c, a).ContainsPoint(point, tolerance))
            return true;
        
        // 检查所有叉积是否同号（都在边的同一侧）
        // 如果三角形是逆时针，点在内部时所有叉积应该都是正的
        // 如果三角形是顺时针，点在内部时所有叉积应该都是负的
        bool allPositive = crossAB >= -tolerance && crossBC >= -tolerance && crossCA >= -tolerance;
        bool allNegative = crossAB <= tolerance && crossBC <= tolerance && crossCA <= tolerance;
        
        return allPositive || allNegative;
    }
    
    /// <summary>
    /// 判断点是否在三角形内部（不包括边界）
    /// 使用"点在所有边同侧"的方法判定
    /// </summary>
    public bool ContainsPointExclusive(Vector2 point, float tolerance = Eps)
    {
        if (isDegenerateToPoint)
            throw new InvalidOperationException("三角形退化到点，无法判断点是否在三角形内部");
        if (isCollinear)
            throw new InvalidOperationException("三角形共线，无法判断点是否在三角形内部");
        
        // 计算点相对于三条边的位置（使用叉积）
        Vector2 ab = b - a;
        Vector2 ap = point - a;
        float crossAB = ab.Cross(ap);
        
        Vector2 bc = c - b;
        Vector2 bp = point - b;
        float crossBC = bc.Cross(bp);
        
        Vector2 ca = a - c;
        Vector2 cp = point - c;
        float crossCA = ca.Cross(cp);
        
        // 如果点在边界上，返回false
        if (Mathf.Abs(crossAB) < tolerance || Mathf.Abs(crossBC) < tolerance || Mathf.Abs(crossCA) < tolerance)
            return false;
        
        // 检查所有叉积是否同号且严格同侧（不在边界上）
        bool allPositive = crossAB > tolerance && crossBC > tolerance && crossCA > tolerance;
        bool allNegative = crossAB < -tolerance && crossBC < -tolerance && crossCA < -tolerance;
        
        return allPositive || allNegative;
    }
    
    
    /// <summary>
    /// 获取三角形的三条边
    /// </summary>
    public readonly (Segment ab, Segment bc, Segment ca) Edges => 
        (new Segment(a, b), new Segment(b, c), new Segment(c, a));
}

