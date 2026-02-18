using System;
using System.Collections.Generic;
using UnityEngine;
using Prota;

namespace Prota.Unity
{
    /// <summary>
    /// 多边形三角化工具类，使用耳切法(Ear Clipping)进行三角化，支持凹多边形
    /// 使用 geometry 系列类实现
    /// </summary>
    public class Triangulator
    {
        private const float Eps = 1e-6f;
        
        /// <summary>
        /// 要三角化的多边形
        /// </summary>
        public Outline outline { get; private set; }
        
        /// <summary>
        /// 三角剖分结果：三角形索引列表（每三个索引组成一个三角形）
        /// </summary>
        public List<int> triangles { get; private set; }
        
        /// <summary>
        /// 是否已经执行过三角剖分
        /// </summary>
        public bool isTriangulated { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="outline">要三角化的多边形</param>
        public Triangulator(Outline outline)
        {
            this.outline = outline;
            this.triangles = new List<int>();
            this.isTriangulated = false;
        }
        
        /// <summary>
        /// 执行三角剖分
        /// </summary>
        public void Triangulate()
        {
            if (outline.vertexCount < 3)
                throw new ArgumentException("Outline 至少需要 3 个顶点");
            
            if (outline.isDegenerateToPoint)
                throw new InvalidOperationException("Outline 退化到点，无法进行三角剖分");
            
            if (outline.isCollinear)
                throw new InvalidOperationException("Outline 共线，无法进行三角剖分");
            
            if (outline.isSelfIntersecting)
            {
                if (outline.GetSelfIntersection(out Vector2 intersectionPoint, out int edgeIndex1, out int edgeIndex2))
                {
                    int nextIndex1 = (edgeIndex1 + 1) % outline.vertexCount;
                    int nextIndex2 = (edgeIndex2 + 1) % outline.vertexCount;
                    Vector2 v1 = outline.vertices[edgeIndex1];
                    Vector2 v2 = outline.vertices[nextIndex1];
                    Vector2 v3 = outline.vertices[edgeIndex2];
                    Vector2 v4 = outline.vertices[nextIndex2];
                    throw new InvalidOperationException(
                        $"Outline 自相交，无法进行三角剖分。\n自交点位置: ({intersectionPoint.x:F4}, {intersectionPoint.y:F4})，" +
                        $"\n自交线段: 边[{edgeIndex1}](顶点{edgeIndex1}({v1.x:F4}, {v1.y:F4})->顶点{nextIndex1}({v2.x:F4}, {v2.y:F4})) " +
                        $"\n与 边[{edgeIndex2}](顶点{edgeIndex2}({v3.x:F4}, {v3.y:F4})->顶点{nextIndex2}({v4.x:F4}, {v4.y:F4}))" +
                        $"\noutline: {outline}");
                }
                else
                {
                    throw new InvalidOperationException("Outline 自相交，无法进行三角剖分（无法获取详细自交信息）");
                }
            }
            
            triangles.Clear();
            
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
            
            // 如果只有3个顶点,直接返回
            if (normalizedOutline.vertexCount == 3)
            {
                triangles.Add(0);
                triangles.Add(1);
                triangles.Add(2);
                isTriangulated = true;
                return;
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
                    triangles.Add(prevNode.Value);
                    triangles.Add(currentNode.Value);
                    triangles.Add(nextNode.Value);
                    var nodeToRemove = currentNode;
                    currentNode = nextNode;
                    vertexIndices.Remove(nodeToRemove);
                    // 如果移除后链表为空或只剩2个节点,退出循环
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
                triangles.Add(node.Value);
                node = node.Next;
                triangles.Add(node.Value);
                node = node.Next;
                triangles.Add(node.Value);
            }
            else if (vertexIndices.Count > 3)
            {
                throw new InvalidOperationException($"三角剖分失败: 剩余 {vertexIndices.Count} 个顶点。多边形可能无效或算法遇到问题。");
            }
            
            isTriangulated = true;
        }
        
        /// <summary>
        /// 判断三个顶点是否构成耳朵（可以安全移除的三角形）
        /// </summary>
        private bool IsEar(
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
            
            // 检查是否有其他顶点在这个三角形内部（不包括边界）
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
        /// 获取三角形对象列表（基于索引列表）
        /// </summary>
        /// <returns>三角形对象列表</returns>
        public List<Triangle> GetTriangles()
        {
            if (!isTriangulated)
                throw new InvalidOperationException("尚未执行三角剖分，请先调用 Triangulate()");
            
            var result = new List<Triangle>();
            Outline normalizedOutline = outline;
            if (!outline.isCounterClockwise)
            {
                var reversedVertices = new Vector2[outline.vertices.Length];
                for (int i = 0; i < outline.vertices.Length; i++)
                {
                    reversedVertices[i] = outline.vertices[outline.vertices.Length - 1 - i];
                }
                normalizedOutline = new Outline(reversedVertices);
            }
            
            for (int i = 0; i < triangles.Count; i += 3)
            {
                var triangle = new Triangle(
                    normalizedOutline.vertices[triangles[i]],
                    normalizedOutline.vertices[triangles[i + 1]],
                    normalizedOutline.vertices[triangles[i + 2]]
                );
                result.Add(triangle);
            }
            
            return result;
        }
    }
}

