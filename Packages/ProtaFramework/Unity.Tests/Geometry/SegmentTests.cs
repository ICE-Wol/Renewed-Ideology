using System;
using UnityEngine;
using NUnit.Framework;
using Prota.Unity;

namespace Geometry
{
    public class SegmentTests
    {
        private const float Tolerance = 1e-6f;
        
        // 测试常量 Eps 的使用
        [Test]
        public void EpsConstant_ShouldBeUsedInTolerance()
        {
            // 这个测试验证 Eps 常量被正确使用
            // 通过测试边界情况来验证容差设置
            var segment = new Segment(new Vector2(0, 0), new Vector2(1, 0));
            var point = new Vector2(0.5f, 0.0000001f); // 非常接近线段
            
            // 应该返回 true，因为点在容差范围内
            Assert.IsTrue(segment.ContainsPoint(point));
        }
        
        #region 基本属性测试
        
        [Test]
        public void Center_ShouldReturnCorrectCenter()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 6));
            
            // Act
            var center = segment.center;
            
            // Assert
            Assert.AreEqual(new Vector2(2, 3), center);
        }
        
        [Test]
        public void Direction_ShouldReturnNormalizedDirection()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(3, 4));
            
            // Act
            var direction = segment.direction;
            
            // Assert
            Assert.AreEqual(0.6f, direction.x, Tolerance);
            Assert.AreEqual(0.8f, direction.y, Tolerance);
            Assert.AreEqual(1f, direction.magnitude, Tolerance);
        }
        
        [Test]
        public void Delta_ShouldReturnCorrectDelta()
        {
            // Arrange
            var segment = new Segment(new Vector2(1, 2), new Vector2(5, 8));
            
            // Act
            var delta = segment.delta;
            
            // Assert
            Assert.AreEqual(new Vector2(4, 6), delta);
        }
        
        [Test]
        public void Length_ShouldReturnCorrectLength()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(3, 4));
            
            // Act
            var length = segment.length;
            
            // Assert
            Assert.AreEqual(5f, length, Tolerance);
        }
        
        [Test]
        public void Length_ZeroLengthSegment_ShouldReturnZero()
        {
            // Arrange
            var point = new Vector2(1, 1);
            var segment = new Segment(point, point);
            
            // Act
            var length = segment.length;
            
            // Assert
            Assert.AreEqual(0f, length, Tolerance);
            Assert.IsTrue(segment.isZeroLength);
        }
        
        #endregion
        
        #region 点在线段上判断测试
        
        [Test]
        public void ContainsPoint_PointOnSegment_ShouldReturnTrue()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var point = new Vector2(2, 0);
            
            // Act
            var result = segment.ContainsPoint(point);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsPoint_PointAtStart_ShouldReturnTrue()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var point = new Vector2(0, 0);
            
            // Act
            var result = segment.ContainsPoint(point);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsPoint_PointAtEnd_ShouldReturnTrue()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var point = new Vector2(4, 0);
            
            // Act
            var result = segment.ContainsPoint(point);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void ContainsPoint_PointOutsideSegment_ShouldReturnFalse()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var point = new Vector2(5, 0);
            
            // Act
            var result = segment.ContainsPoint(point);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsPointExclusive_PointAtStart_ShouldReturnFalse()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var point = new Vector2(0, 0);
            
            // Act
            var result = segment.ContainsPointExclusive(point);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsPointExclusive_PointAtEnd_ShouldReturnFalse()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var point = new Vector2(4, 0);
            
            // Act
            var result = segment.ContainsPointExclusive(point);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsPointExclusive_PointInMiddle_ShouldReturnTrue()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var point = new Vector2(2, 0);
            
            // Act
            var result = segment.ContainsPointExclusive(point);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        #endregion
        
        #region 平行检测测试
        
        [Test]
        public void IsParallelWith_ParallelSegments_ShouldReturnTrue()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new Segment(new Vector2(0, 1), new Vector2(2, 1));
            
            // Act
            var result = segment1.line.IsParallelWith(segment2.line);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsParallelWith_NonParallelSegments_ShouldReturnFalse()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new Segment(new Vector2(0, 0), new Vector2(1, 1));
            
            // Act
            var result = segment1.line.IsParallelWith(segment2.line);
            
            // Assert
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsParallelWith_VerticalParallelSegments_ShouldReturnTrue()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(0, 2));
            var segment2 = new Segment(new Vector2(1, 0), new Vector2(1, 2));
            
            // Act
            var result = segment1.line.IsParallelWith(segment2.line);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsParallelWith_DiagonalParallelSegments_ShouldReturnTrue()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 2));
            var segment2 = new Segment(new Vector2(1, 0), new Vector2(3, 2));
            
            // Act
            var result = segment1.line.IsParallelWith(segment2.line);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsParallelWith_OppositeDirectionParallelSegments_ShouldReturnTrue()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new Segment(new Vector2(2, 1), new Vector2(0, 1));
            
            // Act
            var result = segment1.line.IsParallelWith(segment2.line);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsParallelWith_ZeroLengthSegment_ShouldThrowException()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new Segment(new Vector2(1, 1), new Vector2(1, 1));
            
            // Act & Assert
            // 零长度Segment无法创建Line，会抛出异常
            Assert.Throws<ArgumentException>(() => { var _ = segment2.line; });
            Assert.Throws<InvalidOperationException>(() => { var _ = segment2.direction; });
        }
        
        [Test]
        public void IsParallelWith_BothZeroLengthSegments_ShouldThrowException()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(0, 0));
            var segment2 = new Segment(new Vector2(1, 1), new Vector2(1, 1));
            
            // Act & Assert
            // 零长度Segment无法创建Line，会抛出异常
            Assert.Throws<ArgumentException>(() => { var _ = segment1.line; });
            Assert.Throws<ArgumentException>(() => { var _ = segment2.line; });
            Assert.Throws<InvalidOperationException>(() => { var _ = segment1.direction; });
            Assert.Throws<InvalidOperationException>(() => { var _ = segment2.direction; });
        }
        
        [Test]
        public void IsParallelWith_NearlyParallelSegments_ShouldReturnFalse()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(1, 0));
            var segment2 = new Segment(new Vector2(0, 1), new Vector2(1, 0.0001f)); // 几乎平行
            
            // Act
            var result = segment1.line.IsParallelWith(segment2.line);
            
            // Assert
            // 使用默认 Eps 应该返回 false
            Assert.IsFalse(result);
        }
        
        [Test]
        public void IsParallelWith_IdenticalSegments_ShouldReturnTrue()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            
            // Act
            var result = segment1.line.IsParallelWith(segment2.line);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        [Test]
        public void IsParallelWith_ColinearSegments_ShouldReturnTrue()
        {
            // Arrange
            var segment1 = new Segment(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new Segment(new Vector2(1, 0), new Vector2(3, 0));
            
            // Act
            var result = segment1.line.IsParallelWith(segment2.line);
            
            // Assert
            Assert.IsTrue(result);
        }
        
        #endregion
        
        #region 辅助方法测试
        
        [Test]
        public void GetClosestPoint_PointOnSegment_ShouldReturnSamePoint()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var point = new Vector2(2, 0);
            
            // Act
            var closestPoint = segment.GetClosestPoint(point);
            
            // Assert
            Assert.AreEqual(point, closestPoint);
        }
        
        [Test]
        public void GetClosestPoint_PointOutsideSegment_ShouldReturnClosestEndpoint()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var point = new Vector2(6, 0);
            
            // Act
            var closestPoint = segment.GetClosestPoint(point);
            
            // Assert
            Assert.AreEqual(new Vector2(4, 0), closestPoint);
        }
        
        [Test]
        public void GetClosestPointParameter_PointOnSegment_ShouldReturnCorrectParameter()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var point = new Vector2(2, 0);
            
            // Act
            var parameter = segment.GetClosestPointParameter(point);
            
            // Assert
            Assert.AreEqual(0.5f, parameter, Tolerance);
        }
        
        #endregion
        
        #region 边界情况测试
        
        [Test]
        public void ZeroLengthSegment_AllMethods_ShouldHandleCorrectly()
        {
            // Arrange
            var point = new Vector2(1, 1);
            var segment = new Segment(point, point);
            
            // Act & Assert
            Assert.IsTrue(segment.isZeroLength);
            Assert.IsTrue(segment.ContainsPoint(point));
            Assert.IsFalse(segment.ContainsPointExclusive(point));
            Assert.AreEqual(0f, segment.length, Tolerance);
            Assert.AreEqual(new Vector2(1, 1), segment.center);
            Assert.Throws<InvalidOperationException>(() => { var _ = segment.direction; });
            Assert.Throws<ArgumentException>(() => { var _ = segment.line; });
        }
        
        
        [Test]
        public void VerticalSegment_ShouldWorkCorrectly()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(0, 4));
            var point = new Vector2(0, 2);
            
            // Act & Assert
            Assert.IsTrue(segment.ContainsPoint(point));
            Assert.AreEqual(new Vector2(0, 1), segment.direction);
            Assert.AreEqual(4f, segment.length, Tolerance);
        }
        
        [Test]
        public void HorizontalSegment_ShouldWorkCorrectly()
        {
            // Arrange
            var segment = new Segment(new Vector2(0, 0), new Vector2(4, 0));
            var point = new Vector2(2, 0);
            
            // Act & Assert
            Assert.IsTrue(segment.ContainsPoint(point));
            Assert.AreEqual(new Vector2(1, 0), segment.direction);
            Assert.AreEqual(4f, segment.length, Tolerance);
        }
        
        #endregion
    }
}


