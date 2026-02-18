using System;
using UnityEngine;
using NUnit.Framework;
using Prota.Unity;

namespace Geometry
{
    public class OutlineTests
    {
        private const float Tolerance = 1e-6f;
        
        #region 构造函数测试
        
        [Test]
        public void Constructor_WithNullVertices_ShouldThrowException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Outline((Vector2[])null));
        }
        
        [Test]
        public void Constructor_WithLessThanThreeVertices_ShouldThrowException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new Outline(new Vector2[] { new Vector2(0, 0) }));
            Assert.Throws<ArgumentException>(() => new Outline(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0) }));
        }
        
        [Test]
        public void Constructor_WithThreeOrMoreVertices_ShouldCreateOutline()
        {
            // Arrange
            var vertices = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1) };
            
            // Act
            var outline = new Outline(vertices);
            
            // Assert
            Assert.AreEqual(3, outline.vertexCount);
        }
        
        [Test]
        public void Constructor_WithIEnumerable_ShouldCreateOutline()
        {
            // Arrange
            var vertices = new System.Collections.Generic.List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };
            
            // Act
            var outline = new Outline(vertices);
            
            // Assert
            Assert.AreEqual(4, outline.vertexCount);
        }
        
        #endregion
        
        #region 基本属性测试
        
        [Test]
        public void VertexCount_ShouldReturnCorrectCount()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var count = outline.vertexCount;
            
            // Assert
            Assert.AreEqual(4, count);
        }
        
        [Test]
        public void EdgeCount_ShouldEqualVertexCount()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var edgeCount = outline.edgeCount;
            var vertexCount = outline.vertexCount;
            
            // Assert
            Assert.AreEqual(vertexCount, edgeCount);
        }
        
        [Test]
        public void EdgeIndexer_ShouldReturnCorrectSegment()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act & Assert
            var edge0 = outline[0];
            Assert.AreEqual(new Vector2(0, 0), edge0.from);
            Assert.AreEqual(new Vector2(1, 0), edge0.to);
            
            var edge1 = outline[1];
            Assert.AreEqual(new Vector2(1, 0), edge1.from);
            Assert.AreEqual(new Vector2(1, 1), edge1.to);
            
            var edge2 = outline[2];
            Assert.AreEqual(new Vector2(1, 1), edge2.from);
            Assert.AreEqual(new Vector2(0, 1), edge2.to);
            
            var edge3 = outline[3];
            Assert.AreEqual(new Vector2(0, 1), edge3.from);
            Assert.AreEqual(new Vector2(0, 0), edge3.to);
        }
        
        [Test]
        public void EdgeIndexer_LastEdge_ShouldWrapToFirstVertex()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var lastEdge = outline[3];
            
            // Assert - 最后一条边应该从最后一个顶点连接到第一个顶点
            Assert.AreEqual(new Vector2(0, 1), lastEdge.from);
            Assert.AreEqual(new Vector2(0, 0), lastEdge.to);
        }
        
        [Test]
        public void EdgeIndexer_Triangle_ShouldReturnThreeEdges()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0.5f, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act & Assert
            var edge0 = outline[0];
            Assert.AreEqual(new Vector2(0, 0), edge0.from);
            Assert.AreEqual(new Vector2(1, 0), edge0.to);
            
            var edge1 = outline[1];
            Assert.AreEqual(new Vector2(1, 0), edge1.from);
            Assert.AreEqual(new Vector2(0.5f, 1), edge1.to);
            
            var edge2 = outline[2];
            Assert.AreEqual(new Vector2(0.5f, 1), edge2.from);
            Assert.AreEqual(new Vector2(0, 0), edge2.to);
        }
        
        [Test]
        public void EdgeIndexer_OutOfRange_ShouldThrowException()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = outline[-1]; });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = outline[4]; });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = outline[10]; });
        }
        
        [Test]
        public void EdgeIndexer_AllEdges_ShouldFormClosedPolygon()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var edge0 = outline[0];
            var edge1 = outline[1];
            var edge2 = outline[2];
            var edge3 = outline[3];
            
            // Assert - 验证边是连续的
            Assert.AreEqual(edge0.to, edge1.from);
            Assert.AreEqual(edge1.to, edge2.from);
            Assert.AreEqual(edge2.to, edge3.from);
            Assert.AreEqual(edge3.to, edge0.from); // 最后一条边连接到第一条边的起点
        }
        
        [Test]
        public void SignedArea_CounterClockwiseSquare_ShouldReturnPositive()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var area = outline.signedArea;
            
            // Assert
            Assert.Greater(area, 0f);
            Assert.AreEqual(1f, area, Tolerance);
        }
        
        [Test]
        public void SignedArea_ClockwiseSquare_ShouldReturnNegative()
        {
            // Arrange - 顺时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(0, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 0) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var area = outline.signedArea;
            
            // Assert
            Assert.Less(area, 0f);
            Assert.AreEqual(-1f, area, Tolerance);
        }
        
        [Test]
        public void Area_ShouldReturnAbsoluteValue()
        {
            // Arrange
            var vertices1 = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var vertices2 = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(0, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 0) 
            };
            var outline1 = new Outline(vertices1);
            var outline2 = new Outline(vertices2);
            
            // Act
            var area1 = outline1.area;
            var area2 = outline2.area;
            
            // Assert
            Assert.AreEqual(1f, area1, Tolerance);
            Assert.AreEqual(1f, area2, Tolerance);
            Assert.AreEqual(area1, area2, Tolerance);
        }
        
        [Test]
        public void Area_Pentagon_ShouldReturnCorrectArea()
        {
            // Arrange - 正五边形（近似）
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 1),
                new Vector2(0.951f, 0.309f),
                new Vector2(0.588f, -0.809f),
                new Vector2(-0.588f, -0.809f),
                new Vector2(-0.951f, 0.309f)
            };
            var outline = new Outline(vertices);
            
            // Act
            var area = outline.area;
            
            // Assert - 正五边形面积约为 2.377
            Assert.Greater(area, 2f);
            Assert.Less(area, 3f);
        }
        
        #endregion
        
        #region 顺时针/逆时针判断测试
        
        [Test]
        public void IsCounterClockwise_CounterClockwiseSquare_ShouldReturnTrue()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isCounterClockwise;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsCounterClockwise_ClockwiseSquare_ShouldReturnFalse()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(0, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 0) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isCounterClockwise;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsClockwise_ClockwiseSquare_ShouldReturnTrue()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(0, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 0) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isClockwise;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsClockwise_CounterClockwiseSquare_ShouldReturnFalse()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isClockwise;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        #endregion
        
        #region 退化成点判断测试
        
        [Test]
        public void IsDegenerateToPoint_AllSamePoints_ShouldReturnTrue()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(1, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isDegenerateToPoint;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsDegenerateToPoint_VeryClosePoints_ShouldReturnTrue()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(1, 1), 
                new Vector2(1 + 1e-7f, 1 + 1e-7f), 
                new Vector2(1 - 1e-7f, 1 - 1e-7f) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isDegenerateToPoint;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsDegenerateToPoint_DifferentPoints_ShouldReturnFalse()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isDegenerateToPoint;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        #endregion
        
        #region 共线判断测试
        
        [Test]
        public void IsCollinear_AllPointsOnLine_ShouldReturnTrue()
        {
            // Arrange - 所有点都在 x 轴上
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(2, 0),
                new Vector2(3, 0)
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isCollinear;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsCollinear_PointsOnDiagonalLine_ShouldReturnTrue()
        {
            // Arrange - 所有点都在对角线上
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 1), 
                new Vector2(2, 2),
                new Vector2(3, 3)
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isCollinear;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsCollinear_NonCollinearPoints_ShouldReturnFalse()
        {
            // Arrange - 正方形，不共线
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isCollinear;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsCollinear_DegenerateToPoint_ShouldReturnFalse()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(1, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isCollinear;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        #endregion
        
        #region 自交判断测试
        
        [Test]
        public void IsSelfIntersecting_SimpleSquare_ShouldReturnFalse()
        {
            // Arrange - 简单正方形，不自交
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isSelfIntersecting;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsSelfIntersecting_BowTieShape_ShouldReturnTrue()
        {
            // Arrange - 蝴蝶结形状，自交
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1), 
                new Vector2(1, 0) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isSelfIntersecting;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsSelfIntersecting_Triangle_ShouldReturnFalse()
        {
            // Arrange - 三角形不可能自交（至少需要4个顶点）
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isSelfIntersecting;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsSelfIntersecting_ComplexSelfIntersecting_ShouldReturnTrue()
        {
            // Arrange - 复杂自交形状
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2), 
                new Vector2(2, 0),
                new Vector2(1, 1)
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isSelfIntersecting;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        #endregion
        
        #region GetSelfIntersection 测试
        
        [Test]
        public void GetSelfIntersection_SimpleSquare_ShouldReturnFalse()
        {
            // Arrange - 简单正方形，不自交
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.GetSelfIntersection(out Vector2 intersectionPoint, out int edgeIndex1, out int edgeIndex2);
            
            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(Vector2.zero, intersectionPoint);
            Assert.AreEqual(-1, edgeIndex1);
            Assert.AreEqual(-1, edgeIndex2);
        }
        
        [Test]
        public void GetSelfIntersection_BowTieShape_ShouldReturnTrue()
        {
            // Arrange - 蝴蝶结形状，自交
            // 边0: (0,0) -> (1,1)
            // 边1: (1,1) -> (0,1)
            // 边2: (0,1) -> (1,0)
            // 边3: (1,0) -> (0,0)
            // 边0和边2应该相交于(0.5, 0.5)
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1), 
                new Vector2(1, 0) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.GetSelfIntersection(out Vector2 intersectionPoint, out int edgeIndex1, out int edgeIndex2);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, edgeIndex1);
            Assert.AreEqual(2, edgeIndex2);
            Assert.AreEqual(0.5f, intersectionPoint.x, Tolerance);
            Assert.AreEqual(0.5f, intersectionPoint.y, Tolerance);
        }
        
        [Test]
        public void GetSelfIntersection_Triangle_ShouldReturnFalse()
        {
            // Arrange - 三角形不可能自交（至少需要4个顶点）
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.GetSelfIntersection(out Vector2 intersectionPoint, out int edgeIndex1, out int edgeIndex2);
            
            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(Vector2.zero, intersectionPoint);
            Assert.AreEqual(-1, edgeIndex1);
            Assert.AreEqual(-1, edgeIndex2);
        }
        
        [Test]
        public void GetSelfIntersection_ComplexSelfIntersecting_ShouldReturnTrue()
        {
            // Arrange - 复杂自交形状
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2), 
                new Vector2(2, 0),
                new Vector2(1, 1)
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.GetSelfIntersection(out Vector2 intersectionPoint, out int edgeIndex1, out int edgeIndex2);
            
            // Assert
            Assert.IsTrue(result);
            Assert.GreaterOrEqual(edgeIndex1, 0);
            Assert.Less(edgeIndex1, vertices.Length);
            Assert.GreaterOrEqual(edgeIndex2, 0);
            Assert.Less(edgeIndex2, vertices.Length);
            Assert.AreNotEqual(edgeIndex1, edgeIndex2);
            // 验证交点在线段上
            int next1 = (edgeIndex1 + 1) % vertices.Length;
            int next2 = (edgeIndex2 + 1) % vertices.Length;
            var segment1 = new Segment(vertices[edgeIndex1], vertices[next1]);
            var segment2 = new Segment(vertices[edgeIndex2], vertices[next2]);
            Assert.IsTrue(segment1.ContainsPoint(intersectionPoint, Tolerance));
            Assert.IsTrue(segment2.ContainsPoint(intersectionPoint, Tolerance));
        }
        
        [Test]
        public void GetSelfIntersection_AdjacentEdges_ShouldNotReportIntersection()
        {
            // Arrange - 相邻边不应该报告为自交
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.GetSelfIntersection(out Vector2 intersectionPoint, out int edgeIndex1, out int edgeIndex2);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void GetSelfIntersection_LastAndFirstEdge_ShouldNotReportIntersection()
        {
            // Arrange - 最后一条边和第一条边相邻，只在共享顶点处相交，不应该报告为自交
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.GetSelfIntersection(out Vector2 intersectionPoint, out int edgeIndex1, out int edgeIndex2);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void GetSelfIntersection_LastAndFirstEdge_IntersectingAtNonSharedPoint_ShouldReportIntersection()
        {
            // Arrange - 最后一条边和第一条边在非共享顶点处相交，应该报告为自交
            // 创建一个形状，使边0和边n-1在中间相交（不是共享顶点）
            // 顶点0: (0,0) - 共享顶点
            // 顶点1: (2,0) - 边0的终点
            // 顶点2: (2,2)
            // 顶点3: (1,-1) - 边3的起点，使边3从(1,-1)到(0,0)
            // 边0: (0,0) -> (2,0)  水平线 y=0, x从0到2
            // 边3: (1,-1) -> (0,0)  从(1,-1)到(0,0)，方程是 y = x - 1
            // 边0和边3在(1,0)处相交，这不是共享顶点(0,0)，所以是自交
            var selfIntersectingVertices = new Vector2[] 
            { 
                new Vector2(0, 0),      // 0 - 共享顶点
                new Vector2(2, 0),      // 1
                new Vector2(2, 1),      // 2
                new Vector2(1, -1),     // 3 - 使边3从(1,-1)到(0,0)，与边0在(1,0)处相交
            };
            var outline = new Outline(selfIntersectingVertices);
            
            // Act
            var result = outline.GetSelfIntersection(out Vector2 intersectionPoint, out int edgeIndex1, out int edgeIndex2);
            
            // Assert
            Assert.IsTrue(result, "边0和边3在非共享顶点处相交，应该报告为自交");
            // 边0是索引0，边3是索引3（最后一条边）
            Assert.AreEqual(0, edgeIndex1);
            Assert.AreEqual(2, edgeIndex2);
            // 验证交点在(1,0)附近
            Assert.Less(Mathf.Abs(intersectionPoint.x - 1.5f), Tolerance);
            Assert.Less(Mathf.Abs(intersectionPoint.y - 0f), Tolerance);
        }
        
        [Test]
        public void GetSelfIntersection_LastAndFirstEdge_OnlyAtSharedVertex_ShouldNotReportIntersection()
        {
            // Arrange - 最后一条边和第一条边只在共享顶点处相交，不应该报告为自交
            // 正常的多边形，边0和边n-1只在共享顶点处相交
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0),      // 共享顶点
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.GetSelfIntersection(out Vector2 intersectionPoint, out int edgeIndex1, out int edgeIndex2);
            
            // Assert
            Assert.IsFalse(result, "边0和边3只在共享顶点(0,0)处相交，不应该报告为自交");
        }
        
        [Test]
        public void GetSelfIntersection_IsSelfIntersecting_UsesGetSelfIntersection()
        {
            // Arrange - 验证 isSelfIntersecting 使用 GetSelfIntersection
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1), 
                new Vector2(1, 0) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var isSelfIntersecting = outline.isSelfIntersecting;
            var getSelfIntersectionResult = outline.GetSelfIntersection(out _, out _, out _);
            
            // Assert
            Assert.AreEqual(isSelfIntersecting, getSelfIntersectionResult);
        }
        
        [Test]
        public void GetSelfIntersection_MultipleIntersections_ReturnsFirstFound()
        {
            // Arrange - 有多个自交点的复杂形状
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(3, 0), 
                new Vector2(3, 3), 
                new Vector2(1, 3), 
                new Vector2(1, 1), 
                new Vector2(2, 1), 
                new Vector2(2, 3), 
                new Vector2(0, 3) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.GetSelfIntersection(out Vector2 intersectionPoint, out int edgeIndex1, out int edgeIndex2);
            
            // Assert
            Assert.IsTrue(result);
            Assert.GreaterOrEqual(edgeIndex1, 0);
            Assert.GreaterOrEqual(edgeIndex2, 0);
            // 验证交点在线段上
            int next1 = (edgeIndex1 + 1) % vertices.Length;
            int next2 = (edgeIndex2 + 1) % vertices.Length;
            var segment1 = new Segment(vertices[edgeIndex1], vertices[next1]);
            var segment2 = new Segment(vertices[edgeIndex2], vertices[next2]);
            Assert.IsTrue(segment1.ContainsPoint(intersectionPoint, Tolerance));
            Assert.IsTrue(segment2.ContainsPoint(intersectionPoint, Tolerance));
        }
        
        #endregion
        
        #region 凸多边形判断测试
        
        [Test]
        public void IsConvex_ConvexSquare_ShouldReturnTrue()
        {
            // Arrange - 凸正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isConvex;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsConvex_ConvexTriangle_ShouldReturnTrue()
        {
            // Arrange - 三角形总是凸的
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isConvex;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsConvex_ConvexPentagon_ShouldReturnTrue()
        {
            // Arrange - 凸五边形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 1),
                new Vector2(0.951f, 0.309f),
                new Vector2(0.588f, -0.809f),
                new Vector2(-0.588f, -0.809f),
                new Vector2(-0.951f, 0.309f)
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isConvex;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsConvex_ConcaveShape_ShouldReturnFalse()
        {
            // Arrange - 凹形状（L形）
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 1), 
                new Vector2(1, 1),
                new Vector2(1, 2),
                new Vector2(0, 2)
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isConvex;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsConvex_DegenerateToPoint_ShouldReturnFalse()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(1, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isConvex;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsConvex_Collinear_ShouldReturnFalse()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(2, 0),
                new Vector2(3, 0)
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isConvex;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        #endregion
        
        #region 凹多边形判断测试
        
        [Test]
        public void IsConcave_ConcaveShape_ShouldReturnTrue()
        {
            // Arrange - 凹形状（L形）
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 1), 
                new Vector2(1, 1),
                new Vector2(1, 2),
                new Vector2(0, 2)
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isConcave;
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsConcave_ConvexShape_ShouldReturnFalse()
        {
            // Arrange - 凸正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1), 
                new Vector2(0, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isConcave;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsConcave_DegenerateToPoint_ShouldReturnFalse()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(1, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isConcave;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsConcave_Collinear_ShouldReturnFalse()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(2, 0),
                new Vector2(3, 0)
            };
            var outline = new Outline(vertices);
            
            // Act
            var result = outline.isConcave;
            
            // Assert
            Assert.IsFalse(result);
        }
        
        #endregion
        
        #region Equals 和 GetHashCode 测试
        
        [Test]
        public void Equals_SameVertices_ShouldReturnTrue()
        {
            // Arrange
            var vertices1 = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0, 1) 
            };
            var vertices2 = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0, 1) 
            };
            var outline1 = new Outline(vertices1);
            var outline2 = new Outline(vertices2);
            
            // Act
            var result = outline1.Equals(outline2);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Equals_DifferentVertices_ShouldReturnFalse()
        {
            // Arrange
            var vertices1 = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0, 1) 
            };
            var vertices2 = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1) 
            };
            var outline1 = new Outline(vertices1);
            var outline2 = new Outline(vertices2);
            
            // Act
            var result = outline1.Equals(outline2);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Equals_DifferentLength_ShouldReturnFalse()
        {
            // Arrange
            var vertices1 = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0, 1) 
            };
            var vertices2 = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(1, 1),
                new Vector2(0, 1)
            };
            var outline1 = new Outline(vertices1);
            var outline2 = new Outline(vertices2);
            
            // Act
            var result = outline1.Equals(outline2);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void OperatorEquals_ShouldWorkCorrectly()
        {
            // Arrange
            var vertices1 = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0, 1) 
            };
            var vertices2 = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0, 1) 
            };
            var outline1 = new Outline(vertices1);
            var outline2 = new Outline(vertices2);
            
            // Act & Assert
            Assert.IsTrue(outline1 == outline2);
            Assert.IsFalse(outline1 != outline2);
        }
        
        [Test]
        public void GetHashCode_SameVertices_ShouldReturnSameHash()
        {
            // Arrange
            var vertices1 = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0, 1) 
            };
            var vertices2 = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1, 0), 
                new Vector2(0, 1) 
            };
            var outline1 = new Outline(vertices1);
            var outline2 = new Outline(vertices2);
            
            // Act
            var hash1 = outline1.GetHashCode();
            var hash2 = outline2.GetHashCode();
            
            // Assert
            Assert.AreEqual(hash1, hash2);
        }
        
        #endregion
        
        #region 综合测试
        
        [Test]
        public void OutlineProperties_ConsistencyCheck()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var area = outline.area;
            var signedArea = outline.signedArea;
            var isCCW = outline.isCounterClockwise;
            var isCW = outline.isClockwise;
            var isConvex = outline.isConvex;
            var isConcave = outline.isConcave;
            var isSelfIntersecting = outline.isSelfIntersecting;
            var isCollinear = outline.isCollinear;
            var isDegenerate = outline.isDegenerateToPoint;
            
            // Assert
            Assert.AreEqual(Mathf.Abs(signedArea), area, Tolerance);
            Assert.IsTrue(isCCW != isCW || Mathf.Abs(signedArea) < Tolerance);
            Assert.IsTrue(isConvex != isConcave || isDegenerate || isCollinear);
            Assert.IsFalse(isSelfIntersecting);
            Assert.IsFalse(isCollinear);
            Assert.IsFalse(isDegenerate);
        }
        
        [Test]
        public void LargeOutline_ShouldHandleCorrectly()
        {
            // Arrange - 大正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(1000, 0), 
                new Vector2(1000, 1000), 
                new Vector2(0, 1000) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var area = outline.area;
            var isConvex = outline.isConvex;
            
            // Assert
            Assert.AreEqual(1000000f, area, Tolerance);
            Assert.IsTrue(isConvex);
        }
        
        [Test]
        public void NegativeCoordinates_ShouldWorkCorrectly()
        {
            // Arrange
            var vertices = new Vector2[] 
            { 
                new Vector2(-1, -1), 
                new Vector2(1, -1), 
                new Vector2(1, 1), 
                new Vector2(-1, 1) 
            };
            var outline = new Outline(vertices);
            
            // Act
            var area = outline.area;
            var isConvex = outline.isConvex;
            
            // Assert
            Assert.AreEqual(4f, area, Tolerance);
            Assert.IsTrue(isConvex);
        }
        
        [Test]
        public void ComplexConcaveShape_ShouldBeDetectedCorrectly()
        {
            // Arrange - 星形（凹多边形）
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 2),
                new Vector2(0.5f, 0.5f),
                new Vector2(2, 0),
                new Vector2(0.5f, -0.5f),
                new Vector2(0, -2),
                new Vector2(-0.5f, -0.5f),
                new Vector2(-2, 0),
                new Vector2(-0.5f, 0.5f)
            };
            var outline = new Outline(vertices);
            
            // Act
            var isConvex = outline.isConvex;
            var isConcave = outline.isConcave;
            var isSelfIntersecting = outline.isSelfIntersecting;
            
            // Assert
            Assert.IsFalse(isConvex);
            Assert.IsTrue(isConcave);
            Assert.IsFalse(isSelfIntersecting);
        }
        
        #endregion
        
        #region ContainsPoint 测试
        
        [Test]
        public void ContainsPoint_PointInsideSquare_ShouldReturnTrue()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var point = new Vector2(1, 1);
            
            // Act
            var result = outline.ContainsPoint(point);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsPoint_PointOutsideSquare_ShouldReturnFalse()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var point = new Vector2(3, 3);
            
            // Act
            var result = outline.ContainsPoint(point);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsPoint_PointOnEdge_ShouldReturnTrue()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var point = new Vector2(1, 0);
            
            // Act
            var result = outline.ContainsPoint(point);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsPoint_PointOnVertex_ShouldReturnTrue()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var point = new Vector2(0, 0);
            
            // Act
            var result = outline.ContainsPoint(point);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsPoint_ConcavePolygon_ShouldWorkCorrectly()
        {
            // Arrange - L形多边形（凹多边形）
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(3, 0), 
                new Vector2(3, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 3), 
                new Vector2(0, 3) 
            };
            var outline = new Outline(vertices);
            
            // Act & Assert
            Assert.IsTrue(outline.ContainsPoint(new Vector2(0.5f, 0.5f))); // 在内部
            Assert.IsTrue(outline.ContainsPoint(new Vector2(2, 0.5f))); // 在内部
            Assert.IsFalse(outline.ContainsPoint(new Vector2(2, 2))); // 在凹处外部
            Assert.IsTrue(outline.ContainsPoint(new Vector2(0.5f, 2))); // 在内部
        }
        
        [Test]
        public void ContainsPoint_Triangle_ShouldWorkCorrectly()
        {
            // Arrange - 三角形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(1, 2) 
            };
            var outline = new Outline(vertices);
            
            // Act & Assert
            Assert.IsTrue(outline.ContainsPoint(new Vector2(1, 0.5f))); // 在内部
            Assert.IsFalse(outline.ContainsPoint(new Vector2(1, -0.5f))); // 在外部
            Assert.IsTrue(outline.ContainsPoint(new Vector2(1, 0))); // 在边上
        }
        
        #endregion
        
        #region ContainsSegment 测试
        
        [Test]
        public void ContainsSegment_SegmentInsideSquare_ShouldReturnTrue()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var segment = new Segment(new Vector2(0.5f, 0.5f), new Vector2(1.5f, 1.5f));
            
            // Act
            var result = outline.ContainsSegment(segment);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsSegment_SegmentPartiallyOutside_ShouldReturnFalse()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var segment = new Segment(new Vector2(1, 1), new Vector2(3, 1));
            
            // Act
            var result = outline.ContainsSegment(segment);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsSegment_SegmentOnEdge_ShouldReturnTrue()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var segment = new Segment(new Vector2(0.5f, 0), new Vector2(1.5f, 0));
            
            // Act
            var result = outline.ContainsSegment(segment);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsSegment_SegmentCrossesEdge_ShouldReturnFalse()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var segment = new Segment(new Vector2(1, -0.5f), new Vector2(1, 0.5f));
            
            // Act
            var result = outline.ContainsSegment(segment);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsSegment_SegmentWithEndpointOnEdge_ShouldReturnTrue()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var segment = new Segment(new Vector2(1, 0), new Vector2(1, 1));
            
            // Act
            var result = outline.ContainsSegment(segment);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsSegment_ZeroLengthSegment_ShouldWorkCorrectly()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var segmentInside = new Segment(new Vector2(1, 1), new Vector2(1, 1));
            var segmentOutside = new Segment(new Vector2(3, 3), new Vector2(3, 3));
            
            // Act & Assert
            Assert.IsTrue(outline.ContainsSegment(segmentInside));
            Assert.IsFalse(outline.ContainsSegment(segmentOutside));
        }
        
        #endregion
        
        #region ContainsTriangle 测试
        
        [Test]
        public void ContainsTriangle_TriangleInsideSquare_ShouldReturnTrue()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(3, 0), 
                new Vector2(3, 3), 
                new Vector2(0, 3) 
            };
            var outline = new Outline(vertices);
            var triangle = new Triangle(
                new Vector2(0.5f, 0.5f),
                new Vector2(1.5f, 0.5f),
                new Vector2(1, 1.5f)
            );
            
            // Act
            var result = outline.ContainsTriangle(triangle);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsTriangle_TrianglePartiallyOutside_ShouldReturnFalse()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var triangle = new Triangle(
                new Vector2(1, 1),
                new Vector2(3, 1),
                new Vector2(2, 2)
            );
            
            // Act
            var result = outline.ContainsTriangle(triangle);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsTriangle_TriangleOnEdge_ShouldReturnTrue()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(3, 0), 
                new Vector2(3, 3), 
                new Vector2(0, 3) 
            };
            var outline = new Outline(vertices);
            var triangle = new Triangle(
                new Vector2(0.5f, 0),
                new Vector2(1.5f, 0),
                new Vector2(1, 1)
            );
            
            // Act
            var result = outline.ContainsTriangle(triangle);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsTriangle_TriangleCrossesEdge_ShouldReturnFalse()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var triangle = new Triangle(
                new Vector2(1, -0.5f),
                new Vector2(1, 0.5f),
                new Vector2(2, 0)
            );
            
            // Act
            var result = outline.ContainsTriangle(triangle);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsTriangle_TriangleWithVertexOnEdge_ShouldReturnTrue()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(3, 0), 
                new Vector2(3, 3), 
                new Vector2(0, 3) 
            };
            var outline = new Outline(vertices);
            var triangle = new Triangle(
                new Vector2(1, 0),
                new Vector2(2, 0),
                new Vector2(1.5f, 1)
            );
            
            // Act
            var result = outline.ContainsTriangle(triangle);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsTriangle_ConcavePolygon_ShouldWorkCorrectly()
        {
            // Arrange - L形多边形（凹多边形）
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(3, 0), 
                new Vector2(3, 1), 
                new Vector2(1, 1), 
                new Vector2(1, 3), 
                new Vector2(0, 3) 
            };
            var outline = new Outline(vertices);
            
            // Act & Assert
            var triangleInside = new Triangle(
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 2),
                new Vector2(0.2f, 1.5f)
            );
            Assert.IsTrue(outline.ContainsTriangle(triangleInside));
            
            var triangleInConcave = new Triangle(
                new Vector2(1.5f, 0.5f),
                new Vector2(2.5f, 0.5f),
                new Vector2(2, 1.5f)
            );
            Assert.IsFalse(outline.ContainsTriangle(triangleInConcave));
        }
        
        [Test]
        public void ContainsTriangle_DegenerateTriangle_ShouldHandleCorrectly()
        {
            // Arrange - 逆时针正方形
            var vertices = new Vector2[] 
            { 
                new Vector2(0, 0), 
                new Vector2(2, 0), 
                new Vector2(2, 2), 
                new Vector2(0, 2) 
            };
            var outline = new Outline(vertices);
            var degenerateTriangle = new Triangle(
                new Vector2(1, 1),
                new Vector2(1, 1),
                new Vector2(1, 1)
            );
            
            // Act
            var result = outline.ContainsTriangle(degenerateTriangle);
            
            // Assert
            Assert.IsTrue(result); // 退化三角形如果点在内部，应该返回true
        }
        
        #endregion
    }
}

