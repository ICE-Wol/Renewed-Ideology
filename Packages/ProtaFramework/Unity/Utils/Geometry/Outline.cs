using System;
using System.Collections.Generic;
using System.Linq;
using Prota.Unity;
using UnityEngine;
using System.Text;

public struct Outline : IEquatable<Outline>
{
    private const float Eps = 1e-6f;
    // 用于判断顺逆时针的 epsilon，使用 eps 的平方（面积是二维的）
    private const float OrientationEps = Eps * Eps;
    
    public Vector2[] vertices;
    
    public Outline(Vector2[] vertices)
    {
        if (vertices == null || vertices.Length < 3)
            throw new ArgumentException("Outline 至少需要 3 个顶点");
        this.vertices = vertices;
    }
    
    public Outline(IEnumerable<Vector2> vertices)
    {
        var verts = vertices?.ToArray();
        if (verts == null || verts.Length < 3)
            throw new ArgumentException("Outline 至少需要 3 个顶点");
        this.vertices = verts;
    }
    
    public Outline(int vertCount)
    {
        if (vertCount < 3)
            throw new ArgumentException("Outline 至少需要 3 个顶点");
        this.vertices = new Vector2[vertCount];
    }
    
    /// <summary>
    /// 获取顶点数量
    /// </summary>
    public readonly int vertexCount => vertices.Length;
    
    /// <summary>
    /// 获取边的数量（等于顶点数量）
    /// </summary>
    public readonly int edgeCount => vertices.Length;
    
    /// <summary>
    /// 通过索引访问边，edge[i] 是 vertices[i] 和 vertices[(i + 1) % n] 的 Segment
    /// </summary>
    /// <param name="index">边的索引</param>
    /// <returns>对应的边</returns>
    public readonly Segment this[int index]
    {
        get
        {
            if (index < 0 || index >= vertices.Length)
                throw new ArgumentOutOfRangeException(nameof(index), $"索引 {index} 超出范围 [0, {vertices.Length - 1}]");
            
            int nextIndex = (index + 1) % vertices.Length;
            return new Segment(vertices[index], vertices[nextIndex]);
        }
    }
    
    /// <summary>
    /// 计算多边形的有向面积（符号表示顺逆时针）
    /// </summary>
    public readonly float signedArea
    {
        get
        {
            if (isDegenerateToPoint)
                return 0f;
            
            float area = 0f;
            for (int i = 0; i < vertices.Length; i++)
            {
                int j = (i + 1) % vertices.Length;
                area += vertices[i].x * vertices[j].y;
                area -= vertices[j].x * vertices[i].y;
            }
            return area * 0.5f;
        }
    }
    
    /// <summary>
    /// 计算多边形的面积（绝对值）
    /// </summary>
    public readonly float area => Mathf.Abs(signedArea);
    
    /// <summary>
    /// 判断多边形顶点是否按逆时针排列
    /// </summary>
    public readonly bool isCounterClockwise => signedArea > OrientationEps;
    
    /// <summary>
    /// 判断多边形顶点是否按顺时针排列
    /// </summary>
    public readonly bool isClockwise => signedArea < -OrientationEps;
    
    /// <summary>
    /// 判断多边形是否退化到点（所有顶点相同或非常接近）
    /// </summary>
    public readonly bool isDegenerateToPoint
    {
        get
        {
            if (vertices.Length == 0)
                return true;
            
            Vector2 first = vertices[0];
            float epsSq = Eps * Eps;
            
            for (int i = 1; i < vertices.Length; i++)
            {
                float distSq = (vertices[i] - first).sqrMagnitude;
                if (distSq >= epsSq)
                    return false;
            }
            return true;
        }
    }
    
    /// <summary>
    /// 判断多边形是否共线（所有顶点在同一条直线上，但不是退化到点）
    /// </summary>
    public readonly bool isCollinear
    {
        get
        {
            if (isDegenerateToPoint)
                return false;
            
            if (vertices.Length < 3)
                return true;
            
            // 找到前两个不同的点确定直线方向
            Vector2 p0 = vertices[0];
            Vector2 dir = Vector2.zero;
            
            for (int i = 1; i < vertices.Length; i++)
            {
                Vector2 diff = vertices[i] - p0;
                if (diff.sqrMagnitude > Eps * Eps)
                {
                    dir = diff;
                    break;
                }
            }
            
            // 如果所有点都相同，返回 false（退化到点）
            if (dir.sqrMagnitude <= Eps * Eps)
                return false;
            
            // 检查所有其他点是否在这条直线上
            for (int i = 1; i < vertices.Length; i++)
            {
                Vector2 diff = vertices[i] - p0;
                if (diff.sqrMagnitude <= Eps * Eps)
                    continue;
                
                // 使用叉积判断是否共线
                float cross = dir.Cross(diff);
                if (Mathf.Abs(cross) > Eps)
                    return false;
            }
            
            return true;
        }
    }
    
