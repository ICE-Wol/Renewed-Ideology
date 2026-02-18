using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用 geometry 系列类实现的三角剖分器
/// 使用耳切法(Ear Clipping)进行三角化，支持凹多边形
/// </summary>
public static class GeometryTriangulator
{
    private const float Eps = 1e-6f;
    
    /// <summary>
    /// 对 Outline 进行三角剖分，返回三角形列表
    /// </summary>
    /// <param name="outline">要三角化的多边形</param>
    /// <returns>三角形列表</returns>
    public static List<Triangle> Triangulate(Outline outline)
    {
        if (outline.vertexCount < 3)
            throw new ArgumentException("Outline 至少需要 3 个顶点");
        
        if (outline.isDegenerateToPoint)
            throw new InvalidOperationException("Outline 退化到点，无法进行三角剖分");
        
        if (outline.isCollinear)
            throw new InvalidOperationException("Outline 共线，无法进行三角剖分");
        
        if (outline.isSelfIntersecting)
            throw new InvalidOperationException("Outline 自相交，无法进行三角剖分");
        
        var result = new List<Triangle>();
        
        // 如果只有3个顶点，直接返回一个三角形
        if (outline.vertexCount == 3)
        {
            var triangle = new Triangle(outline.vertices[0], outline.vertices[1], outline.vertices[2]);
            // 确保三角形是逆时针的
            if (!triangle.isCounterClockwise)
            {
                triangle = triangle.reversedTriangle;
            }
            result.Add(triangle);
            return result;
        }
        
        // 确保多边形是逆时针的
        Outline normalizedOutline = outline;
        if (!outline.isCounterClockwise)
        {
            // 反转顶点顺序
            var reversedVertices = new Vector2[outline.vertices.Length];
            for (int i = 0; i < outline.vertices.Length; i++)
            {
                reversedVertices[i] = outline.vertices[outline.vertices.Length - 1 - i];
            }
            normalizedOutline = new Outline(reversedVertices);
        }
        
        // 使用链表存储顶点索引，方便删除
        var vertexIndices = new LinkedList<int>();
        for (int i = 0; i < normalizedOutline.vertices.Length; i++)
        {
            vertexIndices.AddLast(i);
        }
        
        // 使用耳切法进行三角化
        var currentNode = vertexIndices.First;
        int maxIterations = normalizedOutline.vertices.Length * normalizedOutline.vertices.Length;
        int iterations = 0;
        
        while (vertexIndices.Count > 3 && iterations < maxIterations)
        {
            iterations++;
            
            var prevNode = currentNode == vertexIndices.First ? vertexIndices.Last : currentNode.Previous;
            var nextNode = currentNode == vertexIndices.Last ? vertexIndices.First : currentNode.Next;
            
            if (IsEar(normalizedOutline, vertexIndices, prevNode, currentNode, nextNode))
            {
                // 找到耳朵，创建三角形并移除当前顶点
                var triangle = new Triangle(
                    normalizedOutline.vertices[prevNode.Value],
                    normalizedOutline.vertices[currentNode.Value],
                    normalizedOutline.vertices[nextNode.Value]
                );
                result.Add(triangle);
                
                var nodeToRemove = currentNode;
                currentNode = nextNode;
                vertexIndices.Remove(nodeToRemove);
                
                if (vertexIndices.Count <= 2)
                    break;
            }
            else
            {
                currentNode = nextNode;
            }
        }
        
        // 处理最后三个顶点
        if (vertexIndices.Count == 3)
        {
            var node = vertexIndices.First;
            var triangle = new Triangle(
                normalizedOutline.vertices[node.Value],
                normalizedOutline.vertices[node.Next.Value],
                normalizedOutline.vertices[node.Next.Next.Value]
            );
            result.Add(triangle);
        }
        else if (vertexIndices.Count > 3)
        {
            throw new InvalidOperationException($"三角剖分失败: 剩余 {vertexIndices.Count} 个顶点。多边形可能无效或算法遇到问题。");
        }
        
        return result;
    }
    
    /// <summary>
    /// 判断三个顶点是否构成耳朵（可以安全移除的三角形）
    /// </summary>
    private static bool IsEar(
        Outline outline,
        LinkedList<int> vertexIndices,
        LinkedListNode<int> prevNode,
        LinkedListNode<int> currentNode,
        LinkedListNode<int> nextNode)
    {
        var prev = outline.vertices[prevNode.Value];
        var curr = outline.vertices[currentNode.Value];
        var next = outline.vertices[nextNode.Value];
        
        // 创建三角形
        var triangle = new Triangle(prev, curr, next);
        
        // 检查三角形是否有效（不退化、不共线）
        if (triangle.isDegenerateToPoint || triangle.isCollinear)
            return false;
        
        // 检查三角形是否是凸的（逆时针）
        if (!triangle.isCounterClockwise)
            return false;
        
        // 检查是否有其他顶点在这个三角形内部（包括边界, 避免耳朵碰到其它的顶点）
        foreach (var index in vertexIndices)
        {
            if (index == prevNode.Value || index == currentNode.Value || index == nextNode.Value)
                continue;
            
            var point = outline.vertices[index];
            if (triangle.ContainsPoint(point, Eps))
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 对 Outline 进行三角剖分，返回三角形数组
    /// </summary>
    /// <param name="outline">要三角化的多边形</param>
    /// <returns>三角形数组</returns>
    public static Triangle[] TriangulateToArray(Outline outline)
    {
        var triangles = Triangulate(outline);
        return triangles.ToArray();
    }
}

