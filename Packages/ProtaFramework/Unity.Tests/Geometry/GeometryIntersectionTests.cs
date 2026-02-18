using System;
using UnityEngine;
using NUnit.Framework;
using Prota.Unity;


namespace Geometry
{
    public class GeometryIntersectionTests
    {
        private const float Tolerance = 1e-6f;
        
        #region Line 与 Line 相交测试
        
        [Test]
        public void Line_IntersectsWith_IntersectingLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(4, 4));
            var line2 = new Line(new Vector2(0, 4), new Vector2(4, 0));
            
            // Act
            var result = line1.IntersectsWith(line2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(2, 2), intersection);
        }
        
        [Test]
        public void Line_IntersectsWith_ParallelLines_ShouldReturnFalse()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(0, 1), new Vector2(2, 1));
            
            // Act
            var result = line1.IntersectsWith(line2, out var intersection);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Line_IntersectsWith_CoincidentLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Line(new Vector2(1, 0), new Vector2(3, 0));
            
            // Act
            var result = line1.IntersectsWith(line2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(0, 0), intersection);
        }
        
        [Test]
        public void Line_IntersectsWith_VerticalLines_ShouldReturnFalse()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 0), new Vector2(0, 4));
            var line2 = new Line(new Vector2(2, 0), new Vector2(2, 4));
            
            // Act
            var result = line1.IntersectsWith(line2, out var intersection);
            
            // Assert
            Assert.IsFalse(result); // 平行线
        }
        
        [Test]
        public void Line_IntersectsWith_HorizontalAndVertical_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Line(new Vector2(0, 2), new Vector2(4, 2));
            var line2 = new Line(new Vector2(2, 0), new Vector2(2, 4));
            
            // Act
            var result = line1.IntersectsWith(line2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(2, 2), intersection);
        }
        
        #endregion
        
        #region Segment 与 Segment 相交测试（包括顶点）
        
        [Test]
        public void Segment_IntersectsWith_IntersectingSegments_ShouldReturnTrue()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(4, 4));
            var segment2 = new Segment(new Vector2(0, 4), new Vector2(4, 0));
            
            // Act
            var result = segment1.IntersectsWith(segment2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(2, 2), intersection);
        }
        
        [Test]
        public void Segment_IntersectsWith_NonIntersectingSegments_ShouldReturnFalse()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 2));
            var segment2 = new Segment(new Vector2(3, 3), new Vector2(5, 5));
            
            // Act
            var result = segment1.IntersectsWith(segment2, out var intersection);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Segment_IntersectsWith_TouchingAtEndpoints_ShouldReturnTrue()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new Segment(new Vector2(2, 0), new Vector2(4, 0));
            
            // Act
            var result = segment1.IntersectsWith(segment2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(2, 0), intersection);
        }
        
        [Test]
        public void Segment_IntersectsWith_ParallelSegments_ShouldReturnFalse()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new Segment(new Vector2(0, 1), new Vector2(2, 1));
            
            // Act
            var result = segment1.IntersectsWith(segment2, out var intersection);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Segment_IntersectsWith_ColinearOverlappingSegments_ShouldReturnTrue()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var segment2 = new Segment(new Vector2(2, 0), new Vector2(6, 0));
            
            // Act
            var result = segment1.IntersectsWith(segment2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(3, 0), intersection);
        }
        
        [Test]
        public void Segment_IntersectsWith_ColinearNonOverlappingSegments_ShouldReturnFalse()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new Segment(new Vector2(3, 0), new Vector2(5, 0));
            
            // Act
            var result = segment1.IntersectsWith(segment2, out var intersection);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Segment_IntersectsWith_ColinearTouchingSegments_ShouldReturnTrue()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new Segment(new Vector2(2, 0), new Vector2(4, 0));
            
            // Act
            var result = segment1.IntersectsWith(segment2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(2, 0), intersection);
        }
        
        [Test]
        public void Segment_IntersectsWith_ZeroLengthSegment_ShouldWork()
        {
            // Arrange - 零长度线段（点）
            var point = new Vector2(1, 1);
            var segment1 = new Segment(point, point);
            var segment2 = new Segment(new Vector2(0, 0), new Vector2(2, 2));
            
            // Act
            var result = segment1.IntersectsWith(segment2, out var intersection);
            
            // Assert
            Assert.IsTrue(segment1.isZeroLength);
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(1, 1), intersection);
        }
        
        #endregion
        
        #region Segment 与 Segment 相交测试（不包括顶点）
        
        [Test]
        public void Segment_IntersectsWithExclusive_IntersectingSegments_ShouldReturnTrue()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(4, 4));
            var segment2 = new Segment(new Vector2(0, 4), new Vector2(4, 0));
            
            // Act
            var result = segment1.IntersectsWithExclusive(segment2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(2, 2), intersection);
        }
        
        [Test]
        public void Segment_IntersectsWithExclusive_TouchingAtEndpoints_ShouldReturnFalse()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new Segment(new Vector2(2, 0), new Vector2(4, 0));
            
            // Act
            var result = segment1.IntersectsWithExclusive(segment2, out var intersection);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Segment_IntersectsWithExclusive_ParallelSegments_ShouldReturnFalse()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new Segment(new Vector2(0, 1), new Vector2(2, 1));
            
            // Act
            var result = segment1.IntersectsWithExclusive(segment2, out var intersection);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        #endregion
        
        #region Segment 与 Line 相交测试
        
        [Test]
        public void Segment_IntersectsLineWith_IntersectingLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Segment(new Vector2(0, 0), new Vector2(4, 4));
            var line2 = new Segment(new Vector2(0, 4), new Vector2(4, 0));
            
            // Act
            var result = line1.IntersectsLineWith(line2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(2, 2), intersection);
        }
        
        [Test]
        public void Segment_IntersectsLineWith_ParallelLines_ShouldReturnFalse()
        {
            // Arrange
            var line1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Segment(new Vector2(0, 1), new Vector2(2, 1));
            
            // Act
            var result = line1.IntersectsLineWith(line2, out var intersection);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Segment_IntersectsLineWith_CoincidentLines_ShouldReturnTrue()
        {
            // Arrange
            var line1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var line2 = new Segment(new Vector2(1, 0), new Vector2(3, 0));
            
            // Act
            var result = line1.IntersectsLineWith(line2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(0, 0), intersection);
        }
        
        [Test]
        public void Segment_IntersectsSegmentWithLine_Intersecting_ShouldReturnTrue()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var line = new Segment(new Vector2(2, -2), new Vector2(2, 2));
            
            // Act
            var result = segment.IntersectsSegmentWithLine(line, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(2, 0), intersection);
        }
        
        [Test]
        public void Segment_IntersectsSegmentWithLine_LineOutsideSegment_ShouldReturnFalse()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var line = new Segment(new Vector2(3, -2), new Vector2(3, 2));
            
            // Act
            var result = segment.IntersectsSegmentWithLine(line, out var intersection);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Segment_IntersectsSegmentWithLineExclusive_IntersectingAtEndpoint_ShouldReturnFalse()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var line = new Segment(new Vector2(0, -2), new Vector2(0, 2));
            
            // Act
            var result = segment.IntersectsSegmentWithLineExclusive(line, out var intersection);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Segment_IntersectsSegmentWithLineExclusive_IntersectingInMiddle_ShouldReturnTrue()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var line = new Segment(new Vector2(2, -2), new Vector2(2, 2));
            
            // Act
            var result = segment.IntersectsSegmentWithLineExclusive(line, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(2, 0), intersection);
        }
        
        #endregion
        
        #region Triangle 与 Segment 相交测试
        
        [Test]
        public void Triangle_IntersectsWith_SegmentCrossingTriangle_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var segment = new Segment(new Vector2(0, 1), new Vector2(2, 1));
            
            // Act
            var result = triangle.IntersectsWith(segment, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Triangle_IntersectsWith_SegmentInsideTriangle_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var segment = new Segment(new Vector2(0.5f, 0.5f), new Vector2(1.5f, 0.5f));
            
            // Act
            var result = triangle.IntersectsWith(segment, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Triangle_IntersectsWith_SegmentOutsideTriangle_ShouldReturnFalse()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var segment = new Segment(new Vector2(3, 0), new Vector2(3, 2));
            
            // Act
            var result = triangle.IntersectsWith(segment, out var intersection);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Triangle_IntersectsWith_SegmentTouchingEdge_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var segment = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            
            // Act
            var result = triangle.IntersectsWith(segment, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Triangle_IntersectsWith_SegmentTouchingVertex_ShouldReturnTrue()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var segment = new Segment(new Vector2(0, 0), new Vector2(0, 2));
            
            // Act
            var result = triangle.IntersectsWith(segment, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Triangle_IntersectsWith_SegmentParallelToEdge_ShouldReturnFalse()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var segment = new Segment(new Vector2(0, 3), new Vector2(2, 3));
            
            // Act
            var result = triangle.IntersectsWith(segment, out var intersection);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Triangle_IntersectsWith_Segment_AllEdgeCases()
        {
            // Arrange
            var triangle = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            
            // Test various segment positions
            var segment1 = new Segment(new Vector2(0.5f, 0.5f), new Vector2(1.5f, 0.5f)); // 完全在内部
            var segment2 = new Segment(new Vector2(3, 0), new Vector2(3, 2)); // 完全在外部
            var segment3 = new Segment(new Vector2(0, 0), new Vector2(2, 0)); // 与底边重合
            var segment4 = new Segment(new Vector2(1, 0), new Vector2(1, 2)); // 穿过中心
            
            // Act & Assert
            Assert.IsTrue(triangle.IntersectsWith(segment1, out _));
            Assert.IsFalse(triangle.IntersectsWith(segment2, out _));
            Assert.IsTrue(triangle.IntersectsWith(segment3, out _));
            Assert.IsTrue(triangle.IntersectsWith(segment4, out _));
        }
        
        #endregion
        
        #region Triangle 与 Triangle 相交测试
        
        [Test]
        public void Triangle_IntersectsWith_OverlappingTriangles_ShouldReturnTrue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var triangle2 = new Triangle(new Vector2(1, 0), new Vector2(3, 0), new Vector2(2, 2));
            
            // Act
            var result = triangle1.IntersectsWith(triangle2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Triangle_IntersectsWith_OneTriangleInsideAnother_ShouldReturnTrue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(4, 0), new Vector2(2, 4));
            var triangle2 = new Triangle(new Vector2(1, 1), new Vector2(3, 1), new Vector2(2, 2));
            
            // Act
            var result = triangle1.IntersectsWith(triangle2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Triangle_IntersectsWith_DisjointTriangles_ShouldReturnFalse()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 1));
            var triangle2 = new Triangle(new Vector2(3, 0), new Vector2(4, 0), new Vector2(3.5f, 1));
            
            // Act
            var result = triangle1.IntersectsWith(triangle2, out var intersection);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void Triangle_IntersectsWith_TrianglesTouchingAtVertex_ShouldReturnTrue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var triangle2 = new Triangle(new Vector2(1, 2), new Vector2(3, 2), new Vector2(2, 4));
            
            // Act
            var result = triangle1.IntersectsWith(triangle2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(1, 2), intersection);
        }
        
        [Test]
        public void Triangle_IntersectsWith_TrianglesTouchingAtEdge_ShouldReturnTrue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, -2));
            
            // Act
            var result = triangle1.IntersectsWith(triangle2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Triangle_IntersectsWith_EdgeCrossing_ShouldReturnTrue()
        {
            // Arrange
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var triangle2 = new Triangle(new Vector2(1, 1), new Vector2(3, 1), new Vector2(2, 3));
            
            // Act
            var result = triangle1.IntersectsWith(triangle2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void Triangle_IntersectsWith_EdgeCases()
        {
            // Arrange - 两个三角形共享一条边
            var triangle1 = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, 2));
            var triangle2 = new Triangle(new Vector2(0, 0), new Vector2(2, 0), new Vector2(1, -2));
            
            // Act
            var result = triangle1.IntersectsWith(triangle2, out var intersection);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        #endregion
    }
}