    /// <summary>
    /// 获取自交点位置和自交的两个线段的下标
    /// </summary>
    /// <param name="intersectionPoint">自交点位置</param>
    /// <param name="edgeIndex1">第一个自交线段的索引</param>
    /// <param name="edgeIndex2">第二个自交线段的索引</param>
    /// <returns>如果找到自交点返回 true，否则返回 false</returns>
    public readonly bool GetSelfIntersection(out Vector2 intersectionPoint, out int edgeIndex1, out int edgeIndex2)
    {
        intersectionPoint = Vector2.zero;
        edgeIndex1 = -1;
        edgeIndex2 = -1;
        
        if (vertices.Length < 4)
            return false;
        
        // 获取所有边
        List<Segment> edges = new List<Segment>();
        for (int i = 0; i < vertices.Length; i++)
        {
            int j = (i + 1) % vertices.Length;
            edges.Add(new Segment(vertices[i], vertices[j]));
        }
        
        // 检查非相邻边是否相交
        for (int i = 0; i < edges.Count; i++)
        {
            for (int j = i + 2; j < edges.Count; j++)
            {
                if(j == edges.Count - 1 && i == 0) continue;
                
                if (edges[i].IntersectsWithExclusive(edges[j], out var intersection))
                {
                    intersectionPoint = intersection;
                    edgeIndex1 = i;
                    edgeIndex2 = j;
                    return true;
                }
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 判断多边形是否自交（非相邻边相交）
    /// </summary>
    public readonly bool isSelfIntersecting
    {
        get
        {
            return GetSelfIntersection(out _, out _, out _);
        }
    }
    
    /// <summary>
    /// 判断多边形是否为凸多边形
    /// </summary>
    public readonly bool isConvex
    {
        get
        {
            if (isDegenerateToPoint || isCollinear)
                return false;
            
            if (vertices.Length < 3)
                return false;
            
            if (vertices.Length == 3)
                return true; // 三角形总是凸的
            
            // 检查所有顶点是否都在每条边的同一侧
            // 对于凸多边形，所有顶点应该都在每条边的同一侧（相对于多边形内部）
            bool firstSign = false;
            bool signInitialized = false;
            
            for (int i = 0; i < vertices.Length; i++)
            {
                int j = (i + 1) % vertices.Length;
                int k = (i + 2) % vertices.Length;
                
                Vector2 edge = vertices[j] - vertices[i];
                Vector2 toNext = vertices[k] - vertices[i];
                float cross = edge.Cross(toNext);
                
                if (Mathf.Abs(cross) < Eps)
                    continue; // 共线，跳过
                
                bool isPositive = cross > 0;
                
                if (!signInitialized)
                {
                    firstSign = isPositive;
                    signInitialized = true;
                }
                else if (isPositive != firstSign)
                {
                    return false; // 发现不同符号，说明是凹多边形
                }
            }
            
            return true;
        }
    }
    
    /// <summary>
    /// 判断多边形是否为凹多边形
    /// </summary>
    public readonly bool isConcave
    {
        get
        {
            if (isDegenerateToPoint || isCollinear)
                return false;
            
            return !isConvex;
        }
    }
    
    public bool Equals(Outline other)
    {
        if (vertices == null && other.vertices == null)
            return true;
        if (vertices == null || other.vertices == null)
            return false;
        if (vertices.Length != other.vertices.Length)
            return false;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            if (!vertices[i].Equals(other.vertices[i]))
                return false;
        }
        return true;
    }
    
    public override bool Equals(object obj)
    {
        return obj is Outline other && Equals(other);
    }
    
    public override int GetHashCode()
    {
        if (vertices == null || vertices.Length == 0)
            return 0;
        
        var hashCode = new HashCode();
        for (int i = 0; i < vertices.Length; i++)
        {
            hashCode.Add(vertices[i]);
        }
        return hashCode.ToHashCode();
    }
    
    public override string ToString()
    {
        if (vertices == null || vertices.Length == 0) return "[]";
        var sb = new StringBuilder();
        for (int i = 0; i < vertices.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append(i);
            sb.Append(":");
            sb.Append(vertices[i].ToString());
        }
        return sb.ToString();
    }
    
    public static bool operator ==(Outline left, Outline right) => left.Equals(right);
    public static bool operator !=(Outline left, Outline right) => !left.Equals(right);
    
    /// <summary>
    /// 判断点是否在多边形内部（包括边界）
    /// 使用射线法（Ray Casting Algorithm）
    /// </summary>
    public readonly bool ContainsPoint(Vector2 point, float tolerance = Eps)
    {
        if (isDegenerateToPoint)
            return false;
        
        if (isCollinear)
        {
            // 对于共线情况，检查点是否在任意一条边上
            for (int i = 0; i < vertices.Length; i++)
            {
                int j = (i + 1) % vertices.Length;
                if (new Segment(vertices[i], vertices[j]).ContainsPoint(point, tolerance))
                    return true;
            }
            return false;
        }
        
        // 检查点是否在多边形的任意一条边上
        for (int i = 0; i < vertices.Length; i++)
        {
            int j = (i + 1) % vertices.Length;
            if (new Segment(vertices[i], vertices[j]).ContainsPoint(point, tolerance))
                return true;
        }
        
        // 使用射线法：从点向右发射一条水平射线，统计与多边形边的交点数
        // 奇数个交点表示点在内部，偶数个交点表示点在外部
        // 创建水平射线（从 point 向右延伸）
        Line ray = new Line(point, point + Vector2.right);
        
        int intersectionCount = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            int j = (i + 1) % vertices.Length;
            Segment edge = new Segment(vertices[i], vertices[j]);
            
            // 排除水平边（与射线平行）
            if (Mathf.Abs(edge.delta.y) < tolerance)
                continue;
            
            // 使用 line/segment 相交方法检查射线是否与边相交
            if (edge.IntersectsSegmentWithLine(ray, out Vector2 intersection))
            {
                // 如果交点在点的右侧，计数加一
                if (intersection.x > point.x + tolerance)
                    intersectionCount++;
            }
        }
        
        return (intersectionCount % 2) == 1;
    }
    
    /// <summary>
    /// 判断线段是否完全在多边形内部（包括边界）
    /// </summary>
    public readonly bool ContainsSegment(Segment segment, float tolerance = Eps)
    {
        // 检查两个端点是否都在多边形内
        if (!ContainsPoint(segment.from, tolerance) || !ContainsPoint(segment.to, tolerance))
            return false;
        
        // 检查线段是否与多边形的边相交（不包括边界上的情况）
        for (int i = 0; i < vertices.Length; i++)
        {
            int j = (i + 1) % vertices.Length;
            Segment edge = new Segment(vertices[i], vertices[j]);
            
            // 如果线段与边相交，检查交点是否在边界上
            if (segment.IntersectsWith(edge, out Vector2 intersection))
            {
                // 检查交点是否在线段的端点上（边界情况）
                bool onSegmentEndpoint = (segment.from - intersection).sqrMagnitude < tolerance * tolerance ||
                                         (segment.to - intersection).sqrMagnitude < tolerance * tolerance;
                
                // 检查交点是否在边的端点上（边界情况）
                bool onEdgeEndpoint = (vertices[i] - intersection).sqrMagnitude < tolerance * tolerance ||
                                     (vertices[j] - intersection).sqrMagnitude < tolerance * tolerance;
                
                // 如果线段完全在边上（两个端点都在边上），这是允许的边界情况
                bool segmentOnEdge = edge.ContainsPoint(segment.from, tolerance) && 
                                    edge.ContainsPoint(segment.to, tolerance);
                
                // 如果交点既不在线段端点也不在边端点，且线段不完全在边上，说明线段穿过了多边形的边
                if (!onSegmentEndpoint && !onEdgeEndpoint && !segmentOnEdge)
                    return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 判断三角形是否完全在多边形内部（包括边界）
    /// </summary>
    public readonly bool ContainsTriangle(Triangle triangle, float tolerance = Eps)
    {
        // 检查三个顶点是否都在多边形内
        if (!ContainsPoint(triangle.a, tolerance) || 
            !ContainsPoint(triangle.b, tolerance) || 
            !ContainsPoint(triangle.c, tolerance))
            return false;
        
        // 检查三角形的三条边是否都不与多边形的边相交（不包括边界上的情况）
        var (ab, bc, ca) = triangle.Edges;
        
        if (!ContainsSegment(ab, tolerance))
            return false;
        if (!ContainsSegment(bc, tolerance))
            return false;
        if (!ContainsSegment(ca, tolerance))
            return false;
        
        return true;
    }
}

